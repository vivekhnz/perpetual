using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject enemy;
    public float spawnInterval = 3f;

    private float spawnTime;

	// Use this for initialization
	void Start () {
        spawnTime = 0;
        SpawnEnemy();
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
        GameObject EnemyToSpawn =
        Instantiate(enemy, transform.position, Quaternion.identity) as GameObject;
        SeekerEnemyMovement enemyController = EnemyToSpawn.GetComponent<SeekerEnemyMovement>();
        enemyController.SetTarget();
        EnemyToSpawn.SetActive(true);
        spawnTime = Time.time;
    }
}
