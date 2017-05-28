using UnityEngine;

public class ProjectileController : PooledObject
{
    public float MovementSpeed = 4.0f;
    public float Damage = 10.0f;
    public float KnockbackForce = 1.0f;
    public float Lifetime = 0.0f;
    public ParticleSystemAutoDestroy ProjectileExplosion;

    private float timeFired;

    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        timeFired = Time.time;
    }

    void FixedUpdate()
    {
        transform.Translate(
            Vector3.right * Time.deltaTime * MovementSpeed);

        if (Lifetime > 0 && Time.time - timeFired > Lifetime)
            Recycle();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Solid":
                // destroy this projectile
                Recycle();
                CreateExplosion();
                break;
            case "Damageable":
                // damage the object
                var damageable = collider.GetComponent<DamageableObject>();
                if (damageable != null)
                    damageable.TakeDamage(Damage,
                        transform.rotation.eulerAngles.z);

                // knock back the object if it can be pushed
                var pushable = collider.GetComponent<PushableObject>();
                if (pushable != null)
                {
                    var direction = pushable.transform.position
                        - transform.position;
                    pushable.Push(direction.normalized, KnockbackForce);
                }

                // destroy this projectile
                Recycle();
                CreateExplosion();
                break;
        }
    }

    private void CreateExplosion()
    {
        var explosion = ProjectileExplosion.Fetch<ParticleSystemAutoDestroy>();
        explosion.transform.position = transform.position;

        float angle = transform.rotation.eulerAngles.z + 180.0f;
        Vector2 direction = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad));

        var velocity = explosion.ParticleSystem.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x = new ParticleSystem.MinMaxCurve(direction.x * 3.0f);
        velocity.y = new ParticleSystem.MinMaxCurve(direction.y * 3.0f);
    }
}
