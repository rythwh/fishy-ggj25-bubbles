using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class AsyncVertexColorWaveDisplacement : MonoBehaviour
{
    public float waveAmplitude = 0.2f;  // Maximum height of the waves
    public float waveFrequency = 1.0f;  // Frequency of the waves
    public float waveSpeed = 1.0f;      // Speed of the waves
    public Vector3 waveDirection = Vector3.up;  // Direction of the wave displacement

    private Mesh originalMesh;
    private Mesh deformedMesh;
    private Vector3[] originalVertices;
    private Vector3[] deformedVertices;
    private Color[] vertexColors;

    private float randomOffset; // Unique offset for each instance

    void Start()
    {
        // Assign a unique random offset to make the wave effect asynchronous
        randomOffset = Random.Range(0f, 100f);

        // Get the mesh from the MeshFilter and make a copy for modification
        originalMesh = GetComponent<MeshFilter>().mesh;
        deformedMesh = Instantiate(originalMesh);
        GetComponent<MeshFilter>().mesh = deformedMesh;

        // Cache the original vertices and vertex colors
        originalVertices = originalMesh.vertices;
        deformedVertices = new Vector3[originalVertices.Length];
        vertexColors = originalMesh.colors;

        // Ensure the mesh has vertex colors
        if (vertexColors == null || vertexColors.Length == 0)
        {
            Debug.LogError("The mesh does not have vertex colors. Please assign vertex colors to the mesh.");
        }
    }

    void Update()
    {
        if (vertexColors == null || vertexColors.Length == 0) return;

        // Update the vertex positions based on vertex colors and sine wave
        float time = Time.time * waveSpeed + randomOffset; // Add unique offset for asynchronous effect
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 originalVertex = originalVertices[i];

            // Use the red channel of the vertex color to scale the displacement
            float colorInfluence = vertexColors[i].r;
            float wave = Mathf.Sin(originalVertex.x * waveFrequency + time) +
                         Mathf.Sin(originalVertex.z * waveFrequency + time);

            // Apply displacement in the specified direction
            deformedVertices[i] = originalVertex + waveDirection.normalized * wave * waveAmplitude * colorInfluence;
        }

        // Update the deformed mesh
        deformedMesh.vertices = deformedVertices;
        deformedMesh.RecalculateNormals();
        deformedMesh.RecalculateBounds();
    }
}
