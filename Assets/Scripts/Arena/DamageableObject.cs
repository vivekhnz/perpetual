using UnityEngine;
using UnityEngine.Events;

public class DamageableObject : MonoBehaviour
{
    public float InitialHealth = 100;
    public int ScoreValue = 5;
    public float CameraShakeAmount = 5.0f;
    public GameObject Parent;
    public ParticleSystemAutoDestroy Explosion;
    public UnityEvent OnDamaged;
    public UnityEvent OnDestroyed;

    public float CurrentHealth { get; private set; }
    private HUDController hudController;
    private AudioSource explosionSound;
    private GameObject audioListener;

    void Start()
    {
        Parent = Parent ?? gameObject;
        CurrentHealth = InitialHealth;

        if (!gameObject.CompareTag("Damageable"))
            Debug.LogWarning("This object does not have the 'Damageable' tag. Objects may be unable to damage it.");

        explosionSound = GetComponent<AudioSource>();

        // used so that explosion sound clip sounds louder
        audioListener = GameObject.Find("Camera");
    }

    public void TakeDamage(float damage, float? damageAngle = null)
    {
        // reduce health
        CurrentHealth -= damage;

        // raise damaged event
        if (OnDamaged != null)
            OnDamaged.Invoke();

        // am I dead?
        if (CurrentHealth <= 0)
            Die(damageAngle);
    }

    public void ResetHealth()
    {
        CurrentHealth = InitialHealth;
    }

    private void Die(float? damageAngle)
    {
        // ensure we have zero health
        CurrentHealth = 0;

        // raise destroyed event
        if (OnDestroyed != null)
            OnDestroyed.Invoke();

        // increase score
        if (hudController == null)
            hudController = Object.FindObjectOfType<HUDController>();
        if (hudController != null)
            hudController.AddScore(ScoreValue);

        // create explosion
        if (Explosion != null)
            Explode(damageAngle);

        PlayExplosionSoundClip();

        // recycle the object if it is poolable, destroy it otherwise
        var poolable = Parent.GetComponent<PooledObject>();
        if (poolable == null)
        {
            Destroy(Parent);        
        }
        else
        {
            poolable.Recycle();
        }
    }

    private void Explode(float? damageAngle)
    {
        var explosion = Explosion.Fetch<ParticleSystemAutoDestroy>();
        explosion.transform.position = transform.position;

        // was the damage angle specified?
        if (damageAngle.HasValue)
        {
            // if so, apply velocity in the direction of the damage
            Vector2 direction = new Vector2(
                Mathf.Cos(damageAngle.Value * Mathf.Deg2Rad),
                Mathf.Sin(damageAngle.Value * Mathf.Deg2Rad));
            var velocity = explosion.ParticleSystem.velocityOverLifetime;
            velocity.enabled = true;
            velocity.x = new ParticleSystem.MinMaxCurve(direction.x * 3.0f);
            velocity.y = new ParticleSystem.MinMaxCurve(direction.y * 3.0f);
        }

        // shake camera
        var shaker = Camera.main.GetComponent<CameraShake>();
        if (shaker != null)
            shaker.RandomShake(CameraShakeAmount);
    }

    public void SelfDestruct()
    {
        Die(null);
    }

    public void PlayExplosionSoundClip()
    {
        // play explosion sound as an audioClip
        if (explosionSound != null)
        {
            AudioClip explosionSoundClip = explosionSound.clip;

            // the resulting float should be around 0.5f. Higher the health, bigger the boom!
            float explosionVolume = InitialHealth / 200;

            // calculate where the explosion sound plays to make it louder or quieter
            Vector3 PosOfExplosionSound = (audioListener.transform.position - transform.position) * explosionVolume;

            // plays the audio clip even if gameObject is recycled
            AudioSource.PlayClipAtPoint(explosionSoundClip, PosOfExplosionSound);
        }
    }
}
