using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject fishPrefab;
    
    [Header("Spawn Radius Settings")]
    [SerializeField] private int spawnRadius = 10;
    
    [Header("Spawn Time Settings")]
    [SerializeField] private int minTimeBetweenSpawns = 5;
    [SerializeField] private int maxTimeBetweenSpawns = 60;

    [Header("Lifetime Settings")]
    [SerializeField] private int minLifetime = 10;
    [SerializeField] private int maxLifetime = 90;

    private int maximumFishParticlesAtOnce = 5;
    private int currentNumberOfFish = 0;
    private List<GameObject> objectPool = new();
    private float elapsedTime = 0f;
    
    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }
    
    private IEnumerator SpawnRoutine()
    {
        float randomLifetime = Random.Range(minLifetime, maxLifetime);
        while (elapsedTime < randomLifetime)
        {
            elapsedTime += Time.deltaTime;
            if (currentNumberOfFish < maximumFishParticlesAtOnce)
            {
                float randomSpawnDelay = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
                yield return new WaitForSeconds(randomSpawnDelay);

                if (elapsedTime <= randomLifetime)
                {
                    GameObject obj = GetPooledObject();
                    obj.transform.position = GetRandomPositionAroundPlayer();
                    obj.SetActive(true);
                    currentNumberOfFish++;
                    float randomFishLifetime = Random.Range(minLifetime, maxLifetime);
                    StartCoroutine(DeactivateObjectAfterTime(obj, randomFishLifetime));
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
        gameObject.SetActive(false);
    }

    private IEnumerator DeactivateObjectAfterTime(GameObject obj, float time)
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
