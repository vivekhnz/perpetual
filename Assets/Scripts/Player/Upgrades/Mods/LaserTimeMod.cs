using System;
using UnityEngine;

public class LaserTimeMod : WeaponModUpgrade<LaserWeapon> {

    public float LaserDuration = 2.0f;

    protected override void ApplyMod(LaserWeapon weapon)
    {
        weapon.LaserDuration = LaserDuration;
    }
}
