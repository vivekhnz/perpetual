using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public PlayerMovement Parent;
    public float InitialHealth;

    private HUDController hudController;
    private float currentHealth;

    void Start()
    {
        hudController = Object.FindObjectOfType<HUDController>();
        currentHealth = InitialHealth;
        hudController.UpdateHealth(currentHealth);
    }

    public void TakeDamage(float damage, string damageSource)
    {
        var data = new Dictionary<string, object>()
        {
            { "DamageSource", damageSource }
        };
        Analytics.CustomEvent("PlayerDamaged", data);

        // reduce health
        currentHealth -= damage;
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
