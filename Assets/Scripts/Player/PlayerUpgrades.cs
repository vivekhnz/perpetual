using UnityEngine;
using System.Linq;

[RequireComponent(typeof(DataProvider))]
public class PlayerUpgrades : MonoBehaviour
{
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
}
