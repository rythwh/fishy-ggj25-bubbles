using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    [Header("Island Settings")]
    [SerializeField] private GameObject[] islandPrefabs;
    [SerializeField] private int numberOfIslands = 10;
    [SerializeField] private float spawnRadius = 100f;
    [SerializeField] private float minDistanceBetweenIslands = 20f;
    [SerializeField] private Vector2 rotationRange = new Vector2(0f, 360f);
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float minDistanceFromPlayer = 30f;
    [SerializeField] private float islandHeightOffset = -1f; // Negative value to sink below water

    [Header("Scale Settings")]
    [SerializeField] private Vector2 scaleRangeX = new Vector2(0.5f, 2.0f);
    [SerializeField] private Vector2 scaleRangeY = new Vector2(0.5f, 1.5f);
    [SerializeField] private Vector2 scaleRangeZ = new Vector2(0.5f, 2.0f);

    private void Start()
    {
        GenerateIslands();
    }

    public void GenerateIslands()
    {
        // Clear existing islands if needed
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numberOfIslands; i++)
        {
            TrySpawnIsland(20); // Try 20 times to find a valid position
        }
    }

    private void TrySpawnIsland(int maxAttempts)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Generate random position within spawn radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomCircle.x, 0, randomCircle.y);

            // Check if position is valid (not too close to other islands)
            if (IsValidSpawnPosition(spawnPosition))
            {
                SpawnIsland(spawnPosition);
                return;
            }
        }
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        // First check distance from player
        if (playerTransform != null)
        {
            float playerDistance = Vector3.Distance(position, playerTransform.position);
            if (playerDistance < minDistanceFromPlayer)
            {
                return false;
            }
        }

        // Then check distance from other islands
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(position, child.position);
            if (distance < minDistanceBetweenIslands)
            {
                return false;
            }
        }
        return true;
    }

    private void SpawnIsland(Vector3 position)
    {
        // Select random island prefab
        GameObject prefab = islandPrefabs[Random.Range(0, islandPrefabs.Length)];
        
        // Create island with height offset
        Vector3 offsetPosition = new Vector3(position.x, islandHeightOffset, position.z);
        GameObject island = Instantiate(prefab, offsetPosition, Quaternion.identity, transform);
        
        // Random rotation
        float randomRotation = Random.Range(rotationRange.x, rotationRange.y);
        island.transform.rotation = Quaternion.Euler(0, randomRotation, 0);
        
        // Random non-uniform scale
        float scaleX = Random.Range(scaleRangeX.x, scaleRangeX.y);
        float scaleY = Random.Range(scaleRangeY.x, scaleRangeY.y);
        float scaleZ = Random.Range(scaleRangeZ.x, scaleRangeZ.y);
        island.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
} 