using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCollection : ScriptableObject
{
    public List<UpgradeBase> Upgrades;
}

public enum UpgradeType
{
    Weapon = 0,
    Ability = 1,
    WeaponMod = 2,
    AbilityMod = 3,
    PlayerMod = 4
}

public abstract class UpgradeBase : ScriptableObject
{
    public string Name = "New Upgrade";
    public UpgradeType Type;
    public Sprite Icon;
}