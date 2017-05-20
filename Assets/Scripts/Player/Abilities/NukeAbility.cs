using System;
using UnityEngine;

public class NukeAbility : PlayerAbility
{
    public float Cooldown = 12;

    private NukeController controller;
    private float nukeTime;

    void Start()
    {
        controller = GetComponent<NukeController>();
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Ability") && Time.time - nukeTime > Cooldown)
            Nuke();
    }

    private void Nuke()
    {
        controller.CreateShockwave();
        nukeTime = Time.time;
    }

    public override float GetCharge()
    {
        return Mathf.Clamp(
            (Time.time - nukeTime) / Cooldown, 0, 1);
    }
}
