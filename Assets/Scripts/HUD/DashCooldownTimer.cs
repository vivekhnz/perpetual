using UnityEngine;
using UnityEngine.UI;

public class DashCooldownTimer : MonoBehaviour
{
    // The circular image that acts as a slider for the dash ability's cooldown.
    public Image DashCooldownCircularSliderHUD;
    // Takes the player gameObject to find dash ability's cooldown.
    public PlayerDash playerDash;

    // The time it takes to fill the circular bar completely. 
    private float cooldownTimer;

    void Start()
    {
        // Obtain the dash ability's cooldown timer from the GameObject to stay consistent.
        cooldownTimer = playerDash.Cooldown;
    }

    void Update()
    {
        // The bar gradually fills as determined by the cooldown timer.
        DashCooldownCircularSliderHUD.fillAmount +=
            Time.deltaTime / cooldownTimer;
    }

    // Resets the countdown and empties the circular slider.
    public void ResetCountdown()
    {
        DashCooldownCircularSliderHUD.fillAmount = 0f;
    }
}
