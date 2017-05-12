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
        OffScreen = 0,
        Appearing = 1,
        Active = 2,
        Teleporting = 3
    }

    public float TimeBetweenTeleports = 5;
    public float TimeToHide = 1;
    public BossProjectileController Projectile;
    public float BulletsPerMinute = 60.0f;
    public Transform LeftWeapon;
    public Transform RightWeapon;
    public int BulletsPerBurst = 6;
    public float TimeBetweenBursts = 1f;

    private EnemyController controller;
    private HUDController hudController;
    private Animator animator;
    private List<Animator> teleportPoints;
    private Vector3 hidingSpot;

    private BossState currentState;
    private float hideTime;
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
        // retrieve HUD controller
        hudController = GameObject.FindObjectOfType<HUDController>();
        if (hudController == null)
            Debug.LogError("No HUD controller found!");

        // find teleport points
        teleportPoints = GameObject.FindGameObjectsWithTag("TeleportPoint")
            .Select(obj => obj.GetComponent<Animator>()).ToList();
        if (teleportPoints.Count < 2)
            Debug.LogError("Two teleport points must be defined!");

        // start off-screen
        StartTeleport(false);
        Hide();
        animator.SetBool("IsHiding", true);
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
            case BossState.OffScreen:
                // is it time to appear?
                if (Time.time - hideTime > TimeToHide)
                    Appear();
                break;
            case BossState.Active:
                // is it time to teleport?
                if (Time.time - teleportTime > TimeBetweenTeleports)
                    StartTeleport(true);
                break;
        }
    }

    void Hide()
    {
        // teleport off-screen
        currentState = BossState.OffScreen;
        transform.position = hidingSpot;

        // reset hide timer
        hideTime = Time.time;
    }

    void Appear()
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

    void StartTeleport(bool animate)
    {
        // randomly select a teleport destination
        var validIndices = Enumerable.Range(0, teleportPoints.Count).ToList();
        validIndices.Remove(selectedTeleport);
        selectedTeleport = validIndices[
            Random.Range(0, validIndices.Count)];

        // telegraph the teleport destination
        var teleport = teleportPoints[selectedTeleport];
        teleport.SetBool("IsActive", true);

        if (animate)
        {
            // play hide animation
            currentState = BossState.Teleporting;
            animator.SetBool("IsHiding", true);
        }
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
            // can I start a new burst?
            if (Time.time - burstFinishedTime >= TimeBetweenBursts)
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

    public void UpdateHealthUI()
    {
        hudController.UpdateBossHealth(
            controller.DamageableObject.CurrentHealth);
    }
}
