﻿using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HealthPickupController : MonoBehaviour
{
    public float HealthGained = 30;
    public float TimeTilDespawn = 5;
    public float TimeBeforeDespawnToFlash = 3;
    public AudioSource PickupCollectedSound;

    private Animator animator;
    private float spawnTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        spawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time - spawnTime > TimeTilDespawn)
        {
            Destroy(gameObject);
            return;
        }
        animator.SetBool("IsDespawning",
            Time.time - spawnTime > TimeTilDespawn - TimeBeforeDespawnToFlash);
    }

    public void OnCollected(Collider2D other)
    {
        // restore player health
        var health = other.gameObject.GetComponent<PlayerHealth>();
        health.RestoreHealth(HealthGained);

        // play sound
        var sound = Instantiate(PickupCollectedSound);
        Destroy(sound.gameObject, sound.clip.length);

        // disappear
        Destroy(gameObject);
    }
}
