﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float TimeToTeleport = 5;
    public float TimeToHide = 3;
    public float MovementSpeed = 2.0f;
    public BossProjectileController Projectile;
    public float ProjectileSpreadDegrees = 10.0f;
    public float BulletsPerMinute = 60.0f;
    public Transform LeftWeapon;
    public Transform RightWeapon;
    public int BurstNumber = 6;

    private List<Vector3> teleportLocations;
    private float teleportTime;
    private Vector3 selectedTeleport;
    private Vector3 hidingSpot;
    private bool hiding;
    private EnemyController controller;
    private float projectileFiredTime = 0.0f;
    private int currentBurst = 0;
    private Transform currentWeapon;

    void Start()
    {

        controller = GetComponent<EnemyController>();

        // initialise timeholder and bool
        hiding = false;
        teleportTime = Time.time;

        // find teleport points
        teleportLocations = GameObject.FindGameObjectsWithTag("TeleportPoint")
            .Select(obj => obj.transform.position).ToList();
        if (teleportLocations.Count == 0)
            Debug.LogError("No teleport points defined!");

        // hiding spot (dont know how to temporarily disable)
        hidingSpot = new Vector3(-30, 0, 0);

        // initialise weapon
        currentWeapon = LeftWeapon;
    }

    void FixedUpdate()
    {

        if (controller.Player == null)
            return;

        Fire();

        // rotate to face player
        Vector2 direction = controller.Player.transform.position - transform.position;
        direction.Normalize();
        transform.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        // if time to teleport, teleport boss
        if (!hiding && (Time.time - teleportTime) > TimeToTeleport)
        {
            hiding = true;
            transform.position = hidingSpot;
            selectedTeleport = teleportLocations[Random.Range(0, teleportLocations.Count)];
            teleportTime = Time.time;
        }
        else
        {
            // if time to re appear, teleport boss to teleport location
            if (hiding && (Time.time - teleportTime) > TimeToHide)
            {
                hiding = false;
                transform.position = selectedTeleport;
                teleportTime = Time.time;
            }
        }
    }

    void Fire()
    {
        if (Projectile == null)
            return;

        // can I fire again?
        float timeBetweenProjectiles = 60.0f / BulletsPerMinute;
        if (Time.time - projectileFiredTime < timeBetweenProjectiles)
            return;
        projectileFiredTime = Time.time;

        // calculate projectile direction
        float projectileDir = transform.rotation.eulerAngles.z
            + Random.Range(-ProjectileSpreadDegrees, ProjectileSpreadDegrees);

        // spawn projectile
        var projectile = Projectile.Fetch<BossProjectileController>();
        if (currentBurst > BurstNumber)
        {
            switchWeapon();
            currentBurst = 0;
        }
        projectile.Initialize(
            currentWeapon.position, Quaternion.Euler(0.0f, 0.0f, projectileDir));
        currentBurst++;
    }

    void switchWeapon()
    {
        if (currentWeapon == LeftWeapon)
        {
            currentWeapon = RightWeapon;
        }
        else
        {
            currentWeapon = LeftWeapon;
        }
    }
}
