using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fishy.NState;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Fishy.World
{
	public class WorldManager : IManager
	{
		private Vector2 playerPosition;
		private Vector2Int playerChunk = new Vector2Int(int.MaxValue, int.MaxValue);
		public const int ChunkSize = 64;
		private const int ChunkDistance = 4;
		private readonly Dictionary<Vector2Int, SceneryBundle> sceneryBundles = new();

		public void OnCreate() {
			GameManager.SharedReferences.Player.OnPlayerMoved += OnPlayerMoved;
			GameManager.Get<StateManager>().OnStateChanged += OnStateChanged;
		}

		private void OnStateChanged((EState previousState, EState newState) state) {
			if (state is { previousState: EState.LoadToGame, newState: EState.Game }) {
				playerPosition = Vector2.zero;
				GenerateWorld().Forget();
			}
		}

		private void OnPlayerMoved(Vector2 playerPosition) {
			this.playerPosition = playerPosition;
			GenerateWorld().Forget();
		}

		private async UniTaskVoid GenerateWorld() {
			Vector2Int currentPlayerChunk = new Vector2Int(
				Mathf.FloorToInt(playerPosition.x / ChunkSize),
				Mathf.FloorToInt(playerPosition.y / ChunkSize)
			);
			if (currentPlayerChunk != playerChunk) {
				playerChunk = currentPlayerChunk;
				for (int x = -ChunkDistance - 1; x < ChunkDistance + 1; x++) {
					for (int y = -ChunkDistance - 1; y < ChunkDistance + 1; y++) {
						Vector2Int chunkPosition = new Vector2Int(x, y);
						if (x is -ChunkDistance - 1 or ChunkDistance && y is -ChunkDistance - 1 or ChunkDistance) {
							if (sceneryBundles.TryGetValue(chunkPosition, out SceneryBundle sceneryBundle)) {
								Object.Destroy(sceneryBundle);
							}
						} else {
							if (sceneryBundles.TryGetValue(chunkPosition, out SceneryBundle _)) {
								continue;
							}
							Vector2Int spawnPositionInt = chunkPosition * ChunkSize;
							Vector3 spawnPosition = new Vector3(spawnPositionInt.x, 0, spawnPositionInt.y);
							SceneryBundle sceneryBundlePrefab = GameManager.SharedReferences.SceneryBundlePrefab;
							SceneryBundle sceneryBundle = Object.Instantiate(sceneryBundlePrefab, spawnPosition, Quaternion.identity).GetComponent<SceneryBundle>();
							sceneryBundles.Add(chunkPosition, sceneryBundle);
							sceneryBundle.Generate(HashUtility.GetHashCode(spawnPositionInt));
						}
					}
				}
			}
		}
	}
}