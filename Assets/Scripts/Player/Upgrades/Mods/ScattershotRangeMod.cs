using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotRangeMod : WeaponModUpgrade<ScatterShotWeapon> {

    public float ProjectileRange = 1.0f;

    protected override void ApplyMod(ScatterShotWeapon weapon)
    {
        weapon.ProjectileRange = ProjectileRange;
    }

}
