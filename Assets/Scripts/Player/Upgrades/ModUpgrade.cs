using System;
using UnityEngine;

public abstract class ModUpgradeBase : UpgradeBase
{
    public UpgradeBase Dependency;

    public ModUpgradeBase(UpgradeType type)
    {
        Name = "New Mod";
        Type = type;
    }

    public abstract void ApplyMod(object target);
}

public abstract class WeaponModUpgrade<T> : ModUpgradeBase
    where T : MonoBehaviour
{
    public WeaponModUpgrade()
        : base(UpgradeType.WeaponMod)
    {
    }

    public sealed override void ApplyMod(object target)
    {
        var weapon = target as PlayerSecondaryWeapon;
        ApplyMod(weapon.GetComponent<T>());
    }

    protected abstract void ApplyMod(T weapon);
}

public abstract class AbilityModUpgrade<T> : ModUpgradeBase
    where T : PlayerAbilityBase
{
    public AbilityModUpgrade()
        : base(UpgradeType.AbilityMod)
    {
    }

    public sealed override void ApplyMod(object target)
    {
        ApplyMod(target as T);
    }

    protected abstract void ApplyMod(T ability);
}

public abstract class PlayerModUpgrade : ModUpgradeBase
{
    public PlayerModUpgrade()
        : base(UpgradeType.PlayerMod)
    {
    }

    protected PlayerMovement GetMovement(GameObject player)
    {
        return player.GetComponent<PlayerMovement>();
    }

    protected PlayerUpgrades GetUpgrades(GameObject player)
    {
        return player.GetComponent<PlayerUpgrades>();
    }

    protected PlayerHealth GetHealth(GameObject player)
    {
        return player.GetComponentInChildren<PlayerHealth>();
    }

    protected PrimaryWeapon GetPrimaryWeapon(GameObject player)
    {
        return player.GetComponentInChildren<PrimaryWeapon>();
    }

    public sealed override void ApplyMod(object target)
    {
        ApplyMod(target as GameObject);
    }

    protected abstract void ApplyMod(GameObject player);
}