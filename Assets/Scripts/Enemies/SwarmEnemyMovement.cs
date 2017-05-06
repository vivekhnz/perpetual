using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class SwarmEnemyMovement : MonoBehaviour
{
    public float MovementSpeed = 2.0f;
    public float DodgeSpeed = 3.0f;
    public PushableObject Pushable;

    private EnemyController controller;
    private Vector2 velocity;

    void Start()
    {
        if (Pushable == null)
            Debug.LogError("No pushable object specified!");

        controller = GetComponent<EnemyController>();
    }

    void FixedUpdate()
    {
        if (controller.Player == null)
            return;

        // rotate to face player
        Vector2 direction = controller.Player.transform.position - transform.position;
        direction.Normalize();
        transform.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        // move forward
        transform.Translate(
            Vector3.right * MovementSpeed * Time.deltaTime);
    }

    public void Dodge(Collider2D other)
    {
        // dodge away from projectile
        Vector2 dir = other.transform.position - transform.position;
        dir.Normalize();
        var perpendicular = new Vector2(dir.y, -dir.x);

        // randomly decide whether to dodge left or right
        perpendicular *= Random.Range(0, 2) == 0 ? -1 : 1;

        Pushable.Push(perpendicular, DodgeSpeed);
    }
}
