using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyController Enemy;
    public float SpawnInterval = 3f;
    public int EnemiesToSpawn = 5;

    private float spawnTime;
    private EnemySpawnManager manager;
    private int enemiesSpawned;
    private List<EnemyController> children;

    public event EventHandler Destroyed;

    void Start()
    {
        manager = GameObject.FindObjectOfType<EnemySpawnManager>();
        if (manager == null)
            Debug.LogError("No spawn manager found!");

        spawnTime = 0;
        enemiesSpawned = 0;
        children = new List<EnemyController>();

        // spawn the first enemy
        SpawnEnemy();
    }

    void FixedUpdate()
    {
        // do I still have enemies to spawn?
        if (enemiesSpawned >= EnemiesToSpawn)
            return;

        // can I spawn another enemy?
        if (Time.time - spawnTime > SpawnInterval)
            SpawnEnemy();
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

        // kill spawner once all enemies have been spawned and all
        // children enemies were destroyed
        if (enemiesSpawned >= EnemiesToSpawn
            && children.Count < 1)
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        // kill any children enemies
        for (int i = 0; i < children.Count; i++)
        {
            var enemy = children[i];
            children.Remove(enemy);
            enemy.Recycle();
            i--;
        }

        // notify any subscribers that I've been destroyed
        if (Destroyed != null)
            Destroyed(this, EventArgs.Empty);
    }
}
