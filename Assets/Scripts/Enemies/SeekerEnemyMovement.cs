using UnityEngine;

public class SeekerEnemyMovement : MonoBehaviour
{
    public float MovementSpeed;
    public PlayerHealth Player;
    public float CollisionDamage = 40.0f;

    void FixedUpdate()
    {
        if (Player == null)
            return;

        // rotate to face player
        Vector2 direction = Player.transform.position - transform.position;
        direction.Normalize();
        transform.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        // move forward
        transform.Translate(
            Vector3.right * MovementSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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

    public void SetTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerHealth playerTarget = playerObject.GetComponentInChildren<PlayerHealth>();
            if (playerTarget != null)
            {
                Player = playerTarget;
            }
            else
            {
                Debug.Log("PlayerHealth not found");
            }
        }
        else
        {
            Debug.Log("Player not found");
        }
        Debug.Log("Player Set");
    }
}
