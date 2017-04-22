using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour {

    public GameObject spawnPoint;
    public float xBounds = 7;
    public float yBounds = 5;
    
    [HideInInspector]public List<GameObject> EnemySpawners;

    private int spawnPointNumber;
    private int wave;
    private HUDController hudController;

    // Use this for initialization
    void Start () {
        hudController = Object.FindObjectOfType<HUDController>();
        EnemySpawners = new List<GameObject>();
        spawnPointNumber = 1;
        Debug.Log("Spawning");
        wave = 0;
        SpawnPoints();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateSpawn()
    {
        Vector3 spawnPointLocation = 
            new Vector3(Random.Range(-xBounds, xBounds), Random.Range(-yBounds, yBounds), 0);
        GameObject newSpawn = Instantiate(spawnPoint, spawnPointLocation, Quaternion.identity);
        EnemySpawner spawnController = newSpawn.GetComponent<EnemySpawner>();
        if (spawnController != null)
        {
            spawnController.Initialize(this);
        }
        EnemySpawners.Add(newSpawn);
        newSpawn.SetActive(true);
    }

    public void RemoveSpawnPoint(GameObject spawnToRemove)
    {
        EnemySpawners.Remove(spawnToRemove);
        if (EnemySpawners.Count == 0)
        {
            spawnPointNumber++;
            SpawnPoints();
        }
    }

    void SpawnPoints()
    {
        wave++;
        hudController.ShowWave(wave);
        for (int i = 0; i < spawnPointNumber; i++)
        {
            CreateSpawn();
        }
    }
}
