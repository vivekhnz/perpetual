using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    public float InitialHealth = 100;
    public float ScoreValue = 5;

    private float currentHealth;
    private HUDController hudController;

    void Start()
    {
        hudController = Object.FindObjectOfType<HUDController>();
        currentHealth = InitialHealth;

        if (!gameObject.CompareTag("Damageable"))
            Debug.LogWarning("This object does not have the 'Damageable' tag. Objects may be unable to damage it.");
    }

    public void TakeDamage(float damage)
    {
        // reduce health
        currentHealth -= damage;

        // am I dead?
        if (currentHealth <= 0)
        {
            // increase score
            if (hudController != null)
                hudController.AddScore(ScoreValue);

            // self-destruct
            Destroy(gameObject);
        }
    }
}
