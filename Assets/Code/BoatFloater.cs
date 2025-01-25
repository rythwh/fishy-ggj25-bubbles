using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatFloater : MonoBehaviour
{
    public Material waterMaterial; // Reference to the water shader material
    private Rigidbody rb;

    [Header("Wave Settings")]
    [SerializeField] private float waterLevel = 0f; // Base water level
    [SerializeField] private float boatSpeed = 5f; // How fast the boat adjusts to waves
    [SerializeField] private float floatOffset = 0.2f; // Small offset to keep boat above water

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        Vector3 position = transform.position;
        
        // Get wave properties from shader
        float waveSpeed = waterMaterial.GetFloat("_WaveSpeed");
        float waveHeight = waterMaterial.GetFloat("_WaveHeight");
        float waveFreq = waterMaterial.GetFloat("_WaveFrequency");

        // Calculate waves using the same formula as in the shader
        float wave1 = Mathf.Sin(position.x * waveFreq + Time.time * waveSpeed);
        float wave2 = Mathf.Sin(position.z * waveFreq + Time.time * waveSpeed);
        
        // Calculate target height including waves and small offset
        float targetHeight = waterLevel + ((wave1 + wave2) * waveHeight) + floatOffset;
        
        // Calculate the difference between current and target height
        float heightDifference = targetHeight - position.y;
        
        // Apply force to move towards target height
        Vector3 force = Vector3.up * heightDifference * boatSpeed;
        rb.AddForce(force, ForceMode.Acceleration);

        // Rotate boat to match wave angle
        float waveAngleX = Mathf.Cos(position.x * waveFreq + Time.time * waveSpeed) * waveHeight * waveFreq;
        float waveAngleZ = Mathf.Cos(position.z * waveFreq + Time.time * waveSpeed) * waveHeight * waveFreq;
        
        Quaternion targetRotation = Quaternion.Euler(-waveAngleZ * Mathf.Rad2Deg, transform.rotation.eulerAngles.y, waveAngleX * Mathf.Rad2Deg);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * boatSpeed);
    }
} 