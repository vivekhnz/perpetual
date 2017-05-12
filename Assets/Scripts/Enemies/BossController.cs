using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    public float TimeBetweenTeleports = 5;
    public float TimeToHide = 1;
    public BossProjectileController Projectile;
    public float BulletsPerMinute = 60.0f;
    public Transform LeftWeapon;
    public Transform RightWeapon;
    public int BulletsPerBurst = 6;
    public float TimeBetweenBursts = 1f;

    private List<Animator> teleportPoints;
    private float hideTime;
    private float teleportTime;
    private int selectedTeleport;
    private Vector3 hidingSpot;
    private bool hiding;
    private EnemyController controller;

    private float projectileFiredTime = 0.0f;
    private int bulletsCreated = 0;
    private float burstFinishedTime;

    void Start()
    {
        controller = GetComponent<EnemyController>();
        controller.InstanceReset += OnInitialized;

        // find teleport points
        teleportPoints = GameObject.FindGameObjectsWithTag("TeleportPoint")
            .Select(obj => obj.GetComponent<Animator>()).ToList();
        if (teleportPoints.Count < 2)
            Debug.LogError("Two teleport points must be defined!");

        // hiding spot (dont know how to temporarily disable)
        hidingSpot = new Vector3(-30, 0, 0);
    }

    void OnInitialized(object sender, EventArgs e)
    {
        Hide();
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

        // are we hiding off-screen?
        if (hiding)
        {
            // is it time to reveal?
            if (Time.time - hideTime > TimeToHide)
                Reveal();
        }
        else
        {
            // is it time to hide?
            if (Time.time - teleportTime > TimeBetweenTeleports)
                Hide();
        }
    }

    void Fire()
    {
        if (Projectile == null)
            return;

        // am I hidden?
        if (hiding)
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

    void Hide()
    {
        // teleport off-screen
        hiding = true;
        transform.position = hidingSpot;

        // randomly select a teleport destination
        var validIndices = Enumerable.Range(0, teleportPoints.Count).ToList();
        validIndices.Remove(selectedTeleport);
        selectedTeleport = validIndices[
            Random.Range(0, validIndices.Count)];

        // telegraph the teleport destination
        var teleport = teleportPoints[selectedTeleport];
        teleport.SetBool("IsActive", true);

        // reset hide timer
        hideTime = Time.time;
    }

    void Reveal()
    {
        // teleport to destination
        hiding = false;
        var teleport = teleportPoints[selectedTeleport];
        transform.position = teleport.transform.position;
        teleport.SetBool("IsActive", false);

        // reset teleport timer
        teleportTime = Time.time;
    }
}
