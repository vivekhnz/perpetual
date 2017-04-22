using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    public float InitialHealth = 100;
    public int ScoreValue = 5;
    public GameObject Parent;

    private float currentHealth;
    private HUDController hudController;

    void Start()
    {
        Parent = Parent ?? gameObject;

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

            // recycle the object if it is poolable, destroy it otherwise
            var poolable = Parent.GetComponent<PooledObject>();
            if (poolable == null)
            {
                Destroy(Parent);
            }
            else
            {
                poolable.Recycle();
            }
        }
    }

    public void ResetHealth()
    {
        currentHealth = InitialHealth;
    }
}
