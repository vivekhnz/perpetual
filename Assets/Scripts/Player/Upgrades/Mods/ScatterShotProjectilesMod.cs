using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterShotProjectilesMod : WeaponModUpgrade<ScatterShotWeapon> {

    public int ProjectileAmount = 15;

    protected override void ApplyMod(ScatterShotWeapon weapon)
    {
        weapon.ProjectileAmount = ProjectileAmount;
    }
}
