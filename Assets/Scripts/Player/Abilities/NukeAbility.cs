using System;
using UnityEngine;

public class NukeAbility : PlayerAbility {

    public float Cooldown = 3;
    public NukeController NukeController;

    private float nukeTime;

    void Start()
    {
        NukeController = GetComponent<NukeController>();
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Ability") && Time.time - nukeTime > Cooldown)
        {
            Nuke();
        }
    }

    private void Nuke()
    {
        NukeController.CreateShockwave();

        nukeTime = Time.time;
    }

    public override float GetCharge()
    {
        return Mathf.Clamp(
            (Time.time - nukeTime) / Cooldown, 0, 1);
    }
}
