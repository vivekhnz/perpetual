using UnityEngine;

public class EnemyController : PooledObject
{
	public DamageableObject DamageableObject;
    public float CollisionDamage = 40.0f;

    public PlayerHealth Player { get; private set; }

    void Start()
    {
        Player = Object.FindObjectOfType<PlayerHealth>();

		if (DamageableObject == null)
			Debug.LogError("Enemy does not have a damageable object.");
    }

    public void Initialize(Vector3 position)
    {
        this.transform.position = position;
		DamageableObject.ResetHealth();
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
            DamageableObject.TakeDamage(float.MaxValue);
        }
    }
}
