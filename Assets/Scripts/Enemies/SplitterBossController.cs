using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(BossController))]
[RequireComponent(typeof(Animator))]
public class SplitterBossController : MonoBehaviour
{
    public AnimationCurve TimeBetweenBursts =
        AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
    public AnimationCurve RotationSpeed =
        AnimationCurve.Linear(0.0f, 2.0f, 1.0f, 1.0f);
    public BossProjectileController Projectile;
    public float BulletsPerMinute = 60.0f;
    public Transform Pointer;
    public int BulletsPerBurst = 6;
    public float BodyRotationSpeed = 30f;
    public float PointerRotationSpeed = 12f;

    private BossController controller;
    private Animator animator;
    private AudioSource gunSound;

    private float projectileFiredTime = 0.0f;
    private int bulletsCreated = 0;
    private float burstFinishedTime;

    void Start()
    {
        controller = GetComponent<BossController>();
        controller.Teleporting += (sender, args) => animator.SetBool("IsHiding", true);
        controller.Teleported += (sender, args) => animator.SetBool("IsHiding", false);
        controller.Defeated += (sender, args) => animator.SetBool("IsHiding", true);

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("No animator found!");

        gunSound = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        transform.Rotate(0.0f, 0.0f, BodyRotationSpeed * Mathf.Deg2Rad);

        // find the direction towards the player
        Vector2 direction = controller.Player.transform.position - Pointer.position;
        direction.Normalize();
        Quaternion targetRotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        // find rotation speed according to health stage
        float pointerRotate = PointerRotationSpeed * (RotationSpeed.Evaluate(
            1.0f - controller.HealthPercentage));

        // rotate slowly towards the player
        Pointer.rotation = Quaternion.Slerp(Pointer.rotation, targetRotation,
            Time.deltaTime * pointerRotate);

        // update animator health stage
        animator.SetFloat("HealthLevel", controller.HealthPercentage);
        Debug.Log(controller.HealthPercentage);

        Fire();
    }

    void Fire()
    {
        if (Projectile == null)
            return;

        // am I active?
        if (!controller.IsBossActive)
            return;

        // can I fire any more bullets in this burst?
        if (bulletsCreated >= BulletsPerBurst)
        {
            float healthPercentage = 1.0f - controller.HealthPercentage;

            // can I start a new burst?
            if (Time.time - burstFinishedTime >=
                TimeBetweenBursts.Evaluate(healthPercentage))
            {
                // start a new burst
                bulletsCreated = 0;
            }
            else
            {
                return;
            }
        }

        // can I fire again?
        float timeBetweenProjectiles = 60.0f / BulletsPerMinute;
        if (Time.time - projectileFiredTime < timeBetweenProjectiles)
            return;
        projectileFiredTime = Time.time;

        // spawn projectile
        var projectile = Projectile.Fetch<BossProjectileController>();
        projectile.Initialize(Pointer.position, Pointer.rotation);
        bulletsCreated++;

        // play fire sound
        gunSound.pitch = Random.Range(0.7f, 1.0f);
        gunSound.Play();

        // was this the last bullet in the burst?
        if (bulletsCreated >= BulletsPerBurst)
            burstFinishedTime = Time.time;
    }
}
