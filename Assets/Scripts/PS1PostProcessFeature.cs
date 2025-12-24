using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PS1PostProcessFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        [Range(64, 1024)]
        public int pixelationSize = 320;
        
        [Range(0.0f, 1.0f)]
        public float ditherStrength = 0.05f;
        
        public bool enableColorBanding = true;
        
        [Range(8, 256)]
        public int colorSteps = 32;
    }

    public Settings settings = new Settings();
    private PS1Pass renderPass;

    public override void Create()
    {
        renderPass = new PS1Pass(settings);
        renderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderPass.Setup(settings);
        renderer.EnqueuePass(renderPass);
    }

    class PS1Pass : ScriptableRenderPass
    {
        private Material material;
        private Settings settings;
        private RenderTargetIdentifier currentTarget;

        public PS1Pass(Settings settings)
        {
            this.settings = settings;
            Shader shader = Shader.Find("Hidden/PS1PostProcess");
            if (shader != null)
            {
                material = new Material(shader);
            }
        }

        public void Setup(Settings newSettings)
        {
            settings = newSettings;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            currentTarget = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("PS1PostProcess");

            int width = renderingData.cameraData.camera.scaledPixelWidth;
            int height = renderingData.cameraData.camera.scaledPixelHeight;
            int pixelatedHeight = Mathf.RoundToInt(settings.pixelationSize * 0.75f);

            int sourceID = Shader.PropertyToID("_SourceTex");
            int pixelatedID = Shader.PropertyToID("_PixelatedTex");
            int tempID = Shader.PropertyToID("_TempTex");

            cmd.GetTemporaryRT(sourceID, width, height, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(pixelatedID, settings.pixelationSize, pixelatedHeight, 0, FilterMode.Point);
            cmd.GetTemporaryRT(tempID, width, height, 0, FilterMode.Bilinear);

            cmd.Blit(currentTarget, sourceID);
            cmd.Blit(sourceID, pixelatedID);

            material.SetFloat("_DitherStrength", settings.ditherStrength);
            material.SetFloat("_ColorSteps", settings.colorSteps);
            material.SetInt("_EnableColorBanding", settings.enableColorBanding ? 1 : 0);

            cmd.Blit(pixelatedID, tempID, material);
            cmd.Blit(tempID, currentTarget);

            cmd.ReleaseTemporaryRT(sourceID);
            cmd.ReleaseTemporaryRT(pixelatedID);
            cmd.ReleaseTemporaryRT(tempID);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}

