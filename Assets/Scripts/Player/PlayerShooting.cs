using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public ProjectileController Projectile;
    public float ProjectileSpreadDegrees = 10.0f;
    public float BulletsPerMinute = 60.0f;

    private float projectileFiredTime = 0.0f;

    void FixedUpdate()
    {
        // fire weapon
        if (Input.GetButton("Fire"))
            Fire();
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
        var projectile = Projectile.Fetch<ProjectileController>();
        projectile.Initialize(
            transform.position, Quaternion.Euler(0.0f, 0.0f, projectileDir));
    }
}
