﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class EnemySpawnManager : MonoBehaviour
{
    public List<EnemySpawner> Spawners;

    private List<Vector3> spawnLocations;
    private List<EnemySpawner> activeSpawners;
    private int wave;
    private HUDController hud;

    void Start()
    {
        if (Spawners == null)
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
        // start a new wave if no spawners are active and the
        // game has not ended
        if (activeSpawners.Count == 0
            && !hud.IsGameOver)
            StartNewWave();
    }

    void StartNewWave()
    {
        // increment current wave
        wave++;
        hud.ShowWave(wave);

        // the number of spawners created is equal to the wave number
        for (int i = 0; i < wave; i++)
            CreateSpawner();
    }

    void CreateSpawner()
    {
        // pick a random spawner type
        var spawnerType = Spawners[Random.Range(0, Spawners.Count)];
        
        // create spawner
        var spawner = spawnerType.Fetch<EnemySpawner>();
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
