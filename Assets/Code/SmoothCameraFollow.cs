using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float rotationSmoothSpeed = 5f;
    
    private Vector3 offset;
    private Quaternion rotationOffset;

    private void Start()
    {
        // Calculate and store the initial offset between camera and target
        if (target != null)
        {
            offset = Quaternion.Inverse(target.rotation) * (transform.position - target.position);
            rotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
        }
        else
        {
            Debug.LogWarning("No target assigned to SmoothCameraFollow script!");
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired position with rotated offset
        Vector3 rotatedOffset = target.rotation * offset;
        Vector3 desiredPosition = target.position + rotatedOffset;
        
        // Smoothly interpolate position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Calculate desired rotation using the initial rotation offset
        Quaternion desiredRotation = target.rotation * rotationOffset;
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);

        // Update the camera position and rotation
        transform.position = smoothedPosition;
        transform.rotation = smoothedRotation;
    }
} 