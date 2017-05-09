using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCooldownTimer : MonoBehaviour {

    // The circular image that acts as a slider for the dash ability's cooldown.
    public Image DashCooldownCircularSliderHUD;
    // Takes the player gameObject to find dash ability's countdown.
    public PlayerMovement playerMovement;

    // The time it takes to fill the circular bar completely. 
    private float cooldownTimer;

    void Start()
    {
        // Obtain the dash ability's countdown timer from the GameObject to stay consistent.
        cooldownTimer = playerMovement.DashCooldownTimer;
    }

    void Update()
    {
        // The the bar gradually as determined by the cooldown timer.
        DashCooldownCircularSliderHUD.fillAmount += Time.deltaTime / cooldownTimer;
    }

    // Resets the countdown and empties the circular slider.
    public void ResetCountdown()
    {
        DashCooldownCircularSliderHUD.fillAmount = 0f;
    }
}
