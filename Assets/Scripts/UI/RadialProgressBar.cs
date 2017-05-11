using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RadialProgressBar : MonoBehaviour
{
    public FloatBinding Value;

    void Start()
    {
        var image = GetComponent<Image>();
        if (image == null)
            Debug.LogError("No image found!");

        Value.Subscribe(
            value => image.fillAmount = value);
    }
}
