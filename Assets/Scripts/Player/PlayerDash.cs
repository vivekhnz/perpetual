using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    // force of the player's dash
    public float Speed = 0.25f;
    // how long the player cannot dash for in seconds
    public float Cooldown = 3;
    // multiplier to slow down a player's dash (must be < 1)
    public float Friction = 0.9f;
    // references to the cooldown timer HUD element
    public DashCooldownTimer DashCooldownTimerHUD;

    private Vector2 velocity;
    private float dashTime;

    void FixedUpdate()
    {
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
        velocity += dashDirection * Speed;

        // reset cooldown
        dashTime = Time.time;

        // reset the fill of the countdown circular slider
        DashCooldownTimerHUD.ResetCountdown();
    }
}