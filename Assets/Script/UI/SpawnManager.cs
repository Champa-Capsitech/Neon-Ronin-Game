using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    public float ySpawnMin = -4f;
    public float ySpawnMax = 1f;

    public float spawnGap = 6f;       // distance between obstacles
    public float spawnXOffset = 15f;

    public Transform Camera;

    float distanceCounter;

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        float speed = GameManager.instance.worldSpeed;

        // 🔑 No movement → no spawning
        if (speed <= 0f)
            return;

        distanceCounter += speed * Time.deltaTime;

        if (distanceCounter >= spawnGap)
        {
            SpawnObstacle();
            distanceCounter = 0f;
        }
    }

    void SpawnObstacle()
    {
        float randomY = Random.Range(ySpawnMin, ySpawnMax);
        Vector2 spawnPos = new Vector2(Camera.position.x + spawnXOffset, randomY);

        int index = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[index], spawnPos, Quaternion.identity);
    }
}


