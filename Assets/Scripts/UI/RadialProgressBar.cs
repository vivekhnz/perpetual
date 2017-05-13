using UnityEngine;
using UnityEngine.UI;

public class RadialProgressBar : MonoBehaviour
{
    public Image Foreground;
    public Image Icon;

    public FloatBinding Value;
    public BooleanBinding IsEnabled;
    public SpriteBinding IconSprite;

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
        IconSprite.Subscribe(
            sprite => Icon.sprite = sprite);
    }
}
