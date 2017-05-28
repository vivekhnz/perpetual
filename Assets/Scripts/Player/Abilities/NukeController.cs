using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeController : PooledObject
{
    public float Damage = 75.0f;
    public float Range = 2.0f;
    public Transform Collider;

    void Start()
    {
        if (Collider == null)
            Debug.LogError("No collider found!");

        Collider.localScale = new Vector3(Range, Range, 1.0f);
    }

    public void OnDamageableHit(Collider2D collider)
    {
        if (Damage < 1)
            return;

        // damage the object
        var damageable = collider.GetComponent<DamageableObject>();
        if (damageable != null)
            damageable.TakeDamage(Damage, transform.rotation.eulerAngles.z);
    }

    void SelfDestruct()
    {
        Recycle();
    }
}
