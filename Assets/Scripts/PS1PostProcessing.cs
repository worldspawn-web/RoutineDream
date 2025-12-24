using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PS1PostProcessing : MonoBehaviour
{
    [Header("PS1 Эффекты")]
    [Range(64, 1024)]
    public int pixelationSize = 320;
    
    [Range(0.0f, 1.0f)]
    public float ditherStrength = 0.05f;
    
    public bool enableColorBanding = true;
    [Range(8, 256)]
    public int colorSteps = 32;

    private Material postProcessMaterial;
    private RenderTexture pixelatedRT;

    void Start()
    {
        Shader shader = Shader.Find("Hidden/PS1PostProcess");
        if (shader != null)
        {
            postProcessMaterial = new Material(shader);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postProcessMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        int height = Mathf.RoundToInt(pixelationSize * 0.75f); // 4:3 соотношение
        if (pixelatedRT == null || pixelatedRT.width != pixelationSize || pixelatedRT.height != height)
        {
            if (pixelatedRT != null)
                pixelatedRT.Release();
            
            pixelatedRT = new RenderTexture(pixelationSize, height, 0);
            pixelatedRT.filterMode = FilterMode.Point;
        }

        Graphics.Blit(source, pixelatedRT);
        
        postProcessMaterial.SetFloat("_DitherStrength", ditherStrength);
        postProcessMaterial.SetFloat("_ColorSteps", colorSteps);
        postProcessMaterial.SetInt("_EnableColorBanding", enableColorBanding ? 1 : 0);
        
        Graphics.Blit(pixelatedRT, destination, postProcessMaterial);
    }

    void OnDestroy()
    {
        if (pixelatedRT != null)
            pixelatedRT.Release();
    }
}

