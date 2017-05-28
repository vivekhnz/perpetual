using System;

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