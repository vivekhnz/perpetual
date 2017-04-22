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
        // spawn enemy
        var enemy = Enemy.Fetch<EnemyController>();
        enemy.Initialize(transform.position);

        // reset spawn cooldown
        EnemiesToSpawn--;
        spawnTime = Time.time;

        // kill spawner if no more enemies need to be spawned
        if (EnemiesToSpawn <= 0)
            DeleteSpawn();
    }

    void DeleteSpawn()
    {
        Manager.RemoveSpawnPoint(gameObject);
        Destroy(gameObject);
    }
}
