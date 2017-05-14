using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerSensor : MonoBehaviour
{
    [Serializable]
    public class Collider2DEvent : UnityEvent<Collider2D> { }

    public List<string> CollidesWithTags;
    public Collider2DEvent OnDetected;

    void Start()
    {
        if (CollidesWithTags == null || CollidesWithTags.Count < 1)
            Debug.LogError("No tags specified!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (OnDetected == null)
            return;

        if (CollidesWithTags.Contains(other.tag))
            OnDetected.Invoke(other);
    }
}
