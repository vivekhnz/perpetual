using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeButtonController : MonoBehaviour
{
    [Serializable]
    public class UpgradeSelectedEvent : UnityEvent<UpgradeBase> { }

    public TextMeshProUGUI UpgradeNameText;
    public TextMeshProUGUI UpgradeDescriptionText;
    public Image UpgradeButtonIcon;
    public UpgradeSelectedEvent OnUpgradeSelected;

    private UpgradeBase upgrade;

    public void SetUpgrade(UpgradeBase upgrade)
    {
        UpgradeNameText.text = upgrade.Name;
        UpgradeDescriptionText.text = upgrade.Description;
        UpgradeButtonIcon.sprite = upgrade.Icon;
        this.upgrade = upgrade;
    }

    public void UpgradeSelected()
    {
        if (OnUpgradeSelected != null)
            OnUpgradeSelected.Invoke(upgrade);
    }
}
