using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class UpgradeTree
{
    private List<UpgradeBase> allUpgrades;
    private List<UpgradeBase> unlocked;

    public UpgradeTree(UpgradeCollection upgrades)
    {
        allUpgrades = upgrades.Upgrades;
        unlocked = new List<UpgradeBase>();
    }

    public void Unlock(UpgradeBase upgrade)
    {
        unlocked.Add(upgrade);
    }

    public List<UpgradeBase> GetAvailableUpgrades()
    {
        // only allow one upgrade of each type
        var equippedTypes = unlocked.Select(u => u.Type);
        return allUpgrades.Where(u => !equippedTypes.Contains(u.Type)).ToList();
    }
}