using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class VertexColorDisplacement : MonoBehaviour
{
    public float displacementStrength = 0.2f;  // Maximum displacement strength
    public float displacementFrequency = 2f;   // Frequency of the sinusoidal wave (speed of tail movement)
    public Vector3 displacementDirection = Vector3.up; // Direction of displacement (e.g., Y-axis)

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private Color[] vertexColors;

    void Start()
    {
        // Get the SkinnedMeshRenderer component
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        // Get the mesh from the SkinnedMeshRenderer
        originalMesh = skinnedMeshRenderer.sharedMesh;
        
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

        // Get the current time to create the sinusoidal effect
        float time = Time.time;

        // Create a copy of the original vertices to apply displacement
        Vector3[] displacedVertices = new Vector3[originalVertices.Length];

        // Loop through all the vertices and apply displacement
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 originalPosition = originalVertices[i];

            // Get the red channel value from the vertex color (0 to 1 range)
            float redChannel = vertexColors[i].r;

            // Apply a smooth displacement based on the red channel, without filtering out vertices
            float sinusoidalDisplacement = Mathf.Sin(time + originalPosition.x * displacementFrequency) * displacementStrength * redChannel;

            // Apply the displacement in the specified direction (e.g., Y-axis)
            displacedVertices[i] = originalPosition + sinusoidalDisplacement * displacementDirection;
        }

        // Apply the displaced vertices back to the SkinnedMeshRenderer
        // First, we update the mesh's vertices with the displaced ones
        originalMesh.vertices = displacedVertices;

        // Recalculate normals and bounds after updating vertices
        originalMesh.RecalculateNormals();
        originalMesh.RecalculateBounds();

        // Set the updated mesh back to the SkinnedMeshRenderer
        skinnedMeshRenderer.sharedMesh = originalMesh;

        // Optional: If the mesh has blend shapes, ensure their values are respected
        for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            // Reset blendshape weights (if needed)
            skinnedMeshRenderer.SetBlendShapeWeight(i, skinnedMeshRenderer.GetBlendShapeWeight(i));
        }
    }
}
