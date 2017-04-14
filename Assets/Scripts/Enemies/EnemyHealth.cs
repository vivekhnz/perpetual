using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float InitialHealth = 100;
    public float ScoreValue = 5;

    private float currentHealth;
    private HUDController hudController;

    void Start()
    {
        hudController = Object.FindObjectOfType<HUDController>();
        currentHealth = InitialHealth;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Projectile"))
        {
            ProjectileController projectile =
                collider.GetComponent<ProjectileController>();
            if (projectile != null)
                TakeDamage(projectile.Damage);

            Destroy(collider.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (hudController != null)
                hudController.AddScore(ScoreValue);

            Destroy(gameObject);
        }
    }
}
