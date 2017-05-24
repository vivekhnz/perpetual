using System;
using System.Collections.Generic;
using UnityEngine;

using UnityObject = UnityEngine.Object;

public abstract class PlayerAbility : MonoBehaviour
{
    public Sprite Icon { get; private set; }

    public abstract float GetCharge();

    void Start()
    {
        var upgrades = GetComponent<PlayerUpgrades>();
        if (upgrades == null)
            Debug.LogError("Upgrades component not found!");

        // retrieve ability info
        string abilityName = GetType().Name;
        AbilityInfo abilityInfo = null;
        foreach (var info in upgrades.Abilities)
        {
            if (info.AbilityName.Equals(abilityName))
            {
                abilityInfo = info;
                break;
            }
        }
        if (abilityInfo == null)
        {
            Debug.LogError("No ability info found!");
        }
        else
        {
            ExtractAbilityInfo(abilityInfo);
        }
    }

    public virtual void ExtractAbilityInfo(AbilityInfo info)
    {
        Icon = info.Icon;
    }
}

[Serializable]
public class AbilityInfo
{
    [Serializable]
    public class AbilityProp
    {
        public string Name;
        public UnityObject Value;
    }

    public string AbilityName;
    public Sprite Icon;
    public List<AbilityProp> Properties;

    public UnityObject GetObject(string name)
    {
        foreach (var prop in Properties)
        {
            if (prop.Name.Equals(name))
                return prop.Value;
        }

        Debug.LogError($"Property '{name}' not found!");
        return null;
    }

    public T GetComponent<T>(string name)
    {
        var go = GetObject(name) as GameObject;
        return go.GetComponent<T>();
    }
}