using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class EnemySpawnManager : MonoBehaviour
{
    [Serializable]
    public class EnemySpawn
    {
        public EnemySpawner Spawner;
        public int Weight;
    }

    // Constant that determines the number of waves between boss fights.
    const int WAVES_TO_BOSS = 2;

    public List<EnemySpawn> Spawners;
    public EnemySpawner BossSpawner;

    private List<Vector3> spawnLocations;
    private List<EnemySpawner> activeSpawners;
    private int wave;
    // A countdown til the player faces the boss.
    private int wavesTilBoss = WAVES_TO_BOSS;
    private HUDController hud;

    void Start()
    {
        if (Spawners == null || Spawners.Count < 1)
            Debug.LogError("No enemy spawner specified!");

        if (BossSpawner == null)
            Debug.LogError("No boss spawner specified!");

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
        if (activeSpawners.Count == 0 && !hud.IsGameOver)
        {
            StartNewWave();
            Debug.Log("Starting New Wave");
        }
    }

    // Starts a new wave consisted of normal enemies or a boss.
    void StartNewWave()
    {
        // Increment current wave and subtract the countdown to the boss fight.
        wave++;
        wavesTilBoss--;

        // Spawn a boss if the countdown is done. Otherwise spawn regular wave.
        if (wavesTilBoss == 0)
        {
            // Reset countdown back to default.
            wavesTilBoss = WAVES_TO_BOSS;

            // Subtract a wave since a boss wave is unique. Ie: wave 5 -> boss -> wave 6.
            // Not wave 5 -> boss (6) -> wave 7.
            wave--;

            SpawnBoss();
        }
        else
        {
            SpawnNormalWaves();
        }
    }

    // Spawns a boss by creating an EnemySpawner after updating HUD.
    void SpawnBoss()
    {
        // Update HUD to show the current wave. Flash the BOSS wave text.
        hud.StartFlashingWaveText("BOSS! BOSS!");

        // Create a reusable EnemySpawner that spawns Boss1s.
        CreateSpawner(BossSpawner);
    }

    // Spawns a wave of normal enemies by creating an EnemySpawner after updating HUD.
    void SpawnNormalWaves()
    {
        // Update HUD to show the current wave.
        hud.ShowWave(wave);

        // the number of spawners created is equal to the wave number
        for (int i = 0; i < wave; i++)
            CreateSpawner(PickRandomSpawner());
    }

    EnemySpawner PickRandomSpawner()
    {
        // pick a random spawner type
        int selection = Random.Range(0, Spawners.Sum(s => s.Weight));
        foreach (var spawn in Spawners)
        {
            selection -= spawn.Weight;
            if (selection < 0)
                return spawn.Spawner;
        }
        return Spawners.Last().Spawner;
    }

    void CreateSpawner(EnemySpawner spawnerType)
    {
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