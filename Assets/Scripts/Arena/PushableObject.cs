using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public float Friction = 0.9f;
    public GameObject Parent;

    private Vector2 velocity;
    public Vector2 Velocity { get { return velocity; } }

    void Start()
    {
        Parent = Parent ?? gameObject;
    }

    void FixedUpdate()
    {
        // move based on velocity
        Vector2 movement = velocity * Time.deltaTime;
        Parent.transform.Translate(
            movement.x, movement.y, 0, Space.World);

        if (velocity.magnitude < 0.01f)
        {
            // stop moving once we get really slow
            velocity = Vector2.zero;
        }
        else
        {
            // slow down over time
            velocity *= Friction;
        }
    }

    public void Push(Vector2 direction, float force)
    {
        velocity += direction * force;
    }
}
