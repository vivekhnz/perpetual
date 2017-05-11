using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileController : PooledObject
{
    public float MovementSpeed = 4.0f;
    public float Damage = 10.0f;
    public float KnockbackForce = 1.0f;
    public ParticleSystemAutoDestroy ProjectileExplosion;

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
            case "PlayerCollider":
                // damage the player
                var damageable = collider.GetComponent<PlayerHealth>();
                if (damageable != null)
                    damageable.TakeDamage(Damage, "Boss");

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
