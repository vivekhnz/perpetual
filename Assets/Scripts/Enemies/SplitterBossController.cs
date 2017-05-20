using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BossController))]
public class SplitterBossController : MonoBehaviour
{
    public AnimationCurve TimeBetweenBursts =
        AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
    public BossProjectileController Projectile;
    public float BulletsPerMinute = 60.0f;
    public Transform Pointer;
    public int BulletsPerBurst = 6;
    public float BodyRotationSpeed = 30f;
    public float PointerRotationSpeed = 12f;

    private BossController controller;

    private float teleportTime;
    private BossTeleportPointController selectedTeleport;

    private float projectileFiredTime = 0.0f;
    private int bulletsCreated = 0;
    private float burstFinishedTime;

    void Start()
    {
        controller = GetComponent<BossController>();
    }

    void FixedUpdate()
    {
        transform.Rotate(0.0f, 0.0f, BodyRotationSpeed * Mathf.Deg2Rad);

        // find the direction towards the player
        Vector2 direction = controller.Player.transform.position - Pointer.position;
        direction.Normalize();
        Quaternion targetRotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        // rotate slowly towards the player
        Pointer.rotation = Quaternion.Slerp(Pointer.rotation, targetRotation,
            Time.deltaTime * PointerRotationSpeed);

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

        // was this the last bullet in the burst?
        if (bulletsCreated >= BulletsPerBurst)
            burstFinishedTime = Time.time;
    }
}
