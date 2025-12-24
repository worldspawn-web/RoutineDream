using UnityEngine;

[ExecuteInEditMode]
public class PictureFrame : MonoBehaviour
{
    public Material pictureMaterial;
    
    private Texture lastTexture;
    private Material instanceMaterial;

    void Start()
    {
        UpdateTextureFit();
    }

    void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            UpdateTextureFit();
        }
    }

    void UpdateTextureFit()
    {
        if (pictureMaterial == null)
        {
            Renderer rend = GetComponent<Renderer>();
            if (rend != null)
            {
                pictureMaterial = rend.sharedMaterial;
            }
        }

        if (pictureMaterial == null)
            return;

        Texture tex = pictureMaterial.mainTexture;
        
        if (tex == null || tex == lastTexture)
            return;

        lastTexture = tex;

        if (instanceMaterial == null)
        {
            Renderer rend = GetComponent<Renderer>();
            instanceMaterial = new Material(pictureMaterial);
            rend.material = instanceMaterial;
        }

        float textureAspect = (float)tex.width / (float)tex.height;
        float quadAspect = transform.localScale.x / transform.localScale.y;
        
        if (textureAspect > quadAspect)
        {
            float scale = quadAspect / textureAspect;
            instanceMaterial.mainTextureScale = new Vector2(1f, scale);
            instanceMaterial.mainTextureOffset = new Vector2(0f, (1f - scale) * 0.5f);
        }
        else
        {
            float scale = textureAspect / quadAspect;
            instanceMaterial.mainTextureScale = new Vector2(scale, 1f);
            instanceMaterial.mainTextureOffset = new Vector2((1f - scale) * 0.5f, 0f);
        }
    }

    void OnDestroy()
    {
        if (instanceMaterial != null)
        {
            DestroyImmediate(instanceMaterial);
        }
    }
}

