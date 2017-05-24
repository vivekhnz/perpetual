using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeButtonController : MonoBehaviour
{
    public string UpgradeName;
    public string UpgradeType;
    public Sprite UpgradeIcon;

    public Text UpgradeNameText;
    public Text UpgradeTypeText;
    public Image UpgradeButtonIcon;
    public UnityEvent OnUpgradeSelected;

    void Start()
    {
        UpgradeNameText.text = UpgradeName;
        UpgradeTypeText.text = UpgradeType;
        UpgradeButtonIcon.sprite = UpgradeIcon;
    }

    public void UpgradeSelected()
    {
        if (OnUpgradeSelected != null)
            OnUpgradeSelected.Invoke();
    }
}
