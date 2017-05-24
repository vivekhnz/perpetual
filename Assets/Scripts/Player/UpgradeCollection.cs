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
    public Type Component;
}

public abstract class Upgrade<T> : UpgradeBase
{
    public Upgrade()
    {
        Component = typeof(T);
    }
}