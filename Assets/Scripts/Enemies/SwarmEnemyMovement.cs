using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class SwarmEnemyMovement : MonoBehaviour
{
    public float MovementSpeed = 2.0f;

    private bool isDodging;
    private EnemyController controller;

    void Start()
    {
        isDodging = false;
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

        // If a projectile has been detected, commence evasive manuevers!
        if (isDodging)
        {
            transform.Translate(
                Vector3.up * MovementSpeed * Time.deltaTime);
        }
        else
        {
            // move forward
            transform.Translate(
                Vector3.right * MovementSpeed * Time.deltaTime);
        }
    }

    public void setIsDodging(bool isDodging)
    {
        this.isDodging = isDodging;
    }
}
