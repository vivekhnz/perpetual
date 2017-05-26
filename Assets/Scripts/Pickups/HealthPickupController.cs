using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupController : MonoBehaviour {

    public float HealthGained = 30;
    public float ActiveTime = 5;
    public AudioClip HealthPickedUpSfx;

    void Start()
    {
        // health pickup will disappear after set time
        Invoke("Disappear", ActiveTime);
    }

    void Disappear()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.tag)
        {
            case "Player":
                // give player health
                other.gameObject.GetComponentInChildren<PlayerHealth>().GainHealth(HealthGained);
                AudioSource.PlayClipAtPoint(HealthPickedUpSfx, transform.position);
                Disappear();
                break;
            default:
                break;
        }
    }
}
