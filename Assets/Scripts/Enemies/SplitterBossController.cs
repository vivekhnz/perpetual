using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplitterBossController : MonoBehaviour {

    private enum BossState
    {
        Appearing = 0,
        Active = 1,
        Teleporting = 2
    }

    public AnimationCurve TimeBetweenBursts =
        AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
    public BossProjectileController Projectile;
    public float BulletsPerMinute = 60.0f;
    public Transform LeftWeapon;
    public Transform RightWeapon;
    public int BulletsPerBurst = 6;
    public float RotationSpeed = 1f;
    public Vector3 EntryLocation;

    private EnemyController controller;
    private EnemySpawnManager spawnManager;
    private Animator animator;
    private Vector3 hidingSpot;

    private BossState currentState;
    private float teleportTime;
    private BossTeleportPointController selectedTeleport;

    private float projectileFiredTime = 0.0f;
    private int bulletsCreated = 0;
    private float burstFinishedTime;

    // Use this for initialization
    void Start () {
        controller = GetComponent<EnemyController>();
        controller.InstanceReset += (sender, e) => Initialize();

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

        // start off-screen and teleport in
        transform.position = Vector3.zero;
        currentState = BossState.Active;
    }

    void FixedUpdate () {

        // keep the boss in the middle of arena to avoid spawning bugs
        transform.position = Vector3.zero;

        // find the direction towards the player
        Vector2 direction = controller.Player.transform.position - transform.position;
        direction.Normalize();
        Quaternion targetRotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        // rotate slowly towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

        Fire();
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
            controller.DamageableObject.CurrentHealth
            / controller.DamageableObject.InitialHealth);
    }

    public void OnDefeated()
    {
        // reset animations
        animator.SetBool("IsHiding", true);
        //foreach (var teleport in teleportPoints)
        //    teleport.Deactivate();

        // kill projectiles
        var projectiles = GameObject.FindObjectsOfType<BossProjectileController>();
        foreach (var projectile in projectiles)
            projectile.Recycle();

        // notify encounter completion
        spawnManager.FinishBossEncounter();
    }
}
