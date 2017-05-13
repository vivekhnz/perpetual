using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class BossController : MonoBehaviour
{
    private enum BossState
    {
        Appearing = 0,
        Active = 1,
        Teleporting = 2
    }

    public AnimationCurve TimeBetweenTeleports = new AnimationCurve(
        new Keyframe(0.0f, 5.0f),
        new Keyframe(1.0f, 0.5f, Mathf.PI * -3f, 0.0f));
    public AnimationCurve TimeBetweenBursts = new AnimationCurve(
        new Keyframe(0.0f, 1.0f),
        new Keyframe(1.0f, 0.1f, Mathf.PI * -0.8f, 0.0f));
    public BossProjectileController Projectile;
    public float BulletsPerMinute = 60.0f;
    public Transform LeftWeapon;
    public Transform RightWeapon;
    public int BulletsPerBurst = 6;

    private EnemyController controller;
    private EnemySpawnManager spawnManager;
    private Animator animator;
    private List<Animator> teleportPoints;
    private Vector3 hidingSpot;

    private BossState currentState;
    private float teleportTime;
    private int selectedTeleport;

    private float projectileFiredTime = 0.0f;
    private int bulletsCreated = 0;
    private float burstFinishedTime;

    void Start()
    {
        controller = GetComponent<EnemyController>();
        controller.InstanceReset += (sender, e) => Initialize();

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("No animator found!");

        // hiding spot (dont know how to temporarily disable)
        hidingSpot = new Vector3(-30, 0, 0);

        Initialize();
    }

    void Initialize()
    {
        // retrieve enemy manager
        spawnManager = GameObject.FindObjectOfType<EnemySpawnManager>();
        if (spawnManager == null)
            Debug.LogError("No spawn manager found!");
        spawnManager.StartBossEncounter(
            controller.DamageableObject.InitialHealth);

        // find teleport points
        teleportPoints = GameObject.FindGameObjectsWithTag("TeleportPoint")
            .Select(obj => obj.GetComponent<Animator>()).ToList();
        if (teleportPoints.Count < 2)
            Debug.LogError("Two teleport points must be defined!");

        // start off-screen and teleport in
        transform.position = hidingSpot;
        currentState = BossState.Active;
        teleportTime = Time.time - TimeBetweenTeleports.Evaluate(0);
        selectedTeleport = -1;
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

        Fire();

        switch (currentState)
        {
            case BossState.Active:
                float healthPercentage = 1.0f - (
                    controller.DamageableObject.CurrentHealth /
                    controller.DamageableObject.InitialHealth);

                // is it time to teleport?
                if (Time.time - teleportTime
                    > TimeBetweenTeleports.Evaluate(healthPercentage))
                    PrepareToTeleport();
                break;
        }
    }

    void PrepareToTeleport()
    {
        // randomly select a teleport destination
        var validIndices = Enumerable.Range(0, teleportPoints.Count).ToList();
        validIndices.Remove(selectedTeleport);
        selectedTeleport = validIndices[
            Random.Range(0, validIndices.Count)];

        // telegraph the teleport destination
        var teleport = teleportPoints[selectedTeleport];
        teleport.SetBool("IsActive", true);

        // play hide animation
        currentState = BossState.Teleporting;
        animator.SetBool("IsHiding", true);
    }

    void Teleport()
    {
        // teleport to destination
        var teleport = teleportPoints[selectedTeleport];
        transform.position = teleport.transform.position;

        // play appear animation
        currentState = BossState.Appearing;
        animator.SetBool("IsHiding", false);
    }

    void Activate()
    {
        // disable our teleport point
        var teleport = teleportPoints[selectedTeleport];
        teleport.SetBool("IsActive", false);

        // enable shooting and reset teleport timer
        currentState = BossState.Active;
        teleportTime = Time.time;
    }

    void Fire()
    {
        if (Projectile == null)
            return;

        // am I active?
        if (currentState != BossState.Active)
            return;

        // can I fire any more bullets in this burst?
        if (bulletsCreated >= BulletsPerBurst)
        {
            float healthPercentage = 1.0f - (
                controller.DamageableObject.CurrentHealth /
                controller.DamageableObject.InitialHealth);

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

        // was this the last bullet in the burst?
        if (bulletsCreated >= BulletsPerBurst)
            burstFinishedTime = Time.time;
    }

    public void UpdateBossHealth()
    {
        spawnManager.UpdateBossHealth(
            controller.DamageableObject.CurrentHealth);
    }

    public void OnDefeated()
    {
        animator.SetBool("IsHiding", true);
        spawnManager.FinishBossEncounter();
    }
}
