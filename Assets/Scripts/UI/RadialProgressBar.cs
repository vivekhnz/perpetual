using UnityEngine;
using UnityEngine.UI;

public class RadialProgressBar : MonoBehaviour
{
    public Image Foreground;
    public FloatBinding Value;
    public BooleanBinding IsEnabled;

    void Start()
    {
        if (Foreground == null)
            Debug.LogError("No foreground image specified!");

        Value.Subscribe(
            value => Foreground.fillAmount = value);
        IsEnabled.Subscribe(
            value =>
            {
                foreach (var child in GetComponentsInChildren<Image>())
                    child.enabled = value;
            });
    }
}
