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

    public List<EnemySpawn> Spawners;
    public EnemySpawner BossSpawner;
    public int WavesPerRound = 2;

    private HUDController hud;

    private List<Vector3> spawnLocations;
    private List<EnemySpawner> activeSpawners;
    private int wave;
    private int round;

    void Start()
    {
        if (Spawners == null || Spawners.Count < 1)
            Debug.LogError("No enemy spawner specified!");

        if (BossSpawner == null)
            Debug.LogError("No boss spawner specified!");

        hud = GameObject.FindObjectOfType<HUDController>();
        activeSpawners = new List<EnemySpawner>();
        round = 0;
        StartNewRound();

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
            StartNewWave();
    }

    void StartNewRound()
    {
        round++;
        wave = 0;
    }

    void StartNewWave()
    {
        // start a new round if we defeated the boss
        if (wave > WavesPerRound)
            StartNewRound();

        // increment current wave and update HUD
        wave++;
        hud.ShowRoundAndWave(round, wave);

        // have we reached a boss fight?
        if (wave == WavesPerRound + 1)
        {
            // signal the boss fight and create the boss spawner
            hud.SignalBossFight();
            CreateSpawner(BossSpawner);
        }
        else
        {
            // the number of spawners created is incremented each wave
            var count = (WavesPerRound * (round - 1)) + wave;
            for (int i = 0; i < count; i++)
                CreateSpawner(PickRandomSpawner());
        }
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