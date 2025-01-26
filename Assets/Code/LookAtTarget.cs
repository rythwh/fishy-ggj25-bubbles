using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool smoothRotation = true;

    [Header("Axis Lock")]
    [SerializeField] private bool lockXAxis = false;
    [SerializeField] private bool lockYAxis = false;
    [SerializeField] private bool lockZAxis = false;

    private void Update()
    {
        if (target == null) return;

        // Get the direction to the target
        Vector3 directionToTarget = target.position - transform.position;

        // Lock axes by zeroing out components
        if (lockXAxis) directionToTarget.x = 0;
        if (lockYAxis) directionToTarget.y = 0;
        if (lockZAxis) directionToTarget.z = 0;

        // Only proceed if we have a direction to look at
        if (directionToTarget != Vector3.zero)
        {
            // Calculate the target rotation
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // If any axis is locked, preserve the original rotation for that axis
            if (lockXAxis || lockYAxis || lockZAxis)
            {
                Vector3 currentEuler = transform.rotation.eulerAngles;
                Vector3 targetEuler = targetRotation.eulerAngles;

                if (lockXAxis) targetEuler.x = currentEuler.x;
                if (lockYAxis) targetEuler.y = currentEuler.y;
                if (lockZAxis) targetEuler.z = currentEuler.z;

                targetRotation = Quaternion.Euler(targetEuler);
            }

            if (smoothRotation)
            {
                // Smoothly rotate towards the target
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            else
            {
                // Instantly face the target
                transform.rotation = targetRotation;
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
} 