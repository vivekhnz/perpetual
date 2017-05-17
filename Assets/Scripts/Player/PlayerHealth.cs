﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public PlayerMovement Parent;
    public float InitialHealth;
    public float InvincibilityDuration = 1.0f;

    private HUDController hudController;
    private CameraShake shaker;
    private Animator animator;

    private float currentHealth;
    private float damagedTime;
    private bool isInvincible = false;

    void Start()
    {
        hudController = Object.FindObjectOfType<HUDController>();
        shaker = Camera.main.GetComponent<CameraShake>();
        animator = GetComponentInParent<Animator>();

        currentHealth = InitialHealth;
        isInvincible = false;
        hudController.UpdateHealth(currentHealth);
    }

    void FixedUpdate()
    {
        // has the invincibility period ended?
        if (isInvincible && (Time.time - damagedTime) > InvincibilityDuration)
        {
            isInvincible = false;
            animator.SetBool("IsInvincible", false);
        }
    }

    public void TakeDamage(float damage, string damageSource)
    {
        // am I invincible?
        if (isInvincible)
            return;

        // send telemetry regarding what the player was damaged by
        var data = new Dictionary<string, object>()
        {
            { "DamageSource", damageSource }
        };
        Analytics.CustomEvent("PlayerDamaged", data);

        // shake camera
        if (shaker != null)
            shaker.RandomShake(damage * 0.5f);

        // reduce health
        currentHealth -= damage;
        UpdateHealthUI();

        // trigger temporary invincibility
        isInvincible = true;
        animator.SetBool("IsInvincible", true);
        damagedTime = Time.time;
    }

    public void ResetHealth()
    {
        currentHealth = InitialHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (hudController != null)
        {
            // update HUD
            hudController.UpdateHealth(currentHealth);

            // am I dead?
            if (currentHealth <= 0)
            {
                // game over
                hudController.UpdateHealth(0);
                hudController.GameOver();

                // remove player object
                if (Parent != null)
                    Destroy(Parent.gameObject);
            }
        }
    }
}
