using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class ChasePlayer : MonoBehaviour
{
    public float MovementSpeed = 2.0f;

    private EnemyController controller;

    void Start()
    {
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
}
