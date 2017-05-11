using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float TimeToTeleport = 5;
    public float TimeToHide = 3;
    public BossProjectileController Projectile;
    public float BulletsPerMinute = 60.0f;
    public Transform LeftWeapon;
    public Transform RightWeapon;
    public int BulletsPerBurst = 6;
    public float TimeBetweenBursts = 1f;

    private List<Animator> teleportPoints;
    private float teleportTime;
    private Animator selectedTeleport;
    private Vector3 hidingSpot;
    private bool hiding;
    private EnemyController controller;

    private float projectileFiredTime = 0.0f;
    private int bulletsCreated = 0;
    private float burstFinishedTime;

    void Start()
    {
        controller = GetComponent<EnemyController>();

        // initialise timeholder and bool
        hiding = false;
        teleportTime = Time.time;

        // find teleport points
        teleportPoints = GameObject.FindGameObjectsWithTag("TeleportPoint")
            .Select(obj => obj.GetComponent<Animator>()).ToList();
        if (teleportPoints.Count == 0)
            Debug.LogError("No teleport points defined!");

        // hiding spot (dont know how to temporarily disable)
        hidingSpot = new Vector3(-30, 0, 0);
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
            selectedTeleport = teleportPoints[Random.Range(0, teleportPoints.Count)];
            selectedTeleport.SetBool("IsActive", true);
            teleportTime = Time.time;
        }
        else
        {
            // if time to re appear, teleport boss to teleport location
            if (hiding && (Time.time - teleportTime) > TimeToHide)
            {
                hiding = false;
                transform.position = selectedTeleport.transform.position;
                selectedTeleport.SetBool("IsActive", false);
                teleportTime = Time.time;
            }
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

        if (bulletsCreated >= BulletsPerBurst)
            burstFinishedTime = Time.time;
    }
}
