using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject fishPrefab;
    
    [Header("Spawn Radius Settings")]
    [SerializeField] private int spawnRadius = 10;
    
    [Header("Spawn Time Settings")]
    [SerializeField] private int minTimeBetweenSpawns = 15;
    [SerializeField] private int maxTimeBetweenSpawns = 60;

    [Header("Lifetime Settings")]
    [SerializeField] private int minLifetime = 10;
    [SerializeField] private int maxLifetime = 90;

    private List<GameObject> objectPool = new();
    private float elapsedTime = 0f;


private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        float randomLifetime = Random.Range(minLifetime, maxLifetime);
        float randomSpawnDelay = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
        while (elapsedTime < randomLifetime)
        {
            // Increment elapsed time by the time between spawns
            // float spawnDelay = Random.Range(minSpawnTime, maxSpawnTime + 1);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(randomSpawnDelay);

            if (elapsedTime <= randomLifetime)
            {
                GameObject obj = GetPooledObject();
                obj.transform.position = GetRandomPositionAroundPlayer();
                obj.SetActive(true);
                // float lifetime = Random.Range(minLifetime, maxLifetime + 1);
                // StartCoroutine(DeactivateAfterTime(obj, lifetime));
            }
        }
        gameObject.SetActive(false);
    }

    private IEnumerator DeactivateAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    private GameObject GetPooledObject()
    {
        foreach (var pooledObj in objectPool)
        {
            if (!pooledObj.activeInHierarchy)
            {
                return pooledObj;
            }
        }

        GameObject newObj = Instantiate(fishPrefab);
        objectPool.Add(newObj);
        return newObj;
    }

    private Vector3 GetRandomPositionAroundPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            playerTransform.position.x + randomCircle.x,
            playerTransform.position.y,
            playerTransform.position.z + randomCircle.y
        );
    }
}
