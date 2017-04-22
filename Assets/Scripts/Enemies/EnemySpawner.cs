using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyController Enemy;
    public float SpawnInterval = 3f;
    public int EnemiesToSpawn = 5;

    private float spawnTime;
    private EnemySpawnManager Manager;

    // Use this for initialization
    void Start()
    {
        spawnTime = 0;
        SpawnEnemy();
    }

    public void Initialize(EnemySpawnManager spawnManager)
    {
        Manager = spawnManager;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time - spawnTime) > SpawnInterval)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (EnemiesToSpawn <= 0)
        {
            DeleteSpawn();
        }
        else
        {
            Instantiate(Enemy, transform.position, Quaternion.identity);
            EnemiesToSpawn--;
            spawnTime = Time.time;
        }
    }

    void DeleteSpawn()
    {
        Manager.RemoveSpawnPoint(gameObject);
        Destroy(gameObject);
    }
}
