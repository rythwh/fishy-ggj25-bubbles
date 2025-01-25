using System.Collections.Generic;
using Fishy.NState;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fishy.World
{
	public class WorldManager : IManager
	{
		private Vector3 playerPosition;
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
				playerPosition = Vector3.zero;
				GenerateWorld();
			}
		}

		private void OnPlayerMoved(Vector3 playerPosition) {
			this.playerPosition = playerPosition;
			GenerateWorld();
		}

		private void GenerateWorld() {
			Vector2Int currentPlayerChunk = new Vector2Int(
				Mathf.FloorToInt(playerPosition.x / ChunkSize),
				Mathf.FloorToInt(playerPosition.z / ChunkSize)
			);
			if (currentPlayerChunk != playerChunk) {
				playerChunk = currentPlayerChunk;
				Vector2Int playerChunkMin = playerChunk - (Vector2Int.one * (ChunkDistance + 1));
				Vector2Int playerChunkMax = playerChunk + (Vector2Int.one * (ChunkDistance + 1));
				for (int x = playerChunkMin.x; x < playerChunkMax.x; x++) {
					for (int y = playerChunkMin.y; y < playerChunkMax.y; y++) {
						Vector2Int chunkPosition = new Vector2Int(x, y);
						if ((x == playerChunkMin.x || x == playerChunkMax.x - 1) && (y == playerChunkMin.y || y == playerChunkMax.y - 1)) {
							if (sceneryBundles.TryGetValue(chunkPosition, out SceneryBundle sceneryBundle)) {
								Object.Destroy(sceneryBundle.gameObject);
								sceneryBundles.Remove(chunkPosition);
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