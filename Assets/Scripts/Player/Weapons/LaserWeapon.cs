using UnityEngine;
using System.Linq;

[RequireComponent(typeof(PlayerWeapon))]
[RequireComponent(typeof(PlayerSecondaryWeapon))]
[RequireComponent(typeof(LineRenderer))]
public class LaserWeapon : MonoBehaviour
{
    public float Damage = 10.0f;
    // determines how many targets the laser beam
    // can penetrate
    public int MaxDamageablesToHit = 1;
    public float LaserDuration = 1.0f;
    public float Cooldown = 3.0f;
    public ParticleSystemAutoDestroy LaserHitEffect;
    public ParticleSystem LaserBeamEffect;

    PlayerWeapon weapon;
    PlayerSecondaryWeapon secondaryWeapon;
    LineRenderer line;
    ParticleSystem.EmissionModule beamEmission;
    AudioSource gunSound;

    int layerMask;
    float startFireTime;
    float stoppedFiringTime;

    void Start()
    {
        weapon = GetComponent<PlayerWeapon>();
        if (weapon == null)
            Debug.LogError("Weapon not found!");

        secondaryWeapon = GetComponent<PlayerSecondaryWeapon>();
        if (secondaryWeapon == null)
            Debug.LogError("Secondary weapon not found!");

        line = GetComponent<LineRenderer>();
        if (line == null)
            Debug.LogError("No line renderer found!");
        line.sortingLayerName = "Effects";

        gunSound = GetComponent<AudioSource>();

        beamEmission = LaserBeamEffect.emission;

        layerMask = LayerMask.GetMask("Default", "Obstacles");
    }

    void FixedUpdate()
    {
        // disable laser
        line.enabled = false;
        beamEmission.enabled = false;

        // is the laser being fired?
        if (Input.GetButtonDown("FireSecondary")
            && Time.time - stoppedFiringTime > Cooldown)
            startFireTime = Time.time;

        if (Input.GetButton("FireSecondary")
            && Time.time - startFireTime < LaserDuration)
        {
            FireLaser();
            weapon.IsFiring = true;

            // play gun sound at a rate slower than actual RoF
            if (!gunSound.isPlaying)
            {
                gunSound.Play();
            }
            // 0.28f is a sweet spot from experimentation. Change at your own risk!
            if (gunSound.time > 0.28f)
            {
                // randomise volume to keep it fresh
                float randomVolume = Random.Range(0.8f, 1);
                gunSound.volume = randomVolume;

                // pitch not randomised due to poor results from testing
                //float randomPitch = Random.Range(0.8f, 1.0f);
                //gunSound.pitch = randomPitch;

                gunSound.Play();
            }

            // show time remaining until laser runs out
            float laserCharge = Mathf.Clamp(
                (Time.time - startFireTime) / LaserDuration, 0, 1);
            secondaryWeapon.Charge = 1.0f - laserCharge;
        }
        else
        {
            // did we just stop firing?
            if (weapon.IsFiring)
                stoppedFiringTime = Time.time;

            weapon.IsFiring = false;

            // show time remaining until laser is available
            float laserCharge = Mathf.Clamp(
                (Time.time - stoppedFiringTime) / Cooldown, 0, 1);
            secondaryWeapon.Charge = laserCharge;
        }
    }

    private void FireLaser()
    {
        // perform raycast
        var mousePos = Camera.main.ScreenToWorldPoint(
            Input.mousePosition);
        var dirToMouse = mousePos - transform.position;
        var hits = Physics2D.RaycastAll(
            transform.position, dirToMouse.normalized,
            float.MaxValue, layerMask);

        Vector2 laserEnd = transform.position;
        int targetsHit = 0;
        foreach (var hit in hits.OrderBy(h => h.distance))
        {
            // can we penetrate through any more targets?
            if (targetsHit >= MaxDamageablesToHit)
                break;

            var tag = hit.collider.gameObject.tag;

            // have we hit a wall?
            if (tag.Equals("Solid"))
            {
                // create particle effect where laser impacts
                CreateLaserHitEffect(hit.point);

                laserEnd = hit.point;
                break;
            }

            // have we hit a damageable object?
            if (tag.Equals("Damageable"))
            {
                // damage the object
                var damageable = hit.collider
                    .GetComponent<DamageableObject>();
                if (damageable != null)
                {
                    damageable.TakeDamage(Damage,
                        transform.rotation.eulerAngles.z);
                    laserEnd = hit.point;
                    targetsHit++;

                    // create particle effect where laser impacts
                    CreateLaserHitEffect(hit.point);
                }
            }
        }

        // draw laser
        float beamLength = Vector2.Distance(transform.position, laserEnd);
        float beamScale = beamLength * 4;

        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, laserEnd);

        beamEmission.enabled = true;
        transform.localScale = new Vector3(beamScale, 1, 1);
    }

    private void CreateLaserHitEffect(Vector3 position)
    {
        if (LaserHitEffect != null)
        {
            var effect = LaserHitEffect.Fetch<ParticleSystemAutoDestroy>();
            effect.transform.position = position;

            float angle = transform.rotation.eulerAngles.z + 180.0f;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad));

            var velocity = effect.ParticleSystem.velocityOverLifetime;
            velocity.enabled = true;
            velocity.x = new ParticleSystem.MinMaxCurve(direction.x * 3.0f);
            velocity.y = new ParticleSystem.MinMaxCurve(direction.y * 3.0f);
        }
    }
}
