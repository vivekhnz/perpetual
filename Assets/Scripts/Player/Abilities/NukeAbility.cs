﻿using System;
using UnityEngine;

public class NukeAbility : PlayerAbility<NukeAbilityUpgrade>
{
    public float Cooldown = 12;
    public float Range = 1.5f;

    private float nukeTime;
    private NukeController NukeShockwave;

    public override void ExtractAbilityInfo(NukeAbilityUpgrade upgrade)
    {
        NukeShockwave = upgrade.NukeShockwave;
    }

    void Update()
    {
        if (Input.GetButton("Ability") && Time.time - nukeTime > Cooldown)
            Nuke();
    }

    private void Nuke()
    {
        var shockwave = NukeShockwave.Fetch<NukeController>();
        shockwave.Range = Range;
        shockwave.transform.position = transform.position;
        nukeTime = Time.time;
    }

    public override float GetCharge()
    {
        return Mathf.Clamp(
            (Time.time - nukeTime) / Cooldown, 0, 1);
    }
}
