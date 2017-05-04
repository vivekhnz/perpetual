using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastDamage : MonoBehaviour
{
    // damage of the explosion
    public float Damage = 200f;
    // damage modifier applied to the player (normally < 1)
    public float PlayerDamageModifier = 0.1f;
    // how much force to push nearby objects with
    public float Force = 5f;

    private float blastRadius;

    void Start()
    {
        // Obtain the blast radius from the radius of the circle collider for consistency.
        blastRadius = GetComponent<CircleCollider2D>().radius;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Damages other objects from the blast.
        switch (collider.gameObject.tag)
        {
            case "Damageable":
                // damage the object
                var damageable = collider.GetComponent<DamageableObject>();
                if (damageable != null)
                    damageable.TakeDamage(Damage,
                        transform.rotation.eulerAngles.z);
                break;
            case "Player":
                // Get the player's health script and damage the player.
                PlayerHealth playerHeath = collider.GetComponentInChildren<PlayerHealth>();
                playerHeath.TakeDamage(Damage * PlayerDamageModifier);
                break;
        }

        // Knock back the object if it can be pushed
        var pushable = collider.GetComponent<PushableObject>();
        if (pushable != null)
        {
            var direction = pushable.transform.position
                - transform.position;
            float falloff = 1f - (direction.magnitude / blastRadius); 
            pushable.Push(direction.normalized, Force * falloff);
        }
    }
}
