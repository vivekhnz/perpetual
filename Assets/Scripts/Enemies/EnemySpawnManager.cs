﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class EnemySpawnManager : MonoBehaviour
{
    public EnemySpawner Spawner;

    private List<Vector3> spawnLocations;
    private List<EnemySpawner> activeSpawners;
    private int wave;
    private int wavesTilBoss = 5; // A countdown til the player faces the boss.
    private HUDController hud;

    void Start()
    {
        if (Spawner == null)
            Debug.LogError("No enemy spawner specified!");

        hud = GameObject.FindObjectOfType<HUDController>();
        activeSpawners = new List<EnemySpawner>();
        wave = 0;

        // find spawn points
        spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPoint")
            .Select(obj => obj.transform.position).ToList();
        if (spawnLocations.Count == 0)
            Debug.LogError("No spawn points defined!");
    }

    void FixedUpdate()
    {
        // First check if a boss needs to spawn. If not, proceed to spawn normally.
        if (wavesTilBoss == 0)
        {
            SpawnBoss();
        }

        // start a new wave if no spawners are active and the
        // game has not ended
        else if (activeSpawners.Count == 0
            && !hud.IsGameOver)
            StartNewWave();
    }

    void StartNewWave()
    {
        // increment current wave
        wave++;
        wavesTilBoss--;
        hud.ShowWave(wave);

        // the number of spawners created is equal to the wave number
        for (int i = 0; i < wave; i++)
            CreateSpawner();
    }

    void CreateSpawner()
    {
        // create spawner
        var spawner = Spawner.Fetch<EnemySpawner>();
        spawner.Initialize(
            spawnLocations[Random.Range(0, spawnLocations.Count)]);

        // track when the spawner is destroyed
        spawner.InstanceRecycled += OnSpawnerDestroyed;
        activeSpawners.Add(spawner);
    }

    private void OnSpawnerDestroyed(object sender, EventArgs e)
    {
        // stop tracking the spawner
        var spawner = sender as EnemySpawner;
        spawner.InstanceRecycled -= OnSpawnerDestroyed;
        activeSpawners.Remove(spawner);
    }

    void SpawnBoss()
    {
        // Spawns a boss after every 5 waves.
        hud.ShowWave("BOSS! BOSS!");
    }

    void OnDestroy()
    {
        // kill any active spawners
        for (int i = 0; i < activeSpawners.Count; i++)
        {
            var spawner = activeSpawners[i];
            activeSpawners.Remove(spawner);
            spawner.Recycle();
            i--;
        }
    }
}
