using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    public PlayerMovement Parent;
    public float maxHealth;
    public float enemyDamage;

    private HUDController hudController;
    private float currentHealth;

	// Use this for initialization
	void Start () {
		hudController = Object.FindObjectOfType<HUDController>();
        currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Parent == null)
            return;
        
        if (other.gameObject.tag == "Enemy")
        {
            Destroy(other.gameObject);
            currentHealth -= enemyDamage;

            if (hudController != null && currentHealth <= 0)
            {
                Destroy(Parent.gameObject);
                hudController.GameOver();
            }
        }
    }
    
}
