using Fishy.World;
using UnityEngine;

namespace Fishy
{
	public class SharedReferences : MonoBehaviour
	{
		[SerializeField] private Transform canvas;
		public Transform Canvas => canvas;

		[SerializeField] private Player player;
		public Player Player => player;

		[SerializeField] private SceneryBundle sceneryBundlePrefab;
		public SceneryBundle SceneryBundlePrefab => sceneryBundlePrefab;
	}
}