using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class EnemySpawnManager : MonoBehaviour
{
    public EnemySpawner Spawner;
    public float xBounds = 7;
    public float yBounds = 5;

    private List<EnemySpawner> activeSpawners;
    private int wave;
    private HUDController hudController;

    void Start()
    {
        if (Spawner == null)
            Debug.LogError("No enemy spawner specified!");

        hudController = GameObject.FindObjectOfType<HUDController>();
        activeSpawners = new List<EnemySpawner>();

        wave = 0;
        StartNewWave();
    }

    void StartNewWave()
    {
        // increment current wave
        wave++;
        hudController.ShowWave(wave);

        // the number of spawners created is equal to the wave number
        for (int i = 0; i < wave; i++)
            CreateSpawner();
    }

    void CreateSpawner()
    {
        // determine spawner location
        var spawnPos = new Vector3(
            Random.Range(-xBounds, xBounds),
            Random.Range(-yBounds, yBounds), 0);

        // create spawner
        var spawner = Instantiate(
            Spawner, spawnPos, Quaternion.identity);

        // track when the spawner is destroyed
        spawner.Destroyed += OnSpawnerDestroyed;
        activeSpawners.Add(spawner);
    }

    private void OnSpawnerDestroyed(object sender, EventArgs e)
    {
        // stop tracking the spawner
        var spawner = sender as EnemySpawner;
        spawner.Destroyed -= OnSpawnerDestroyed;
        activeSpawners.Remove(spawner);

        // start a new wave if no more spawners are active and the a game has not ended
        if (activeSpawners.Count == 0
            && !hudController.IsGameOver)
            StartNewWave();
    }

    void OnDestroy()
    {
        // kill any active spawners
        for (int i = 0; i < activeSpawners.Count; i++)
        {
            var spawner = activeSpawners[i];
            activeSpawners.Remove(spawner);
            Destroy(spawner);
            i--;
        }
    }
}
