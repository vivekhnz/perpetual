using UnityEngine;
using UnityEngine.Events;

public class PlayerWeapon : MonoBehaviour
{
    public bool IsFiring { get; set; }
    public float CameraShakeAmount = 0.0f;

    private CameraShake shaker;

    void Start()
    {
        shaker = Camera.main.GetComponent<CameraShake>();
    }

    void FixedUpdate()
    {
        // shake the camera if the weapon is being fired
        if (IsFiring && shaker != null)
            shaker.RandomShake(CameraShakeAmount);
    }
}

