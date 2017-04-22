using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour {

    public GameObject spawnPoint;
    public float xBounds = 7;
    public float yBounds = 5;

    [HideInInspector]public List<GameObject> EnemySpawners;

	// Use this for initialization
	void Start () {
        EnemySpawners = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (EnemySpawners.Count == 0)
        {
            CreateSpawn();
        }
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
    }
}
