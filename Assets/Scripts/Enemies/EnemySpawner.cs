using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : PooledObject
{
    public EnemyController Enemy;
    public float SpawnInterval = 3f;
    public int EnemiesToSpawn = 5;

    private float spawnTime;
    private int enemiesSpawned;
    private List<EnemyController> children
        = new List<EnemyController>();

    public void Initialize(Vector3 position)
    {
        transform.position = position;
        spawnTime = 0;
        enemiesSpawned = 0;
        children.Clear();

        // spawn the first enemy
        SpawnEnemy();
    }

    public override void CleanupInstance()
    {
        // kill any children enemies
        for (int i = 0; i < children.Count; i++)
        {
            var enemy = children[i];
            children.Remove(enemy);
            enemy.Recycle();
            i--;
        }
    }

    void FixedUpdate()
    {
        // do I still have enemies to spawn?
        if (enemiesSpawned < EnemiesToSpawn)
        {
            // can I spawn another enemy?
            if (Time.time - spawnTime > SpawnInterval)
                SpawnEnemy();
        }
        else if (enemiesSpawned >= EnemiesToSpawn
            && children.Count < 1)
        {
            // kill spawner once all enemies have been spawned and all
            // children enemies are destroyed
            Recycle();
        }
    }

    void SpawnEnemy()
    {
        // spawn enemy
        var enemy = Enemy.Fetch<EnemyController>();
        enemy.Initialize(transform.position);

        // track when the enemy is destroyed
        enemy.InstanceRecycled += OnEnemyDestroyed;
        enemiesSpawned++;
        children.Add(enemy);

        // reset spawn cooldown
        spawnTime = Time.time;
    }

    private void OnEnemyDestroyed(object sender, EventArgs e)
    {
        // stop tracking the enemy
        var enemy = sender as EnemyController;
        enemy.InstanceRecycled -= OnEnemyDestroyed;
        children.Remove(enemy);
    }
}
