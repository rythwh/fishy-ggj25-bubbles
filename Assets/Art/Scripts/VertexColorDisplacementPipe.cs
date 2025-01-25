using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VertexColorDisplacementPipe : MonoBehaviour
{
    public float displacementStrength = 0.2f;  // Maximum displacement strength (height of the bounce)
    public float displacementSpeed = 1f;      // Speed of the up and down movement
    public Vector3 displacementDirection = Vector3.up; // Direction of displacement (e.g., Y-axis)

    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private Color[] vertexColors;

    void Start()
    {
        // Get the MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        
        // Get the mesh from the MeshFilter
        originalMesh = meshFilter.sharedMesh;
        
        originalVertices = originalMesh.vertices;
        vertexColors = originalMesh.colors;

        // Ensure the mesh has vertex colors
        if (vertexColors == null || vertexColors.Length == 0)
        {
            Debug.LogError("The mesh does not have vertex colors. Please assign vertex colors to the mesh.");
            return;
        }
    }

    void Update()
    {
        if (vertexColors == null || vertexColors.Length == 0) return;

        // Get the current time to create the bounce effect
        float time = Time.time * displacementSpeed;

        // Create a copy of the original vertices to apply displacement
        Vector3[] displacedVertices = new Vector3[originalVertices.Length];

        // Loop through all the vertices and apply displacement
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 originalPosition = originalVertices[i];

            // Get the red channel value from the vertex color (0 to 1 range)
            float redChannel = vertexColors[i].r;

            // Apply an up and down displacement based on the red channel, without filtering out vertices
            float bounceDisplacement = Mathf.PingPong(time, 1) * displacementStrength * redChannel;

            // Apply the displacement in the specified direction (e.g., Y-axis)
            displacedVertices[i] = originalPosition + bounceDisplacement * displacementDirection;
        }

        // Apply the displaced vertices back to the mesh
        originalMesh.vertices = displacedVertices;

        // Recalculate normals and bounds after updating vertices
        originalMesh.RecalculateNormals();
        originalMesh.RecalculateBounds();
    }
}
