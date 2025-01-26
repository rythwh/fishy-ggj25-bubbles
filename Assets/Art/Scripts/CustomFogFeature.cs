using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomFogFeature : ScriptableRendererFeature
{
    // Create a class to define the pass
    class CustomFogPass : ScriptableRenderPass
    {
        public Material fogMaterial;  // Expose this to assign the material in the Inspector

        public CustomFogPass()
        {
            // Set the pass to run after rendering skybox
            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (fogMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Custom Fog");

            // Blit the fog material over the screen
            cmd.Blit(null, BuiltinRenderTextureType.CurrentActive, fogMaterial);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    CustomFogPass fogPass;

    // Expose the fog material to the Inspector
    public Material fogMaterial;

    public override void Create()
    {
        // Initialize the fog pass
        fogPass = new CustomFogPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (fogMaterial != null)
        {
            // Assign the material to the fog pass
            fogPass.fogMaterial = fogMaterial;

            // Add the fog pass to the render pipeline
            renderer.EnqueuePass(fogPass);
        }
    }
}
