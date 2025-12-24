using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
    [Header("Настройки контура")]
    public Color outlineColor = Color.yellow;
    [Range(0f, 0.1f)]
    public float outlineWidth = 0.03f;
    
    private Material outlineMaterial;
    private Renderer rend;
    private Material[] originalMaterials;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalMaterials = rend.materials;
        
        Shader outlineShader = Shader.Find("Custom/Outline");
        if (outlineShader != null)
        {
            outlineMaterial = new Material(outlineShader);
            outlineMaterial.SetColor("_OutlineColor", outlineColor);
            outlineMaterial.SetFloat("_OutlineWidth", outlineWidth);
        }
        
        enabled = false;
    }

    void OnEnable()
    {
        if (rend != null && outlineMaterial != null && originalMaterials != null)
        {
            Material[] newMaterials = new Material[originalMaterials.Length + 1];
            for (int i = 0; i < originalMaterials.Length; i++)
            {
                newMaterials[i] = originalMaterials[i];
            }
            newMaterials[originalMaterials.Length] = outlineMaterial;
            rend.materials = newMaterials;
        }
    }

    void OnDisable()
    {
        if (rend != null && originalMaterials != null)
        {
            rend.materials = originalMaterials;
        }
    }

    void OnDestroy()
    {
        if (outlineMaterial != null)
        {
            Destroy(outlineMaterial);
        }
    }
}

