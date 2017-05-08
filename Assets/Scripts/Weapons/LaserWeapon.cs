using UnityEngine;
using System.Linq;

public class LaserWeapon : MonoBehaviour
{
    public float Damage = 10.0f;

    int layerMask;

    void Start()
    {
        layerMask = LayerMask.GetMask("Default", "Obstacles");
    }

    void FixedUpdate()
    {
        // is the laser being fired?
        if (Input.GetButton("FireSecondary"))
        {
            // perform raycast
            var dirToMouse =
                Camera.main.ScreenToWorldPoint(Input.mousePosition)
                - transform.position;
            var hits = Physics2D.RaycastAll(
                transform.position, dirToMouse.normalized,
                float.MaxValue, layerMask);
            foreach (var hit in hits.OrderBy(h => h.distance))
            {
                var tag = hit.collider.gameObject.tag;

                // have we hit a wall?
                if (tag.Equals("Solid"))
                    break;

                // have we hit a damageable object?
                if (tag.Equals("Damageable"))
                {
                    // damage the object
                    var damageable = hit.collider
                        .GetComponent<DamageableObject>();
                    if (damageable != null)
                        damageable.TakeDamage(Damage,
                            transform.rotation.eulerAngles.z);
                }
            }
        }
    }
}
