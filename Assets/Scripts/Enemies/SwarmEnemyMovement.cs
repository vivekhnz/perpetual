using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class SwarmEnemyMovement : MonoBehaviour
{
    public float MovementSpeed = 2.0f;
    public float DodgeSpeed = 3.0f;
    public float DodgeCooldown = 1.0f;
    public PushableObject Pushable;

    private EnemyController controller;
    private float dodgeTime;

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

        // move toward player
        Vector2 direction = controller.Player.transform.position - transform.position;
        direction.Normalize();
        transform.Translate(
            direction * MovementSpeed * Time.deltaTime,
            Space.World);

        // spin based on speed
        float speed = MovementSpeed + (Pushable.Velocity.magnitude * 1.5f);
        transform.Rotate(0.0f, 0.0f, 45.0f * speed * Time.deltaTime);
    }

    public void Dodge(Collider2D other)
    {
        // can I dodge again?
        if (Time.time - dodgeTime < DodgeCooldown)
            return;

        // dodge away from projectile
        Vector2 dir = other.transform.position - transform.position;
        dir.Normalize();
        var perpendicular = new Vector2(dir.y, -dir.x);

        // randomly decide whether to dodge left or right
        perpendicular *= Random.Range(0, 2) == 0 ? -1 : 1;
        Pushable.Push(perpendicular, DodgeSpeed);

        // reset cooldown
        dodgeTime = Time.time;
    }
}
