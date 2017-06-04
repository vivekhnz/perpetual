using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

[RequireComponent(typeof(DataProvider))]
public class PlayerUpgrades : MonoBehaviour
{
    public UpgradeCollection Upgrades;
    public AudioClip WeaponOffCooldownSFX;
    public AudioClip AbilityOffCooldownSFX;

    private DataProvider data;
    private PlayerWeapon[] weapons;
    private PlayerSecondaryWeapon secondaryWeapon;
    private PlayerAbilityBase ability;
    private UpgradeTree tree;
    private bool weaponOffCooldownSFXPlayed = false;
    private bool abilityOffCooldownSFXPlayed = false;

    public bool IsFiring
    {
        get { return weapons.Any(w => w.IsFiring); }
    }

    void Start()
    {
        data = GetComponent<DataProvider>();
        if (data == null)
            Debug.LogError("No data provider found!");

        tree = new UpgradeTree(Upgrades);

        // retrieve all weapons
        weapons = GetComponentsInChildren<PlayerWeapon>();

        // find secondary weapon
        foreach (var weapon in weapons)
        {
            var secondary = weapon.GetComponent<PlayerSecondaryWeapon>();
            if (secondary != null)
            {
                secondaryWeapon = secondary;
                break;
            }
        }

        // find current ability
        ability = GetComponent<PlayerAbilityBase>();
    }

    void FixedUpdate()
    {
        // update secondary weapon
        if (secondaryWeapon == null)
        {
            data.UpdateValue<bool>("HasSecondaryWeapon", false);
        }
        else
        {
            data.UpdateValue<float>("SecondaryWeaponCharge", secondaryWeapon.Charge);
            data.UpdateValue<Sprite>("SecondaryWeaponIcon", secondaryWeapon.Icon);
            data.UpdateValue<bool>("HasSecondaryWeapon", true);

            // play a sound when weapon ability is off cooldown
            if (secondaryWeapon.Charge == 1 && !weaponOffCooldownSFXPlayed)
            {
                AudioSource.PlayClipAtPoint(WeaponOffCooldownSFX, transform.position);
                weaponOffCooldownSFXPlayed = true;
            }
            else if (secondaryWeapon.Charge < 1 && weaponOffCooldownSFXPlayed)
            {
                weaponOffCooldownSFXPlayed = false;
            }
        }

        // update ability
        if (ability == null)
        {
            data.UpdateValue<bool>("HasAbility", false);
        }
        else
        {
            data.UpdateValue<float>("AbilityCharge", ability.GetCharge());
            data.UpdateValue<Sprite>("AbilityIcon", ability.Icon);
            data.UpdateValue<bool>("HasAbility", true);

            // play a sound when player ability is off cooldown
            if (ability.GetCharge() == 1 && !abilityOffCooldownSFXPlayed)
            {
                AudioSource.PlayClipAtPoint(AbilityOffCooldownSFX, transform.position);
                abilityOffCooldownSFXPlayed = true;
            }
            else if (ability.GetCharge() < 1 && abilityOffCooldownSFXPlayed)
            {
                abilityOffCooldownSFXPlayed = false;
            }
        }
    }

    public void Unlock(UpgradeBase upgrade)
    {
        switch (upgrade.Type)
        {
            case UpgradeType.Weapon:
                UnlockWeapon(upgrade as WeaponUpgrade);
                break;
            case UpgradeType.Ability:
                UnlockAbility(upgrade as AbilityUpgradeBase);
                break;
            case UpgradeType.WeaponMod:
                (upgrade as ModUpgradeBase).ApplyMod(secondaryWeapon);
                break;
            case UpgradeType.AbilityMod:
                (upgrade as ModUpgradeBase).ApplyMod(ability);
                break;
            case UpgradeType.PlayerMod:
                (upgrade as ModUpgradeBase).ApplyMod(gameObject);
                break;
        }
        tree.Unlock(upgrade);
    }

    private void UnlockWeapon(WeaponUpgrade upgrade)
    {
        // remove any existing secondary weapons
        if (secondaryWeapon != null)
            Destroy(secondaryWeapon.gameObject);

        // instantiate the weapon and attach it to the player
        secondaryWeapon = Instantiate(upgrade.Prefab, gameObject.transform.position,
            gameObject.transform.rotation);
        secondaryWeapon.transform.parent = gameObject.transform;
    }

    private void UnlockAbility(AbilityUpgradeBase upgrade)
    {
        // remove any existing abilities
        if (ability != null)
            Destroy(ability);

        // attach the new ability
        ability = gameObject.AddComponent(upgrade.Component) as PlayerAbilityBase;
        ability.InjectUpgrade(upgrade);
    }

    public List<UpgradeBase> GetUpgradeChoices(int maximum)
    {
        var available = tree.GetAvailableUpgrades();
        if (available.Count > maximum)
        {
            var choices = new List<UpgradeBase>();
            while (choices.Count < maximum)
            {
                var upgrade = available[Random.Range(0, available.Count)];
                choices.Add(upgrade);
                available.Remove(upgrade);
            }
            return choices;
        }
        else
        {
            return available;
        }
    }
}
