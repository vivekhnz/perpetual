using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(DataProvider))]
public class PlayerUpgrades : MonoBehaviour
{
    public List<PlayerSecondaryWeapon> SecondaryWeapons;

    private DataProvider data;
    private PlayerWeapon[] weapons;
    private PlayerSecondaryWeapon secondaryWeapon;
    private PlayerAbility ability;

    public bool IsFiring
    {
        get { return weapons.Any(w => w.IsFiring); }
    }

    void Start()
    {
        data = GetComponent<DataProvider>();
        if (data == null)
            Debug.LogError("No data provider found!");

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
        ability = GetComponent<PlayerAbility>();
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

    public void UnlockWeapon<T>()
        where T : MonoBehaviour
    {
        foreach (var prefab in SecondaryWeapons)
        {
            if (prefab.GetComponent<T>() != null)
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

    public bool HasWeapon<T>()
        where T : MonoBehaviour
    {
        return secondaryWeapon != null
            && secondaryWeapon.GetComponent<T>() != null;
    }

    public void UnlockAbility<T>()
        where T : PlayerAbility
    {
        // remove any existing abilities
        if (ability != null)
            Destroy(ability);

        // attach the new ability and retrieve its icon
        ability = gameObject.AddComponent<T>();
        ability.Icon = Resources.Load<Sprite>(
            $"Abilities/{typeof(T).Name}Icon");
    }

    public bool HasAbility<T>()
        where T : PlayerAbility
    {
        return ability != null && ability is T;
    }
}
