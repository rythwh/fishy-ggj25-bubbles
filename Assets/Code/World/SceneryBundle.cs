using System.Collections.Generic;
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
			Vector3 previousRockPosition = Vector3.zero;
			float previousRockSize = 0;
			Vector3 startPosition = Random.insideUnitSphere * WorldManager.ChunkSize;
			for (int i = 0; i < RockCount; i++) {
				float rockSize = Random.Range(RockSizeMin, RockSizeMax);
				GameObject rock = Instantiate(rockPrefab, transform, false);
				rock.transform.position += i == 0 ? startPosition : previousRockPosition + (Vector3.one * ((previousRockSize + rockSize) * Random.Range(-1, 1)));
				rock.transform.localScale = Vector3.one * rockSize;
				rock.transform.rotation = Random.rotation;
				previousRockPosition = rock.transform.position;
			}
			// Vector3 previousRockSize = Vector3.one * RockSizeMax;
			// Vector3 previousRockPosition = Vector3.zero;
			// for (int i = 0; i < RockCount; i++) {
			//
			// 	int randomBaseSize = Random.Range(RockSizeMin, RockSizeMax);
			// 	Vector3 previousRockInterior = Vector3.one * (randomBaseSize + Random.Range(-2, 2));
			// 	Vector3 aroundPreviousRockPosition = (Random.insideUnitCircle * previousRockPosition);
			//
			// 	Vector3 rockPosition = previousRockInterior + aroundPreviousRockPosition;
			// 	Vector3 rockScale = Vector3.one * randomBaseSize;
			// 	GameObject rock = Instantiate(rockPrefab, transform, false);
			// 	rock.transform.localPosition = rockPosition;
			// 	rock.transform.localScale = rockScale;
			// 	rock.transform.rotation = Random.rotation;
			//
			// 	previousRockSize = rock.transform.localScale;
			// 	previousRockPosition = rock.transform.position;
			// }
		}
	}
}