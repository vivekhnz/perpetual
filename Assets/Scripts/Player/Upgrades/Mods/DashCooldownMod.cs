using System;
using UnityEngine;

public class DashCooldownMod : AbilityModUpgrade<DashAbility> {

    public float Cooldown = 2.0f;

    protected override void ApplyMod(DashAbility ability)
    {
        ability.Cooldown = Cooldown;
    }

}
