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

    void Start()
    {
        Parent = Parent ?? gameObject;
        CurrentHealth = InitialHealth;

        if (!gameObject.CompareTag("Damageable"))
            Debug.LogWarning("This object does not have the 'Damageable' tag. Objects may be unable to damage it.");
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
        {
            OnDestroyed.Invoke();
        }

        // increase score
        if (hudController == null)
            hudController = Object.FindObjectOfType<HUDController>();
        if (hudController != null)
        {
            hudController.AddScore(ScoreValue);

            // create score popup
            Vector2 velocity = Vector2.zero;
            if (damageAngle.HasValue)
                velocity = new Vector2(
                    Mathf.Cos(damageAngle.Value * Mathf.Deg2Rad),
                    Mathf.Sin(damageAngle.Value * Mathf.Deg2Rad)) * 0.1f;
            hudController.CreateScorePopup(ScoreValue, transform.position, velocity);
        }

        // create explosion
        if (Explosion != null)
            Explode(damageAngle);

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
}
