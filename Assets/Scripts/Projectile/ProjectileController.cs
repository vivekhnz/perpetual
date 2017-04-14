using UnityEngine;

public class ProjectileController : PooledObject
{
    public float MovementSpeed = 4.0f;
    public float Damage = 10.0f;

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
                break;
            case "Damageable":
                // damage the object
                var damageable = collider.GetComponent<DamageableObject>();
                if (damageable != null)
                    damageable.TakeDamage(Damage);

                // destroy this projectile
                Recycle();
                break;
        }
    }
}
