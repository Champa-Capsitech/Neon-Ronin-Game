using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    public float ySpawnMin = -4f;
    public float ySpawnMax = 1f;

    public float startDelay = 1f;
    public float repeatRate = 2f;

    public float spawnX = 15f;

    public Transform Camera;

    void Start()
    {
        InvokeRepeating(nameof(SpawnObstacle), startDelay, repeatRate);
    }

    void SpawnObstacle()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Running)
            return;

        if (GameManager.instance.playerBlocked)
            return; // STOP SPAWNING

        float randomY = Random.Range(ySpawnMin, ySpawnMax);
        Vector2 spawnPos = new Vector2(spawnX + Camera.position.x, randomY);

        int index = Random.Range(0, obstaclePrefabs.Length);

        Instantiate(obstaclePrefabs[index], spawnPos, Quaternion.identity);
    }

}
