using UnityEngine;

public class BoatFloater : MonoBehaviour
{
    public Material waterMaterial; // Reference to the water shader material

    [Header("Wave Settings")]
    [SerializeField] private float waterLevel = 0f; // Base water level
    [SerializeField] private float floatSpeed = 5f; // How fast the boat adjusts to waves
    [SerializeField] private float floatOffset = 0.2f; // Small offset to keep boat above water
    [SerializeField] private float rotationSpeed = 5f; // How fast the boat rotates to match waves
    [SerializeField] private float waveAmplifier = 2f; // Amplifies the wave effect
    [SerializeField] private float rotationAmplifier = 45f; // Amplifies the rotation effect

    private void Update()
    {
        Vector3 position = transform.position;
        
        // Get wave properties from shader
        float waveSpeed = waterMaterial.GetFloat("_WaveSpeed");
        float waveHeight = waterMaterial.GetFloat("_WaveHeight");
        float waveFreq = waterMaterial.GetFloat("_WaveFrequency");

        // Calculate waves using the same formula as in the shader but amplified
        float wave1 = Mathf.Sin(position.x * waveFreq + Time.time * waveSpeed) * waveAmplifier;
        float wave2 = Mathf.Sin(position.z * waveFreq + Time.time * waveSpeed) * waveAmplifier;
        
        // Calculate target height including waves and small offset
        float targetHeight = waterLevel + ((wave1 + wave2) * waveHeight) + floatOffset;
        
        // Direct position update for more noticeable movement
        position.y = Mathf.Lerp(position.y, targetHeight, floatSpeed * Time.deltaTime);
        transform.position = position;

        // Rotate boat to match wave angle with amplified effect
        float waveAngleX = Mathf.Cos(position.x * waveFreq + Time.time * waveSpeed) * waveHeight * waveFreq * rotationAmplifier;
        float waveAngleZ = Mathf.Cos(position.z * waveFreq + Time.time * waveSpeed) * waveHeight * waveFreq * rotationAmplifier;
        
        Vector3 targetEuler = new Vector3(-waveAngleZ, transform.rotation.eulerAngles.y, waveAngleX);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetEuler), rotationSpeed * Time.deltaTime);
    }
} 