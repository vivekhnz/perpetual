using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastDamage : MonoBehaviour {

    public float blastDamage;
    public float playerDamageModifier; // The damage modifier applied to the player. Normally < 1.

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
    }
}
