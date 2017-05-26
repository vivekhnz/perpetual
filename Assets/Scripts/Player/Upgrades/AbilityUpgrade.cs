using System;

public abstract class AbilityUpgradeBase : UpgradeBase
{
    public Type Component;

    public AbilityUpgradeBase(Type component)
    {
        Name = "New Ability";
        Type = UpgradeType.Ability;
        Component = component;
    }
}

public abstract class AbilityUpgrade<T> : AbilityUpgradeBase
{
    public AbilityUpgrade()
        : base(typeof(T))
    {
    }
}