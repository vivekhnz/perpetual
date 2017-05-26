using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupController : MonoBehaviour {

    public float HealthGained = 30;
    public float TimeTilDespawn = 5;
    public float TimeBeforeDespawnToFlash = 3;
    public AudioClip HealthPickedUpSfx;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // health pickup will disappear after set time
        Invoke("Disappear", TimeTilDespawn);
        Invoke("StartFlashing", TimeTilDespawn - TimeBeforeDespawnToFlash);
    }

    void Disappear()
    {
        Destroy(gameObject);
    }

    void StartFlashing()
    {
        // makes the sprite flash on and off just before despawn
        animator.SetBool("IsDespawning", true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.tag)
        {
            case "Player":
                // give player health
                other.gameObject.GetComponentInChildren<PlayerHealth>().GainHealth(HealthGained);
                AudioSource.PlayClipAtPoint(HealthPickedUpSfx, transform.position);
                Disappear();
                break;
            default:
                break;
        }
    }
}
