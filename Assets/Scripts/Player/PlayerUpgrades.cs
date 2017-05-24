using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

[RequireComponent(typeof(DataProvider))]
public class PlayerUpgrades : MonoBehaviour
{
    public List<PlayerSecondaryWeapon> SecondaryWeapons;
    public UpgradeCollection Upgrades;

    private DataProvider data;
    private PlayerWeapon[] weapons;
    private PlayerSecondaryWeapon secondaryWeapon;
    private PlayerAbilityBase ability;
    private UpgradeTree tree;

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
        }
    }

    public void Unlock(UpgradeBase upgrade)
    {
        switch (upgrade.Type)
        {
            case UpgradeType.Weapon:
                UnlockWeapon(upgrade.Component);
                break;
            case UpgradeType.Ability:
                UnlockAbility(upgrade);
                break;
        }
        tree.Unlock(upgrade);
    }

    private void UnlockWeapon(Type weaponType)
    {
        foreach (var prefab in SecondaryWeapons)
        {
            if (prefab.GetComponent(weaponType) != null)
            {
                // remove any existing secondary weapons
                if (secondaryWeapon != null)
                    Destroy(secondaryWeapon.gameObject);

                // instantiate the weapon and attach it to the player
                var weapon = Instantiate(prefab, gameObject.transform.position,
                    gameObject.transform.rotation);
                weapon.transform.parent = gameObject.transform;
                secondaryWeapon = weapon;
                break;
            }
        }
    }

    private void UnlockAbility(UpgradeBase upgrade)
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
