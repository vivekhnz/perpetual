using System;
using UnityEngine;

public class LaserCooldownMod : WeaponModUpgrade<LaserWeapon> {

    public float Cooldown = 2.0f;

    protected override void ApplyMod(LaserWeapon ability)
    {
        ability.Cooldown = Cooldown;
    }
}
