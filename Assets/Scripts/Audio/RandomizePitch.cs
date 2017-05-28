using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomizePitch : MonoBehaviour
{
    public float MinimumPitch = 0.85f;
    public float MaximumPitch = 1.15f;

    void Awake()
    {
        var source = GetComponent<AudioSource>();
        if (source != null)
            source.pitch = Random.Range(MinimumPitch, MaximumPitch);
    }
}
