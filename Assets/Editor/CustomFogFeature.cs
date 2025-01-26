using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomFogFeature : ScriptableRendererFeature
{
    class CustomFogPass : ScriptableRenderPass
    {
        public Material fogMaterial;

        public CustomFogPass()
        {
            // Configure this pass to run after the opaque objects
            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (fogMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Custom Fog");

            // Set the target to the camera's color buffer
            cmd.Blit(null, BuiltinRenderTextureType.CurrentActive, fogMaterial);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    CustomFogPass fogPass;

    public override void Create()
    {
        fogPass = new CustomFogPass();
        // Here, assign the fog material (you can make this public or load it dynamically)
        fogPass.fogMaterial = Resources.Load<Material>("Materials/FogMaterial"); // Assuming your material is located in a Resources folder
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Only add the render pass if the material is assigned
        if (fogPass.fogMaterial != null)
        {
            renderer.EnqueuePass(fogPass);
        }
    }
}
