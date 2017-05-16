using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public PlayerMovement Parent;
    public float InitialHealth;
    public float InvincibilityLength;

    private HUDController hudController;
    private CameraShake shaker;
    private float currentHealth;
    private float timeWhenDamaged;
    private Animator anim;

    void Start()
    {
        hudController = Object.FindObjectOfType<HUDController>();
        shaker = Camera.main.GetComponent<CameraShake>();
        anim = GetComponentInParent<Animator>();

        currentHealth = InitialHealth;
        hudController.UpdateHealth(currentHealth);
    }

    void FixedUpdate()
    {
        // stops invincible anim after set amount of time.
        if (anim.GetBool("isInvincible"))
        {
            if ((Time.time - timeWhenDamaged) > InvincibilityLength)
            {
                anim.SetBool("isInvincible", false);
            }
        }
    }

    public void TakeDamage(float damage, string damageSource)
    {
        // see if player is still invincible
        if ((Time.time - timeWhenDamaged) > InvincibilityLength)
        {
            // telemetry showing which enemy damaged player
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

            // the player is now invincible.
            anim.SetBool("isInvincible", true);

            // store time to address temp invincibility
            timeWhenDamaged = Time.time;
        }
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
