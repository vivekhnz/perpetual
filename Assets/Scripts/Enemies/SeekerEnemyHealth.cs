using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerEnemyHealth : MonoBehaviour {

    public float maxHealth;
    public SeekerEnemyMovement Parent;
    public float ScoreValue;

    private float currentHealth;
    private HUDController hudController;

	// Use this for initialization
	void Start () {
        hudController = Object.FindObjectOfType<HUDController>();
        currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Projectile"))
        {
            ProjectileController projectile = collider.GetComponent<ProjectileController>();
            if (projectile != null)
            {
                Debug.Log("DAMAGE");
                TakeDamage(projectile.Damage);
            }
            Destroy(collider.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        if (Parent == null)
        {
            return;
        }

        currentHealth -= damage;
        hudController.AddScore(ScoreValue);

        if (currentHealth <= 0)
        {
            Destroy(Parent.gameObject);
        }
    }
}
