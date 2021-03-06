﻿using UnityEngine;

public class EnemyController : PooledObject
{
    public DamageableObject DamageableObject;
    public float CollisionDamage = 40.0f;
    public string EnemyID = "UnknownEnemy";
    public bool SelfDestructOnCollision = true;

    public PlayerHealth Player { get; private set; }

    void Start()
    {
        if (DamageableObject == null)
            Debug.LogError("Enemy does not have a damageable object.");
    }

    public override void ResetInstance()
    {
        Player = Object.FindObjectOfType<PlayerHealth>();
        DamageableObject.ResetHealth();
    }

    public void Initialize(Vector3 position)
    {
        this.transform.position = position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Player == null)
            return;

        // did I crash into the player?
        if (other.gameObject.Equals(Player.gameObject))
        {
            // damage player
            if (Player != null)
                Player.TakeDamage(CollisionDamage, EnemyID);

            // self-destruct if not boss
            if (SelfDestructOnCollision)
                Recycle();
        }
    }
}
