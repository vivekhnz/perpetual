using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterShotCooldown : WeaponModUpgrade<ScatterShotWeapon> {

    public float Cooldown = 0.5f;

    protected override void ApplyMod(ScatterShotWeapon weapon)
    {
        weapon.Cooldown = Cooldown;
    }
}
