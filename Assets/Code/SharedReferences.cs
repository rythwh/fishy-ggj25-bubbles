using UnityEngine;

namespace Fishy
{
	public class SharedReferences : MonoBehaviour
	{
		[SerializeField] private Transform canvas;
		public Transform Canvas => canvas;
	}
}