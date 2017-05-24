using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class UpgradeTree
{
    readonly static List<Upgrade> Upgrades;

    static UpgradeTree()
    {
        // retrieve all upgrade types
        var upgrades =
            from type in Assembly.GetExecutingAssembly().GetTypes()
            let attributes = type.GetCustomAttributes(typeof(UpgradeAttribute), true)
            where attributes != null && attributes.Length > 0
            let attribute = attributes[0] as UpgradeAttribute
            select new Upgrade
            {
                Name = attribute.Name,
                Type = attribute.Type,
                Component = type
            };
        Upgrades = upgrades.ToList();
    }

    private List<Upgrade> unlocked = new List<Upgrade>();

    public void Unlock(Upgrade upgrade)
    {
        unlocked.Add(upgrade);
    }

    public List<Upgrade> GetAvailableUpgrades()
    {
        // only allow one upgrade of each type
        var equippedTypes = unlocked.Select(u => u.Type);
        return Upgrades.Where(u => !equippedTypes.Contains(u.Type)).ToList();
    }
}

public class Upgrade
{
    public string Name { get; set; }
    public UpgradeType Type { get; set; }
    public Type Component { get; set; }
}