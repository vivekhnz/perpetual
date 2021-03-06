using UnityEngine;

[RequireComponent(typeof(PlayerUpgrades))]
public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed = 4.0f;
    public float MovementSpeedWhileFiring = 2.5f;

    private PlayerUpgrades upgrades;

    void Start()
    {
        upgrades = GetComponent<PlayerUpgrades>();
        if (upgrades == null)
            Debug.LogError("Upgrades component not found!");
    }

    void FixedUpdate()
    {
        // calculate world position of mouse cursor
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.forward, Vector3.zero);
        float distance = 0.0f;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 mousePos = ray.GetPoint(distance);

            // calculate rotation based on direction to mouse cursor
            Vector3 dirToMouse = mousePos - transform.position;
            float rotation = Mathf.Atan2(dirToMouse.y, dirToMouse.x)
                * Mathf.Rad2Deg;

            // set the player rotation
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
        }

        // move player based on current speed
        float speed = upgrades.IsFiring
            ? MovementSpeedWhileFiring
            : MovementSpeed;
        Vector2 movement = new Vector2(
            Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))
            * Time.deltaTime * speed;
        transform.Translate(
            movement.x, movement.y, 0, Space.World);
    }
}
