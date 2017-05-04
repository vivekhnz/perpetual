using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastDamage : MonoBehaviour {

    public float blastDamage; // Damage of the explosion.
    public float playerDamageModifier; // The damage modifier applied to the player. Normally < 1.
    public float blastForce; // Determines how powerful the blast will push other objects.

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
                    damageable.TakeDamage(blastDamage,
                        transform.rotation.eulerAngles.z);
                break;
            case "Player":
                // Get the player's health script and damage the player.
                PlayerHealth playerHeath = collider.GetComponentInChildren<PlayerHealth>();
                playerHeath.TakeDamage(blastDamage * playerDamageModifier);
                break;
        }

        // Knockback the objects after damage has been dealt.
        // Early mobs will die instantly to explosion, so it mainly affects player.
        KnockBackObjects();
    }

    // Knocks back all damageable objects.
    void KnockBackObjects()
    {
        // Get origin of explosion to prepare for vector calculations.
        Vector3 explosionPos = transform.position;

        // Get ALL colliders that are in the blast radius.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, blastRadius);

        // Apply blast force to all colliding GameObjects that have Rigidbodies.
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                AddExplosionForce(rb, blastForce, explosionPos, blastRadius);
            }
        }
    }

    // Creates an explosive force that moves a rigidbody away from the source of the explosion.
    private void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        // Calculate the vector of the explosion by subtracting the explosion's position from the target position.
        var dir = (body.transform.position - explosionPosition);

        // Calculate a proportional decrease in power the further away the target is from the explosion.
        float wearoff = 1 - (dir.magnitude / explosionRadius);

        // Add the force to the rigidbody as an Impulse force for immediate effect.
        body.AddForce(dir.normalized * explosionForce * wearoff, ForceMode2D.Impulse);
    }
}
