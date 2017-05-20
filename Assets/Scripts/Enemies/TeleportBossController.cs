using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(BossController))]
[RequireComponent(typeof(Animator))]
public class TeleportBossController : MonoBehaviour
{
    public AnimationCurve TimeBetweenTeleports =
        AnimationCurve.Linear(0.0f, 4.0f, 1.0f, 0.5f);
    public AnimationCurve TimeBetweenBursts =
        AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
    public AnimationCurve TeleportSpeed =
        AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 2.0f);
    public BossProjectileController Projectile;
    public float BulletsPerMinute = 60.0f;
    public Transform LeftWeapon;
    public Transform RightWeapon;
    public int BulletsPerBurst = 6;

    private BossController controller;
    private Animator animator;
    private AudioSource gunSound;

    private List<BossTeleportPointController> teleportPoints;
    private float teleportTime;
    private BossTeleportPointController selectedTeleport;

    private float projectileFiredTime = 0.0f;
    private int bulletsCreated = 0;
    private float burstFinishedTime;

    void Start()
    {
        controller = GetComponent<BossController>();
        controller.Initialized += OnInitialized;
        controller.Teleporting += OnTeleporting;
        controller.Teleported += (sender, args) => animator.SetBool("IsHiding", false);
        controller.Activated += OnActivated;
        controller.Defeated += OnDefeated;

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("No animator found!");

        gunSound = GetComponent<AudioSource>();
    }

    void OnInitialized(object sender, EventArgs e)
    {
        // find teleport points
        teleportPoints = GameObject.FindGameObjectsWithTag("TeleportPoint")
            .Select(obj => obj.GetComponent<BossTeleportPointController>()).ToList();
        if (teleportPoints.Count < 2)
            Debug.LogError("Two teleport points must be defined!");

        // teleport in
        teleportTime = Time.time - TimeBetweenTeleports.Evaluate(0);
        selectedTeleport = null;
    }

    void OnTeleporting(object sender, BossController.TeleportingEventArgs args)
    {
        // determine which teleport points I can go to
        var validTeleports = (
            from t in teleportPoints
            where t != selectedTeleport
            && controller.HealthPercentage <= t.HealthThreshold
            select t).ToList();

        // randomly select a teleport destination
        selectedTeleport = validTeleports[
            Random.Range(0, validTeleports.Count)];

        // telegraph the destination
        selectedTeleport.Activate();
        animator.SetBool("IsHiding", true);

        args.Destination = selectedTeleport.transform.position;
    }

    void OnActivated(object sender, EventArgs e)
    {
        selectedTeleport.Deactivate();
        teleportTime = Time.time;
    }

    void OnDefeated(object sender, EventArgs e)
    {
        animator.SetBool("IsHiding", true);
        foreach (var teleport in teleportPoints)
            teleport.Deactivate();
    }

    void FixedUpdate()
    {
        if (controller.Player == null)
            return;

        // rotate to face player
        Vector2 direction = controller.Player.transform.position - transform.position;
        direction.Normalize();
        transform.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        float healthPercentage = 1.0f - controller.HealthPercentage;
        animator.SetFloat("TeleportSpeed",
            TeleportSpeed.Evaluate(healthPercentage));
        animator.SetFloat("HealthPercentage", controller.HealthPercentage);

        Fire();

        if (controller.IsBossActive)
        {
            // is it time to teleport?
            if (Time.time - teleportTime
                > TimeBetweenTeleports.Evaluate(healthPercentage))
                controller.BeginTeleport();
        }
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

        // calculate projectile direction
        var currentWeapon = bulletsCreated % 2 == 0
            ? LeftWeapon : RightWeapon;
        var dirToPlayer = controller.Player.transform.position
            - currentWeapon.position;
        float projectileDir = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x)
            * Mathf.Rad2Deg;

        projectile.Initialize(
            currentWeapon.position, Quaternion.Euler(0.0f, 0.0f, projectileDir));
        bulletsCreated++;

        // play fire sound
        gunSound.pitch = Random.Range(0.7f, 1.0f);
        gunSound.Play();

        // was this the last bullet in the burst?
        if (bulletsCreated >= BulletsPerBurst)
            burstFinishedTime = Time.time;
    }
}
