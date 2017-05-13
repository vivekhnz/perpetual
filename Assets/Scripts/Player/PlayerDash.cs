using UnityEngine;

[RequireComponent(typeof(DataProvider))]
public class PlayerDash : MonoBehaviour
{
    // force of the player's dash
    public float Speed = 0.25f;
    // how long the player cannot dash for in seconds
    public float Cooldown = 3;
    // multiplier to slow down a player's dash (must be < 1)
    public float Friction = 0.9f;

    private DataProvider data;

    private Vector2 velocity;
    private float dashTime;

    void Start()
    {
        data = GetComponent<DataProvider>();
        if (data == null)
            Debug.LogError("No data provider found!");
    }

    void FixedUpdate()
    {
        // update ability charge
        float abilityCharge = Mathf.Clamp(
            (Time.time - dashTime) / Cooldown, 0, 1);
        data.UpdateValue<float>("AbilityCharge", abilityCharge);

        // can I dash?
        if (Input.GetButton("Ability")
            && Time.time - dashTime > Cooldown)
            Dash();

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

        transform.Translate(
            velocity.x, velocity.y, 0, Space.World);
    }

    private void Dash()
    {
        // apply dash velocity
        var dashDirection = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")).normalized;

        // are we actually moving?
        if (dashDirection.magnitude < 0.001f)
            return;

        velocity += dashDirection * Speed;

        // reset cooldown
        dashTime = Time.time;
    }
}