using System;
using System.Collections.Generic;
using System.Linq;

public class UpgradeTree
{
    private List<UpgradeBase> allUpgrades;
    private List<UpgradeBase> unlocked;

    private bool hasWeapon;
    private bool hasAbility;

    public UpgradeTree(UpgradeCollection upgrades)
    {
        allUpgrades = upgrades.Upgrades;
        unlocked = new List<UpgradeBase>();
        hasWeapon = false;
        hasAbility = false;
    }

    public void Unlock(UpgradeBase upgrade)
    {
        if (upgrade.Type == UpgradeType.Weapon)
        {
            // do we already have a weapon?
            if (hasWeapon)
                return;
            hasWeapon = true;
        }
        else if (upgrade.Type == UpgradeType.Ability)
        {
            // do we already have an ability?
            if (hasAbility)
                return;
            hasAbility = upgrade;
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
                return !hasWeapon;
            case UpgradeType.Ability:
                return !hasAbility;
            case UpgradeType.WeaponMod:
            case UpgradeType.AbilityMod:
            case UpgradeType.PlayerMod:
                var mod = upgrade as ModUpgradeBase;
                if (mod.Dependency == null)
                    return true;
                return unlocked.Contains(mod.Dependency);
        }

        return false;
    }
}