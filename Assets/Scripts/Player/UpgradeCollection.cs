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
    Ability = 1
}

public abstract class UpgradeBase : ScriptableObject
{
    public string Name = "New Upgrade";
    public UpgradeType Type;
    public Sprite Icon;
}

public abstract class AbilityUpgradeBase : UpgradeBase
{
    public Type Component;

    public AbilityUpgradeBase()
    {
        Name = "New Ability";
        Type = UpgradeType.Ability;
    }
}

public abstract class AbilityUpgrade<T> : AbilityUpgradeBase
{
    public AbilityUpgrade()
    {
        Component = typeof(T);
    }
}

[Serializable]
public class WeaponUpgrade : UpgradeBase
{
    public PlayerSecondaryWeapon Prefab;

    public WeaponUpgrade()
    {
        Name = "New Weapon";
        Type = UpgradeType.Weapon;
    }
}