using UnityEngine;

namespace Fishy.World
{
	public class SceneryBundle : MonoBehaviour
	{
		private const int RockCount = 10;
		private const float RockSizeMin = 1;
		private const float RockSizeMax = 20;

		[SerializeField] private GameObject rockPrefab;

		public void Generate(int seed) {
			Random.State previousState = Random.state;
			Random.InitState(seed);
			GenerateRocks();
			Random.state = previousState;
		}

		private void GenerateRocks() {
			Vector3 startPosition = (Random.insideUnitSphere * WorldManager.ChunkSize) + (transform.position);
			for (int i = 0; i < RockCount; i++) {
				float rockSize = Random.Range(RockSizeMin, RockSizeMax);
				GameObject rock = Instantiate(rockPrefab, transform, false);
				Vector3 rockPosition = i == 0
					? startPosition
					: new Vector3(
						startPosition.x + Random.Range(-WorldManager.ChunkSize, WorldManager.ChunkSize),
						0,
						startPosition.z + Random.Range(-WorldManager.ChunkSize, WorldManager.ChunkSize)
					);
				rock.transform.localPosition = rockPosition;
				rock.transform.localScale = Vector3.one * rockSize;
				rock.transform.rotation = Random.rotation;
			}
		}
	}
}