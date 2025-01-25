using UnityEngine;

namespace Fishy
{
	public class SharedReferences : MonoBehaviour
	{
		[SerializeField] private Transform canvas;
		public Transform Canvas => canvas;

		[SerializeField] private Transform playerTransform;
		public Transform PlayerTransform => playerTransform;
	}
}