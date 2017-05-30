using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public PlayerMovement Parent;
    public float InitialHealth;
    public float InvincibilityDuration = 1.0f;
    public AudioSource takeDamageAudio;

    private HUDController hudController;
    private CameraShake shaker;
    private DamageEffects damageEffects;
    private Animator animator;
    private DataProvider data;

    private float currentHealth;
    private float damagedTime;
    private bool isInvincible = false;

    void Start()
    {
        hudController = Object.FindObjectOfType<HUDController>();
        shaker = Camera.main.GetComponent<CameraShake>();
        damageEffects = Camera.main.GetComponent<DamageEffects>();
        animator = GetComponentInParent<Animator>();
        data = GetComponentInParent<DataProvider>();

        currentHealth = InitialHealth;
        isInvincible = false;
    }

    void FixedUpdate()
    {
        // has the invincibility period ended?
        if (isInvincible && (Time.time - damagedTime) > InvincibilityDuration)
        {
            isInvincible = false;
            animator.SetBool("IsInvincible", false);
        }

        // update UI
        data.UpdateValue<float>("MaxHealth", InitialHealth);
        data.UpdateValue<float>("CurrentHealth", currentHealth);

        // am I dead?
        if (hudController != null && currentHealth <= 0)
        {
            // game over
            hudController.GameOver();

            // remove player object
            if (Parent != null)
                Destroy(Parent.gameObject);
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

        // activate damage effects
        if (shaker != null)
            shaker.RandomShake(damage * 0.25f);
        if (damageEffects != null)
            damageEffects.Activate();

        // trigger temporary invincibility
        isInvincible = true;
        animator.SetBool("IsInvincible", true);
        damagedTime = Time.time;

        // play damage audio
        takeDamageAudio.Play();

        // reduce health
        currentHealth -= damage;
    }

    public void GainHealth(float healthGained)
    {
        // regain health without exceeding max hp
        if ((currentHealth + healthGained) >= InitialHealth)
        {
            currentHealth = InitialHealth;
        }
        else
        {
            currentHealth += healthGained;
        }

        UpdateHealthUI();
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
