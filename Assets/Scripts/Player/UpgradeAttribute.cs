using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class UpgradeAttribute : Attribute
{
    private readonly string name;
    private readonly UpgradeType type;

    public string Name
    {
        get { return name; }
    }
    public UpgradeType Type
    {
        get { return type; }
    }

    public UpgradeAttribute(string name, UpgradeType type)
    {
        this.name = name;
        this.type = type;
    }
}

public enum UpgradeType
{
    Weapon = 0,
    Ability = 1
}