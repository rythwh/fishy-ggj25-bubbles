using UnityEngine;

namespace Fishy.Whale
{
	public class Whale : MonoBehaviour
	{
		private const float MaxLifetime = 100f;
		private float currentLifetime = 0;
		private const float MaxMoveSpeed = 5f;

		private void Update() {
			UpdatePosition();
			UpdateLifetime();
		}

		private void UpdatePosition() {
			transform.position = Vector3.MoveTowards(
				transform.position,
				GameManager.SharedReferences.PlayerTransform.position,
				Time.deltaTime * MaxMoveSpeed
			);
			transform.position = new Vector3(transform.position.x, WhaleManager.WhaleDepth, transform.position.z);
		}

		private void UpdateLifetime() {
			currentLifetime += Time.deltaTime;
			if (currentLifetime < MaxLifetime) {
				return;
			}
			currentLifetime = 0;
			Destroy(gameObject);
		}
	}
}