using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerEnemyHealth : MonoBehaviour {

    public float maxHealth;
    public SeekerEnemyMovement Parent;

    private float currentHealth;

	// Use this for initialization
	void Start () {
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

        if (currentHealth <= 0)
        {
            Debug.Log(damage);
            Destroy(Parent.gameObject);
        }
    }
}
