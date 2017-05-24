using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeButtonController : MonoBehaviour
{
    [Serializable]
    public class UpgradeSelectedEvent : UnityEvent<Upgrade> { }

    public Text UpgradeNameText;
    public Text UpgradeTypeText;
    public Image UpgradeButtonIcon;
    public UpgradeSelectedEvent OnUpgradeSelected;

    private Upgrade upgrade;

    public void SetUpgrade(Upgrade upgrade)
    {
        UpgradeNameText.text = upgrade.Name;
        UpgradeTypeText.text = upgrade.Type.ToString();
        this.upgrade = upgrade;
    }

    public void UpgradeSelected()
    {
        if (OnUpgradeSelected != null)
            OnUpgradeSelected.Invoke(upgrade);
    }
}
