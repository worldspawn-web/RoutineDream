using UnityEngine;
using System.Collections;

public class LightSource : MonoBehaviour
{
    public enum FlickerMode
    {
        Normal,
        Occasional,
        Frequent,
        Broken
    }

    [Header("Компоненты")]
    public Light lightComponent;
    public Renderer lampRenderer;
    public int emissiveMaterialIndex = 0;
    
    [Header("Настройки света")]
    public bool isOnByDefault = true;
    public float normalIntensity = 1f;
    public Color emissiveColor = Color.white;
    [Range(0f, 10f)]
    public float emissiveIntensity = 2f;
    
    [Header("Режим мерцания")]
    public FlickerMode flickerMode = FlickerMode.Normal;
    [Range(0f, 1f)]
    public float flickerStrength = 0.3f;
    
    [Header("Звуки")]
    public AudioClip turnOnSound;
    public AudioClip turnOffSound;
    public AudioClip flickerSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;
    
    private bool isOn;
    private AudioSource audioSource;
    private Material emissiveMaterial;
    private float targetIntensity;
    private bool isFlickering = false;

    void Start()
    {
        if (lightComponent == null)
        {
            lightComponent = GetComponentInChildren<Light>();
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (turnOnSound != null || turnOffSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
        
        if (lampRenderer != null)
        {
            Material[] materials = lampRenderer.materials;
            if (emissiveMaterialIndex < materials.Length)
            {
                emissiveMaterial = materials[emissiveMaterialIndex];
            }
        }
        
        isOn = isOnByDefault;
        SetLightState(isOn, true);
        
        if (isOn && flickerMode != FlickerMode.Normal)
        {
            StartFlickering();
        }
    }

    void Update()
    {
        if (isOn && !isFlickering)
        {
            lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, targetIntensity, Time.deltaTime * 5f);
        }
    }

    public void Toggle()
    {
        SetLightState(!isOn);
    }

    public void TurnOn()
    {
        SetLightState(true);
    }

    public void TurnOff()
    {
        SetLightState(false);
    }

    void SetLightState(bool state, bool instant = false)
    {
        isOn = state;
        targetIntensity = isOn ? normalIntensity : 0f;
        
        if (instant)
        {
            lightComponent.intensity = targetIntensity;
        }
        
        if (lightComponent != null)
        {
            lightComponent.enabled = isOn;
        }
        
        UpdateEmissive(isOn);
        
        if (audioSource != null)
        {
            AudioClip clip = isOn ? turnOnSound : turnOffSound;
            if (clip != null)
            {
                audioSource.PlayOneShot(clip, soundVolume);
            }
        }
        
        if (isOn && flickerMode != FlickerMode.Normal)
        {
            StartFlickering();
        }
        else
        {
            StopAllCoroutines();
            isFlickering = false;
        }
    }

    void UpdateEmissive(bool glowing)
    {
        if (emissiveMaterial == null) return;
        
        if (glowing)
        {
            emissiveMaterial.EnableKeyword("_EMISSION");
            emissiveMaterial.SetColor("_EmissionColor", emissiveColor * emissiveIntensity);
        }
        else
        {
            emissiveMaterial.SetColor("_EmissionColor", Color.black);
        }
    }

    void StartFlickering()
    {
        if (isFlickering) return;
        
        isFlickering = true;
        
        switch (flickerMode)
        {
            case FlickerMode.Occasional:
                StartCoroutine(OccasionalFlicker());
                break;
            case FlickerMode.Frequent:
                StartCoroutine(FrequentFlicker());
                break;
            case FlickerMode.Broken:
                StartCoroutine(BrokenFlicker());
                break;
        }
    }

    IEnumerator OccasionalFlicker()
    {
        while (isOn)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f));
            
            if (isOn)
            {
                yield return StartCoroutine(FlickerEffect(1));
            }
        }
    }

    IEnumerator FrequentFlicker()
    {
        while (isOn)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            
            if (isOn)
            {
                yield return StartCoroutine(FlickerEffect(Random.Range(1, 3)));
            }
        }
    }

    IEnumerator BrokenFlicker()
    {
        while (isOn)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            
            if (isOn)
            {
                yield return StartCoroutine(FlickerEffect(1));
            }
        }
    }

    IEnumerator FlickerEffect(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float flickerIntensity = normalIntensity * (1f - flickerStrength);
            lightComponent.intensity = flickerIntensity;
            UpdateEmissive(false);
            
            if (flickerSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(flickerSound, soundVolume * 0.5f);
            }
            
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            
            lightComponent.intensity = normalIntensity;
            UpdateEmissive(true);
            
            if (i < count - 1)
            {
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            }
        }
    }
}

