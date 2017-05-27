using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerWeapon))]
[RequireComponent(typeof(PlayerSecondaryWeapon))]
public class ScatterShotWeapon : MonoBehaviour
{

    public float Cooldown = 1.0f;
    public ProjectileController Projectile;
    public int ProjectileAmount = 10;
    public float ProjectileSpreadDegrees = 45.0f;
    public float ProjectileRange = 0.5f;

    PlayerWeapon weapon;
    PlayerSecondaryWeapon secondaryWeapon;

    private float startFireTime;

    // Use this for initialization
    void Start()
    {
        weapon = GetComponent<PlayerWeapon>();
        if (weapon == null)
            Debug.LogError("Weapon not found!");

        secondaryWeapon = GetComponent<PlayerSecondaryWeapon>();
        if (secondaryWeapon == null)
            Debug.LogError("Secondary weapon not found!");

        startFireTime = Time.time;
    }

    void FixedUpdate()
    {
        // fire weapon
        if (Input.GetButton("FireSecondary") && Time.time - startFireTime > Cooldown)
        {
            Fire();
            weapon.IsFiring = true;
        }
        else
        {
            weapon.IsFiring = false;

            // show time remaining until scattershot is available
            float scatterShotCharge = Mathf.Clamp(
                (Time.time - startFireTime) / Cooldown, 0, 1);
            secondaryWeapon.Charge = scatterShotCharge;
        }
    }

    void Fire()
    {
        if (Projectile == null)
            return;

        // create projectiles
        for (int i = 0; i < ProjectileAmount; i++)
        {
            // calculate projectile direction
            float projectileDir = transform.rotation.eulerAngles.z
                + Random.Range(-ProjectileSpreadDegrees, ProjectileSpreadDegrees);

            // spawn projectile
            var projectile = Projectile.Fetch<ProjectileController>();
            projectile.Initialize(
                transform.position, Quaternion.Euler(0.0f, 0.0f, projectileDir));
            projectile.Lifetime = ProjectileRange;
        }

        // reset time
        startFireTime = Time.time;
    }
}
