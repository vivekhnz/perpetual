using System;
using System.Collections.Generic;
using System.Linq;

public class UpgradeTree
{
    private List<UpgradeBase> allUpgrades;
    private List<UpgradeBase> unlocked;

    private UpgradeBase weapon;
    private UpgradeBase ability;

    public UpgradeTree(UpgradeCollection upgrades)
    {
        allUpgrades = upgrades.Upgrades;
        unlocked = new List<UpgradeBase>();
        weapon = null;
        ability = null;
    }

    public void Unlock(UpgradeBase upgrade)
    {
        if (upgrade.Type == UpgradeType.Weapon)
        {
            // do we already have a weapon?
            if (weapon != null)
                return;
            weapon = upgrade;
        }
        else if (upgrade.Type == UpgradeType.Ability)
        {
            // do we already have an ability?
            if (ability != null)
                return;
            ability = upgrade;
        }

        unlocked.Add(upgrade);
    }

    public List<UpgradeBase> GetAvailableUpgrades()
    {
        return allUpgrades.Where(IsUpgradeAvailable).ToList();
    }

    private bool IsUpgradeAvailable(UpgradeBase upgrade)
    {
        if (unlocked.Contains(upgrade))
            return false;

        switch (upgrade.Type)
        {
            case UpgradeType.Weapon:
                return weapon == null;
            case UpgradeType.Ability:
                return ability == null;
            case UpgradeType.WeaponMod:
                return (upgrade as ModUpgradeBase).Dependency.Equals(weapon);
            case UpgradeType.AbilityMod:
                return (upgrade as ModUpgradeBase).Dependency.Equals(ability);
            case UpgradeType.PlayerMod:
                return true;
        }

        return false;
    }
}