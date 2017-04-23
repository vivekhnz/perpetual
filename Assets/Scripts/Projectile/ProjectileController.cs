using UnityEngine;

public class ProjectileController : PooledObject
{
    public float MovementSpeed = 4.0f;
    public float Damage = 10.0f;
    public ParticleSystem ProjectileExplosion;

    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    void FixedUpdate()
    {
        transform.Translate(
            Vector3.right * Time.deltaTime * MovementSpeed);
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

                // destroy this projectile
                Recycle();
                CreateExplosion();
                break;
        }
    }

    private void CreateExplosion()
    {
        var explosion = Instantiate(ProjectileExplosion);
        explosion.transform.position = transform.position;
        
        float angle = transform.rotation.eulerAngles.z + 180.0f;
        Vector2 direction = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad));

        var velocity = explosion.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x = new ParticleSystem.MinMaxCurve(direction.x * 3.0f);
        velocity.y = new ParticleSystem.MinMaxCurve(direction.y * 3.0f);
    }
}
