using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateMod : PlayerModUpgrade {

    public float BulletsPerMinute = 1500.0f;

    protected override void ApplyMod(GameObject player)
    {
        var weapon = GetPrimaryWeapon(player);
        weapon.BulletsPerMinute = BulletsPerMinute;
    }

}
