using System;

public class LaserOverpenetrationMod : WeaponModUpgrade<LaserWeapon>
{
    public int MaxDamageablesToHit = 3;

    protected override void ApplyMod(LaserWeapon weapon)
    {
        weapon.MaxDamageablesToHit = MaxDamageablesToHit;
    }
}
