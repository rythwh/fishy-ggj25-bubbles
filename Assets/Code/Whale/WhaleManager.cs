using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fishy.NState;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Fishy.Whale
{
	public class WhaleManager : IManager
	{
		private bool whalesCanSpawn = false;
		private float whaleSpawnTimer = 0;
		private const float WhaleSpawnInterval = 10f;
		private const int WhaleSpawnAreaSize = 100;
		public const float WhaleDepth = -10;

		private readonly List<Whale> whales = new List<Whale>();

		public void OnCreate() {
			GameManager.Get<StateManager>().OnStateChanged += OnStateChanged;
		}

		private void OnStateChanged((EState previousState, EState newState) state) {
			if (state.previousState is EState.LoadToGame && state.newState is EState.Game) {
				whalesCanSpawn = true;
			}
		}

		public void OnUpdate() {
			if (whalesCanSpawn) {
				UpdateWhaleSpawnTimer();
			}
		}

		private void UpdateWhaleSpawnTimer() {
			whaleSpawnTimer += Time.deltaTime;
			if (whaleSpawnTimer < WhaleSpawnInterval) {
				return;
			}
			SpawnWhales().Forget();
			whaleSpawnTimer = 0;
		}

		private async UniTaskVoid SpawnWhales() {
			AsyncOperationHandle<GameObject> whalePrefabAsyncOperationHandle = Addressables.LoadAssetAsync<GameObject>("Prefabs/Whale");
			GameObject whalePrefab = await whalePrefabAsyncOperationHandle;
			Whale whale = Object.Instantiate(whalePrefab, GetWhaleSpawnPosition(Vector2.zero), Quaternion.identity).GetComponent<Whale>();
			whales.Add(whale);
			whalePrefabAsyncOperationHandle.Release();
		}

		private Vector3 GetWhaleSpawnPosition(Vector2 playerPosition) {
			Vector2 spawnLocation = Random.insideUnitCircle * WhaleSpawnAreaSize + playerPosition;
			return new Vector3(spawnLocation.x, -10, spawnLocation.y);
		}
	}
}