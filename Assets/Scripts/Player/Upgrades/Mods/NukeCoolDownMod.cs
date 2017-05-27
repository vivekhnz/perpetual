using System;
using UnityEngine;

public class NukeCoolDownMod : AbilityModUpgrade<NukeAbility> {

    public float Cooldown = 9.0f;

    protected override void ApplyMod(NukeAbility ability)
    {
        ability.Cooldown = Cooldown;
    }

}
