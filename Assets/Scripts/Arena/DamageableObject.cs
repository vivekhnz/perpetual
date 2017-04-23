using UnityEngine;
using UnityEngine.Events;

public class DamageableObject : MonoBehaviour
{
    public float InitialHealth = 100;
    public int ScoreValue = 5;
    public GameObject Parent;
    public ParticleSystem Explosion;
    public UnityEvent OnDamaged;

    private float currentHealth;
    private HUDController hudController;

    void Start()
    {
        Parent = Parent ?? gameObject;
        currentHealth = InitialHealth;

        if (!gameObject.CompareTag("Damageable"))
            Debug.LogWarning("This object does not have the 'Damageable' tag. Objects may be unable to damage it.");
    }

    public void TakeDamage(float damage, float? damageAngle = null)
    {
        // reduce health
        currentHealth -= damage;

        // raise damaged event
        if (OnDamaged != null)
            OnDamaged.Invoke();

        // am I dead?
        if (currentHealth <= 0)
        {
            // increase score
            if (hudController == null)
                hudController = Object.FindObjectOfType<HUDController>();
            if (hudController != null)
                hudController.AddScore(ScoreValue);

            // create explosion
            if (Explosion != null)
            {
                var explosion = Instantiate(Explosion);
                explosion.transform.position = transform.position;

                // was the damage angle specified?
                if (damageAngle.HasValue)
                {
                    // if so, apply velocity in the direction of the damage
                    Vector2 direction = new Vector2(
                        Mathf.Cos(damageAngle.Value * Mathf.Deg2Rad),
                        Mathf.Sin(damageAngle.Value * Mathf.Deg2Rad));
                    var velocity = explosion.velocityOverLifetime;
                    velocity.enabled = true;
                    velocity.x = new ParticleSystem.MinMaxCurve(direction.x * 3.0f);
                    velocity.y = new ParticleSystem.MinMaxCurve(direction.y * 3.0f);
                }
            }

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
    }

    public void ResetHealth()
    {
        currentHealth = InitialHealth;
    }
}
