using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class EnemySpawner : PooledObject
{
    [Serializable]
    public class EnemySpawn
    {
        public EnemyController Enemy;
        public int EnemiesToSpawn;

        [HideInInspector]
        public int EnemiesSpawned;
    }

    public List<EnemySpawn> Enemies;
    public float SpawnInterval = 3f;
    public float SpawnDelay = 0f;

    private int totalEnemiesToSpawn;
    private float spawnTime;
    private float createdTime;
    private List<EnemyController> children
        = new List<EnemyController>();

    void Start()
    {
        if (Enemies == null || Enemies.Count < 1)
            Debug.LogError("No enemies specified!");

        totalEnemiesToSpawn = Enemies.Sum(e => e.EnemiesToSpawn);
    }

    public void Initialize(Vector3 position)
    {
        transform.position = position;
        spawnTime = 0;
        createdTime = Time.time;
        foreach (var enemy in Enemies)
            enemy.EnemiesSpawned = 0;
        children.Clear();
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
        if (Time.time - createdTime < SpawnDelay)
            return;

        // do I still have enemies to spawn?
        var totalEnemiesSpawned = Enemies.Sum(e => e.EnemiesSpawned);
        if (totalEnemiesSpawned < totalEnemiesToSpawn)
        {
            // can I spawn another enemy?
            if (Time.time - spawnTime > SpawnInterval)
                SpawnEnemy();
        }
        else if (totalEnemiesSpawned >= totalEnemiesToSpawn
            && children.Count < 1)
        {
            // kill spawner once all enemies have been spawned and all
            // children enemies are destroyed
            Recycle();
        }
    }

    void SpawnEnemy()
    {
        // determine enemy types that can still be spawned
        var types = Enemies.Where(
            e => e.EnemiesSpawned < e.EnemiesToSpawn).ToList();

        // spawn a random enemy type
        var enemyType = types[Random.Range(0, types.Count)];
        var enemy = enemyType.Enemy.Fetch<EnemyController>();
        enemy.Initialize(transform.position);

        // track when the enemy is destroyed
        enemy.InstanceRecycled += OnEnemyDestroyed;
        enemyType.EnemiesSpawned++;
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
