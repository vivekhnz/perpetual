using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeButtonController : MonoBehaviour
{
    [Serializable]
    public class UpgradeSelectedEvent : UnityEvent<UpgradeBase> { }

    public Text UpgradeNameText;
    public Text UpgradeTypeText;
    public Image UpgradeButtonIcon;
    public UpgradeSelectedEvent OnUpgradeSelected;

    private UpgradeBase upgrade;

    public void SetUpgrade(UpgradeBase upgrade)
    {
        UpgradeNameText.text = upgrade.Name;
        UpgradeTypeText.text = upgrade.Type.ToString();
        UpgradeButtonIcon.sprite = upgrade.Icon;
        this.upgrade = upgrade;
    }

    public void UpgradeSelected()
    {
        if (OnUpgradeSelected != null)
            OnUpgradeSelected.Invoke(upgrade);
    }
}
