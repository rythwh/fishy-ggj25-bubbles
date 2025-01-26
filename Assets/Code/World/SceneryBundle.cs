using UnityEngine;

namespace Fishy.World
{
    public class SceneryBundle : MonoBehaviour
    {
        private const int RockCount = 10;

        [SerializeField] private GameObject[] rockPrefabs;

        public void Generate(int seed) {
            Random.State previousState = Random.state;
            Random.InitState(seed);
            GenerateRocks();
            Random.state = previousState;
        }

        private void GenerateRocks() {
            Vector3 startPosition = (Random.insideUnitSphere * WorldManager.ChunkSize) + (transform.position);
            for (int i = 0; i < RockCount; i++) {
                
                GameObject rockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                
                GameObject rock = Instantiate(rockPrefab, transform, false);
                Vector3 rockPosition = i == 0
                    ? startPosition
                    : new Vector3(
                        startPosition.x + Random.Range(-WorldManager.ChunkSize, WorldManager.ChunkSize),
                        0,
                        startPosition.z + Random.Range(-WorldManager.ChunkSize, WorldManager.ChunkSize)
                    );
                rock.transform.localPosition = rockPosition;
                rock.transform.localScale = Vector3.one;
                rock.transform.rotation = Random.rotation;
            }
        }
    }
}