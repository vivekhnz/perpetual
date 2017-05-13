using UnityEngine;

[RequireComponent(typeof(DataProvider))]
public class PlayerUpgrades : MonoBehaviour
{
    private DataProvider data;

    void Start()
    {
        data = GetComponent<DataProvider>();
        if (data == null)
            Debug.LogError("No data provider found!");
    }

    void FixedUpdate()
    {
        data.UpdateValue<float>("SecondaryWeaponCharge", Time.time % 1f);
    }
}
