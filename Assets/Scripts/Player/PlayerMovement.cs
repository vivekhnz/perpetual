using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    // Player's speed multiplier.
	public float MovementSpeed = 1.0f;
    // Multiplier, ie Friction to slow down a player's dash. MUST BE < 1!
    public float Friction = 0.9f;
    // The force of the player's dash.
    public float DashSpeed = 5;
    // How long the player cannot dash for in seconds.
    public float DashCooldownTimer = 3;
    // References the DashCooldownTimer (image) on the HUD.
    public DashCooldownTimer DashCooldownTimerHUD;

    // Determines the dash vector of the player's velocity. Stored here to eliminate getting a new Vector every update.
    private Vector2 dashVector;
    // Stores when the dash was used to calculate cooldown.
    private float timeOfDash = -4;

    // FixedUpdate is called once every set interval, 0.02s by default iirc.
    void FixedUpdate () {

        // Determines the walking vector component of the player's velocity.
        Vector2 walkVector;

        // Stores the x and y components using the WASD keys.
        walkVector.x = Input.GetAxis("Horizontal");
        walkVector.y = Input.GetAxis("Vertical");

        // Calculates the dash vector component of the player's velocity if dash ability is off cooldown.
        if (!IsDashStillOnCooldown() && Input.GetKeyDown("space"))
        {
            dashVector += walkVector * DashSpeed;
            timeOfDash = Time.time;

            // Reset the fill of the countdown circular slider.
            DashCooldownTimerHUD.ResetCountdown();
        }

        // Calculate the player's final vector which is a sum of the walk and dash vectors.
        Vector2 velocity = dashVector + walkVector;

        // Calculate the player's movement vector after frame independence and a speed multiplier.
        Vector2 movement = velocity * Time.deltaTime * MovementSpeed;

        if (dashVector.magnitude < 0.01f)
        {
            // stop moving once we get really slow
            dashVector = Vector2.zero;
        }
        else
        {
            // slow down over time
            dashVector *= Friction;
        }

        // Finally, update the player's position with new coordinates.
		transform.Translate(
			movement.x, movement.y, 0, Space.World);
	}

    // Returns true if the dash ability is still on cooldown.
    private bool IsDashStillOnCooldown()
    {
        return (Time.time - timeOfDash) < DashCooldownTimer;
    }
}
