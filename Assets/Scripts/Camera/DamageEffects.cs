using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class DamageEffects : MonoBehaviour
{
    public VignetteAndChromaticAberration effect;
    public float ChromaticAberration = 5.0f;
    public float Blur = 1.0f;
    public float CeaseRate = 0.9f;

    void Start()
    {
        effect.chromaticAberration = 0;
        effect.blur = 0;
    }

    void FixedUpdate()
    {
        if (effect.blur < 0.01f)
        {
            effect.blur = 0;
        }
        else
        {
            effect.blur *= CeaseRate;
        }

        if (effect.chromaticAberration < 0.01f)
        {
            effect.chromaticAberration = 0;
        }
        else
        {
            effect.chromaticAberration *= CeaseRate;
        }
    }

    public void Activate()
    {
        effect.chromaticAberration = ChromaticAberration;
        effect.blur = Blur;
    }
}
