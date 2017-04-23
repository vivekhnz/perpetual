using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {

    public PlayerMovement Parent;
    public float InitialHealth;

    private HUDController hudController;
    private float currentHealth;

	// Use this for initialization
	void Start () {
		hudController = Object.FindObjectOfType<HUDController>();
        currentHealth = InitialHealth;
        hudController.UpdateHealth(currentHealth);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(float damage)
    {
        // reduce health
        currentHealth -= damage;
        
        if (hudController != null) {
            // update HUD
            hudController.UpdateHealth(currentHealth);

            // am I dead?
            if (currentHealth <= 0) {
                // game over
                hudController.UpdateHealth(0);
                hudController.GameOver();
                Invoke("ReturnToStartMenu", 3);

                // remove player object
                if (Parent != null)
                    Destroy(Parent.gameObject);
            }
        }
    }

    private void ReturnToStartMenu()
    {
        //SceneManager.LoadScene(0);
        Debug.Log("Reload");
    }
}
