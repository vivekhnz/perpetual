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
        AnimationCurve.Linear(0.0f, 30.0f, 1.0f, 60.0f);
    public AnimationCurve MovementSpeed =
        AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.5f);
    public BossProjectileController Projectile;
    public float BulletsPerMinute = 60.0f;
    public Transform Pointer;
    public int BulletsPerBurst = 6;
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
        // rotate body
        transform.Rotate(0.0f, 0.0f,
            RotationSpeed.Evaluate(1.0f - controller.HealthPercentage) * Mathf.Deg2Rad);

        // rotate pointer to face player
        Vector2 direction = controller.Player.transform.position - transform.position;
        direction.Normalize();
        Pointer.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        // update animator health stage
        animator.SetFloat("HealthPercentage", controller.HealthPercentage);

        // am I active?
        if (!controller.IsBossActive)
            return;

        // move towards player
        var speed = MovementSpeed.Evaluate(1.0f - controller.HealthPercentage);
        transform.Translate(
            direction * speed * Time.deltaTime, Space.World);

        Fire();
    }

    void Fire()
    {
        if (Projectile == null)
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
