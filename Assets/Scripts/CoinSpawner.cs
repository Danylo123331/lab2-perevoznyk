using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public float spawnRate = 5f;
    public float minY = -2f;
    public float maxY = 2f;
    public float spawnChance = 0.5f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            timer = 0f;

            if (Random.value <= spawnChance)
            {
                SpawnCoin();
            }
        }
    }

    void SpawnCoin()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0f);

        Collider2D hit = Physics2D.OverlapBox(spawnPos, new Vector2(6f, 4f), 0f);
        if (hit != null && hit.CompareTag("Obstacle"))
        {
            return;
        }

        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }
}