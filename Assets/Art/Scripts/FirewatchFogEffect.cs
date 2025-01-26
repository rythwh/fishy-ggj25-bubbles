using UnityEngine;

[ExecuteInEditMode] // Make sure it runs in the editor, too
public class FirewatchFogEffect : MonoBehaviour
{
    public Material fogMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Log to see if it's being called
        Debug.Log("OnRenderImage is called.");

        if (fogMaterial != null)
        {
            Debug.Log("Fog material applied.");
            Graphics.Blit(src, dest, fogMaterial);
        }
        else
        {
            Debug.Log("No fog material assigned.");
            Graphics.Blit(src, dest);
        }
    }
}
