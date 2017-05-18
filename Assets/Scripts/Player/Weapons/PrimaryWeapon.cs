using UnityEngine;

[RequireComponent(typeof(PlayerWeapon))]
public class PrimaryWeapon : MonoBehaviour
{
    public ProjectileController Projectile;
    public float ProjectileSpreadDegrees = 10.0f;
    public float BulletsPerMinute = 60.0f;

    private float projectileFiredTime = 0.0f;
    private PlayerWeapon weapon;
    private AudioSource gunSound;

    void Start()
    {
        weapon = GetComponent<PlayerWeapon>();
        if (weapon == null)
            Debug.LogError("Weapon not found!");

        gunSound = GetComponent<AudioSource>();
        if (gunSound == null)
            Debug.LogError("Gun Sound not found!");
    }

    void FixedUpdate()
    {
        // fire weapon
        if (Input.GetButton("Fire"))
        {
            Fire();
            weapon.IsFiring = true;
        }
        else
        {
            weapon.IsFiring = false;
        }
    }

    void Fire()
    {
        // play gun sound at a rate slower than actual RoF
        if (!gunSound.isPlaying)
        {
            gunSound.Play();
        }
        if (gunSound.time > 0.05f)
        {
            // randomise pitch and volume to keep it fresh
            float randomVolume = Random.Range(0.4f, 0.6f);
            gunSound.volume = randomVolume;

            float randomPitch = Random.Range(0.8f, 1.2f);
            gunSound.pitch = randomPitch;

            gunSound.Play();
        }

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
