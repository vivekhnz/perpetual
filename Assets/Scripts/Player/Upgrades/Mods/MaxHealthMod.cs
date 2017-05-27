using System;
using UnityEngine;

public class MaxHealthMod : PlayerModUpgrade {

    public float MaximumHealth = 150.0f;

    protected override void ApplyMod(GameObject player)
    {
        var health = GetHealth(player);
        health.InitialHealth = MaximumHealth;
    }

}
