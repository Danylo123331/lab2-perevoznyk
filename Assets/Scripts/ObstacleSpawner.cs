using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public float baseSpawnInterval = 2f;
    public float minSpawnInterval = 0.5f;
    public float spawnX = 15f;
    public float minY = -3.5f;
    public float maxY = 3.5f;

    private float timer;

    void Start()
    {
        timer = baseSpawnInterval;
    }

    void Update()
    {
        float currentInterval = baseSpawnInterval;
        if (GameManager.instance != null)
        {
            currentInterval = baseSpawnInterval / GameManager.instance.gameSpeedMultiplier;
            if (currentInterval < minSpawnInterval)
            {
                currentInterval = minSpawnInterval;
            }
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnObstacle();
            timer = currentInterval;
        }
    }

    void SpawnObstacle()
    {
        if (obstacles == null || obstacles.Length == 0) return;
        int randomIndex = Random.Range(0, obstacles.Length);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(spawnX, randomY, 0f);
        Instantiate(obstacles[randomIndex], spawnPos, Quaternion.identity);
    }
}