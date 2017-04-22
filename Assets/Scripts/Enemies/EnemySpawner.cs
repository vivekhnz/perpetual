using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject enemy;
    public float spawnInterval = 3f;
    public int enemiesToSpawn;

    private float spawnTime;
    private EnemySpawnManager Manager;

	// Use this for initialization
	void Start () {
        spawnTime = 0;
        SpawnEnemy();
	}

    public void Initialize(EnemySpawnManager spawnManager)
    {
        Manager = spawnManager;
    }
	
	// Update is called once per frame
	void Update () {
		if ((Time.time - spawnTime) > spawnInterval)
        {
            SpawnEnemy();
        }
	}

    void SpawnEnemy()
    {
        if (enemiesToSpawn <= 0)
        {
            DeleteSpawn();
        }
        else
        {
            GameObject EnemyToSpawn =
            Instantiate(enemy, transform.position, Quaternion.identity) as GameObject;
            SeekerEnemyMovement enemyController = EnemyToSpawn.GetComponent<SeekerEnemyMovement>();
            enemyController.SetTarget();
            EnemyToSpawn.SetActive(true);
            enemiesToSpawn--;
            spawnTime = Time.time;
        }
    }

    void DeleteSpawn()
    {
        Manager.RemoveSpawnPoint(gameObject);
        Destroy(gameObject);
    }
}
