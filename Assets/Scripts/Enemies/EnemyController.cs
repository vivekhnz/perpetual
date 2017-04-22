using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float CollisionDamage = 40.0f;
    
	[HideInInspector]
	public PlayerHealth Player;

	void Start() {
		Player = Object.FindObjectOfType<PlayerHealth>();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Player == null)
            return;

        // did I crash into the player?
        if (other.gameObject.Equals(Player.gameObject))
        {
            // damage player
            if (Player != null)
                Player.TakeDamage(CollisionDamage);

            // self-destruct
            Destroy(gameObject);
        }
    }
}
