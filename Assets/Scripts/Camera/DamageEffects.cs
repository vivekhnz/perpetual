using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class DamageEffects : MonoBehaviour {

    public VignetteAndChromaticAberration effect;
    public float Intensity = 10;
    public float CeaseRate = 0.1f;

	// Use this for initialization
	void Start (){
        effect.chromaticAberration = 0;

    }

    void FixedUpdate()
    {
        if (effect.chromaticAberration < 0)
        {
            effect.chromaticAberration = 0;
        }

        if (effect.chromaticAberration != 0)
        {
            effect.chromaticAberration -= CeaseRate;
        }
    }

    public void Activate()
    {
        effect.chromaticAberration = Intensity;
    }
}
