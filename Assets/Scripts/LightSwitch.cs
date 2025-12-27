using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Светильники")]
    public LightSource[] controlledLights;
    
    [Header("Звуки выключателя")]
    public AudioClip switchSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;
    
    [Header("Визуализация")]
    public Renderer switchRenderer;
    public int materialIndex = 0;
    public Color onColor = Color.green;
    public Color offColor = Color.red;
    
    private bool lightsOn = true;
    private AudioSource audioSource;
    private Material switchMaterial;
    private InteractableObject interactable;

    void Start()
    {
        interactable = GetComponent<InteractableObject>();
        if (interactable != null)
        {
            interactable.showThoughtText = false;
            interactable.onInteract.AddListener(ToggleLights);
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && switchSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
        
        if (switchRenderer != null)
        {
            Material[] materials = switchRenderer.materials;
            if (materialIndex < materials.Length)
            {
                switchMaterial = materials[materialIndex];
                UpdateSwitchColor();
            }
        }
        
        lightsOn = true;
        foreach (var light in controlledLights)
        {
            if (light != null)
            {
                lightsOn = light.isOnByDefault;
                break;
            }
        }
    }

    void ToggleLights()
    {
        lightsOn = !lightsOn;
        
        foreach (var light in controlledLights)
        {
            if (light != null)
            {
                if (lightsOn)
                {
                    light.TurnOn();
                }
                else
                {
                    light.TurnOff();
                }
            }
        }
        
        if (switchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(switchSound, soundVolume);
        }
        
        UpdateSwitchColor();
    }

    void UpdateSwitchColor()
    {
        if (switchMaterial != null)
        {
            switchMaterial.SetColor("_EmissionColor", lightsOn ? onColor : offColor);
            switchMaterial.EnableKeyword("_EMISSION");
        }
    }
}

