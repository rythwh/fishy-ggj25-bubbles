using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float playerOffset;
    [SerializeField] private GameObject fishPrefab;
    
    [Header("Spawn Radius Settings")]
    [SerializeField] private int spawnRadius = 10;
    
    [Header("Spawn Time Settings")]
    [SerializeField] private int minTimeBetweenSpawns = 5;
    [SerializeField] private int maxTimeBetweenSpawns = 60;

    [Header("Fish Settings")]
    [SerializeField] private float minimumSpacingBetweenFish = 3f;
    [SerializeField] private int maximumFishCount = 5;

    private int currentNumberOfFish = 0;
    private List<GameObject> objectPool = new();
    
    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }
    
    private IEnumerator SpawnRoutine()
    {
        while (true) // Run forever
        {
            if (currentNumberOfFish < maximumFishCount)
            {
                float randomSpawnDelay = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
                yield return new WaitForSeconds(randomSpawnDelay);

                GameObject obj = GetPooledObject();
                obj.transform.position = GetRandomPositionAroundPlayer();
                obj.SetActive(true);
                currentNumberOfFish++;
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void RemoveFishingSpot(GameObject fishingSpot)
    {
        Debug.Log($"RemoveFishingSpot called for {fishingSpot.name}");
        if (fishingSpot.activeInHierarchy)
        {
            Debug.Log("Deactivating fishing spot");
            fishingSpot.SetActive(false);
            currentNumberOfFish--;
            Debug.Log($"Current number of fish: {currentNumberOfFish}");
        }
        else
        {
            Debug.Log("Fishing spot was not active in hierarchy");
        }
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
        int maxAttempts = 30; // Prevent infinite loops
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 potentialPosition = new Vector3(
                playerTransform.position.x + randomCircle.x,
                playerTransform.position.y - playerOffset,
                playerTransform.position.z + randomCircle.y
            );

            // Check if this position is far enough from all other fish
            bool isFarEnough = true;
            foreach (var fish in objectPool)
            {
                if (!fish.activeInHierarchy) continue;
                
                float distance = Vector3.Distance(fish.transform.position, potentialPosition);
                if (distance < minimumSpacingBetweenFish)
                {
                    isFarEnough = false;
                    break;
                }
            }

            if (isFarEnough)
            {
                return potentialPosition;
            }

            attempts++;
        }

        // If we couldn't find a good position after max attempts, just return the last attempted position
        Vector2 fallbackCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            playerTransform.position.x + fallbackCircle.x,
            playerTransform.position.y - playerOffset,
            playerTransform.position.z + fallbackCircle.y
        );
    }
}
