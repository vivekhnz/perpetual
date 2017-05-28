using UnityEngine;

public abstract class PlayerAbilityBase : MonoBehaviour
{
    public Sprite Icon { get; private set; }

    public abstract float GetCharge();

    public void InjectUpgrade(UpgradeBase upgrade)
    {
        Icon = upgrade.Icon;
        ExtractAbilityInfo(upgrade);
    }

    public virtual void ExtractAbilityInfo(UpgradeBase upgrade)
    {
    }
}

public abstract class PlayerAbility<T> : PlayerAbilityBase
    where T : UpgradeBase
{
    public sealed override void ExtractAbilityInfo(UpgradeBase upgrade)
    {
        ExtractAbilityInfo(upgrade as T);
    }

    public virtual void ExtractAbilityInfo(T upgrade)
    {
    }
}