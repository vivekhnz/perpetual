using System;
using UnityEngine;

public class HighCaliberRoundsMod : PlayerModUpgrade
{
    public float ProjectileKnockbackForce = 1.0f;

    protected override void ApplyMod(GameObject player)
    {
        var weapon = GetPrimaryWeapon(player);
        weapon.ProjectileKnockbackForce = ProjectileKnockbackForce;
    }
}