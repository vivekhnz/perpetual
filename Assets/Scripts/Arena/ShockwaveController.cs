using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ShockwaveController : PooledObject
{
    // damage of the shockwave
    public float DamageToEnemies = 75.0f;
    public float DamageToPlayer = 25.0f;
    // how much force to push nearby objects with
    public float ForceAgainstDamageables = 8f;
    public float ForceAgainstPlayer = 8f;

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Damageable":
                if (DamageToEnemies < 1)
                    return;

                // damage the object
                var damageable = collider.GetComponent<DamageableObject>();
                if (damageable != null)
                    damageable.TakeDamage(DamageToEnemies,
                        transform.rotation.eulerAngles.z);

                Push(collider.GetComponent<PushableObject>(), ForceAgainstDamageables);
                break;
            case "Player":
                if (DamageToPlayer < 1)
                    return;

                // damage the player
                PlayerHealth playerHeath = collider.GetComponentInChildren<PlayerHealth>();
                playerHeath.TakeDamage(DamageToPlayer, "Shockwave");

                Push(collider.GetComponentInChildren<PushableObject>(), ForceAgainstPlayer);
                break;
        }
    }

    void Push(PushableObject pushable, float force)
    {
        // knock back the object if it can be pushed
        if (pushable != null)
        {
            var direction = pushable.transform.position - transform.position;
            pushable.Push(direction.normalized, force);
        }
    }

    void SelfDestruct()
    {
        Recycle();
    }
}
