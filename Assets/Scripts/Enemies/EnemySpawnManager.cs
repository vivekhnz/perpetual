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
    public List<EnemySpawner> BossSpawners;
    public int WavesPerRound = 2;
    public GameObject HealthPickup;

    private HUDController hud;
    private DataProvider data;
    private PlayerHealth player;

    private List<Vector3> spawnLocations;
    private List<EnemySpawner> activeSpawners;
    private int wave;
    private int round;
    private int currentBoss;

    void Start()
    {
        if (Spawners == null || Spawners.Count < 1)
            Debug.LogError("No enemy spawner specified!");
        if (BossSpawners == null)
            Debug.LogError("No boss spawner specified!");
        hud = GameObject.FindObjectOfType<HUDController>();
        if (hud == null)
            Debug.LogError("No HUD controller found.");
        data = GetComponent<DataProvider>();
        if (data == null)
            Debug.LogError("No data provider found!");
        player = GameObject.FindObjectOfType<PlayerHealth>();
        if (player == null)
            Debug.LogError("No player found.");
        if (HealthPickup == null)
            Debug.LogError("Health Pickup Prefab Not Found!");

        currentBoss = 0;

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
        if (activeSpawners.Count == 0 && hud.CanProgressToNextWave)
            StartNewWave();
    }

    void StartNewRound()
    {
        round++;
        wave = 0;
        player.ResetHealth();
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
            CreateSpawner(GetBoss());
            SpawnHealthPickup();
        }
        else
        {
            // the number of spawners created is incremented each wave
            var count = Math.Max(((WavesPerRound - 3) * (round - 1)) + wave, 1);
            for (int i = 0; i < count; i++)
                CreateSpawner(PickRandomSpawner());
        }
    }

    EnemySpawner GetBoss()
    {
        var boss = BossSpawners[currentBoss];
        currentBoss = (currentBoss + 1) % BossSpawners.Count;
        return boss;
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

    void SpawnHealthPickup()
    {
        List<Vector3> HealthPickupSpawnPoints = 
            GameObject.FindGameObjectsWithTag("HealthPickupSpawnPoints")
            .Select(obj => obj.transform.position).ToList();
        Instantiate(HealthPickup, HealthPickupSpawnPoints[Random.Range(0, HealthPickupSpawnPoints.Count)],
            Quaternion.identity);
    }

    private void OnSpawnerDestroyed(object sender, EventArgs e)
    {
        // stop tracking the spawner
        var spawner = sender as EnemySpawner;
        spawner.InstanceRecycled -= OnSpawnerDestroyed;
        activeSpawners.Remove(spawner);
    }

    public void StartBossEncounter(float initialHealth)
    {
        data.UpdateValue<bool>("IsBossEncounterActive", true);
        UpdateBossHealth(initialHealth);
    }

    public void FinishBossEncounter()
    {
        data.UpdateValue<bool>("IsBossEncounterActive", false);

        // show upgrade unlocked message
        hud.SignalUpgradeUnlocked();
    }

    public void UpdateBossHealth(float currentHealth)
    {
        data.UpdateValue<float>("BossHealth", currentHealth);
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