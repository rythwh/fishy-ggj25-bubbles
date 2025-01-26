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
		private Vector3Int playerChunk = new Vector3Int(int.MaxValue, int.MaxValue);
		public const int ChunkSize = 64;
		private const int ChunkDistance = 4;
		private readonly Dictionary<Vector3Int, SceneryBundle> sceneryBundles = new();

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
			Vector3Int currentPlayerChunk = new Vector3Int(
				Mathf.FloorToInt(playerPosition.x / ChunkSize),
				0,
				Mathf.FloorToInt(playerPosition.z / ChunkSize)
			);
			if (currentPlayerChunk != playerChunk) {
				playerChunk = currentPlayerChunk;
				Vector3Int playerChunkMin = playerChunk - (Vector3Int.one * (ChunkDistance));
				Vector3Int playerChunkMax = playerChunk + (Vector3Int.one * (ChunkDistance));
				for (int x = playerChunkMin.x; x < playerChunkMax.x; x++) {
					for (int z = playerChunkMin.z; z < playerChunkMax.z; z++) {
						Vector3Int chunkPosition = new Vector3Int(x, 0, z);
						if ((x == playerChunkMin.x || x == playerChunkMax.x - 1) || (z == playerChunkMin.z || z == playerChunkMax.z - 1)) {
							if (sceneryBundles.TryGetValue(chunkPosition, out SceneryBundle sceneryBundle)) {
								Object.Destroy(sceneryBundle.gameObject);
								sceneryBundles.Remove(chunkPosition);
							}
						} else {
							if (sceneryBundles.TryGetValue(chunkPosition, out SceneryBundle _)) {
								continue;
							}
							Vector3Int spawnPositionInt = chunkPosition * ChunkSize;
							Vector3 spawnPosition = new Vector3(spawnPositionInt.x, 0, spawnPositionInt.z);
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