using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class LaserWeapon : MonoBehaviour
{
    public float Damage = 10.0f;
    // determines how many targets the laser beam
    // can penetrate
    public int MaxDamageablesToHit = 1;
    public ParticleSystem LaserHitEffect;

    LineRenderer line;
    int layerMask;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        if (line == null)
            Debug.LogError("No line renderer found!");

        layerMask = LayerMask.GetMask("Default", "Obstacles");
    }

    void Update()
    {
        line.enabled = false;

        // is the laser being fired?
        if (Input.GetButton("FireSecondary"))
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
            line.enabled = true;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, laserEnd);
        }
    }

    private void CreateLaserHitEffect(Vector3 position)
    {
        if (LaserHitEffect != null)
        {
            var effect = Instantiate(LaserHitEffect);
            effect.transform.position = position;

            float angle = transform.rotation.eulerAngles.z + 180.0f;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad));

            var velocity = effect.velocityOverLifetime;
            velocity.enabled = true;
            velocity.x = new ParticleSystem.MinMaxCurve(direction.x * 3.0f);
            velocity.y = new ParticleSystem.MinMaxCurve(direction.y * 3.0f);
        }
    }
}
