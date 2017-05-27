using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ProgressBar : MonoBehaviour
{
    public FloatBinding Value;
    public FloatBinding MinValue;
    public FloatBinding MaxValue;
    public BooleanBinding IsEnabled;

    void Start()
    {
        var slider = GetComponent<Slider>();
        if (slider == null)
            Debug.LogError("No slider found!");

        Value.Subscribe(
            value => slider.value = value);
        MinValue.Subscribe(
            value => slider.minValue = value);
        MaxValue.Subscribe(
            value => slider.maxValue = value);
        IsEnabled.Subscribe(
            value =>
            {
                foreach (var image in GetComponentsInChildren<Image>())
                    image.enabled = value;
            });
    }
}
