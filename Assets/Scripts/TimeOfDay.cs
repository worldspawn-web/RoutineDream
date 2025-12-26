using UnityEngine;

[ExecuteInEditMode]
public class TimeOfDay : MonoBehaviour
{
    public enum DayTime
    {
        Night,
        Morning,
        Day,
        Evening
    }

    [System.Serializable]
    public class TimePreset
    {
        public string name;
        public Material skyboxMaterial;
        public Color skyColor;
        public Color equatorColor;
        public Color fogColor;
        public float fogDensity;
        public Color lightColor;
        public float lightIntensity;
        public Color ambientColor;
    }

    [Header("Настройки")]
    public DayTime currentTime = DayTime.Night;
    public Light directionalLight;
    
    [Header("Пресеты")]
    public TimePreset[] presets;
    
    [Header("Переход")]
    public float transitionSpeed = 1f;
    
    private TimePreset targetPreset;
    private Color currentSkyColor;
    private Color currentEquatorColor;
    private Color currentFogColor;
    private float currentFogDensity;
    private Color currentLightColor;
    private float currentLightIntensity;
    private Color currentAmbientColor;

    void Start()
    {
        if (presets.Length == 0)
        {
            CreateDefaultPresets();
        }

        if (directionalLight == null)
        {
            directionalLight = FindObjectOfType<Light>();
        }

        SetTime(currentTime, true);
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (directionalLight == null)
            {
                directionalLight = FindObjectOfType<Light>();
            }
            
            SetTime(currentTime, true);
        }
    }

    void Update()
    {
        if (targetPreset != null)
        {
            float t = Time.deltaTime * transitionSpeed;
            
            currentSkyColor = Color.Lerp(currentSkyColor, targetPreset.skyColor, t);
            currentEquatorColor = Color.Lerp(currentEquatorColor, targetPreset.equatorColor, t);
            currentFogColor = Color.Lerp(currentFogColor, targetPreset.fogColor, t);
            currentFogDensity = Mathf.Lerp(currentFogDensity, targetPreset.fogDensity, t);
            currentLightColor = Color.Lerp(currentLightColor, targetPreset.lightColor, t);
            currentLightIntensity = Mathf.Lerp(currentLightIntensity, targetPreset.lightIntensity, t);
            currentAmbientColor = Color.Lerp(currentAmbientColor, targetPreset.ambientColor, t);
            
            ApplySettings();
        }
    }

    public void SetTime(DayTime time, bool instant = false)
    {
        currentTime = time;
        int index = (int)time;
        
        if (index < 0 || index >= presets.Length)
            return;

        targetPreset = presets[index];

        if (instant)
        {
            currentSkyColor = targetPreset.skyColor;
            currentEquatorColor = targetPreset.equatorColor;
            currentFogColor = targetPreset.fogColor;
            currentFogDensity = targetPreset.fogDensity;
            currentLightColor = targetPreset.lightColor;
            currentLightIntensity = targetPreset.lightIntensity;
            currentAmbientColor = targetPreset.ambientColor;
            ApplySettings();
        }
    }

    void ApplySettings()
    {
        if (targetPreset != null && targetPreset.skyboxMaterial != null)
        {
            RenderSettings.skybox = targetPreset.skyboxMaterial;
        }
        
        RenderSettings.fogColor = currentFogColor;
        RenderSettings.fogDensity = currentFogDensity;
        RenderSettings.ambientSkyColor = currentSkyColor;
        RenderSettings.ambientEquatorColor = currentEquatorColor;
        RenderSettings.ambientLight = currentAmbientColor;
        
        if (directionalLight != null)
        {
            directionalLight.color = currentLightColor;
            directionalLight.intensity = currentLightIntensity;
        }
    }

    void CreateDefaultPresets()
    {
        presets = new TimePreset[4];
        
        presets[0] = new TimePreset
        {
            name = "Night",
            skyColor = new Color(0.05f, 0.05f, 0.15f),
            equatorColor = new Color(0.1f, 0.1f, 0.2f),
            fogColor = new Color(0.1f, 0.1f, 0.2f),
            fogDensity = 0.03f,
            lightColor = new Color(0.6f, 0.6f, 0.8f),
            lightIntensity = 0.3f,
            ambientColor = new Color(0.2f, 0.2f, 0.3f)
        };
        
        presets[1] = new TimePreset
        {
            name = "Morning",
            skyColor = new Color(0.8f, 0.6f, 0.5f),
            equatorColor = new Color(0.9f, 0.7f, 0.5f),
            fogColor = new Color(0.9f, 0.8f, 0.7f),
            fogDensity = 0.02f,
            lightColor = new Color(1f, 0.9f, 0.7f),
            lightIntensity = 0.8f,
            ambientColor = new Color(0.7f, 0.6f, 0.5f)
        };
        
        presets[2] = new TimePreset
        {
            name = "Day",
            skyColor = new Color(0.5f, 0.7f, 1f),
            equatorColor = new Color(0.7f, 0.8f, 1f),
            fogColor = new Color(0.7f, 0.8f, 0.9f),
            fogDensity = 0.015f,
            lightColor = new Color(1f, 1f, 0.95f),
            lightIntensity = 1.2f,
            ambientColor = new Color(0.8f, 0.8f, 0.9f)
        };
        
        presets[3] = new TimePreset
        {
            name = "Evening",
            skyColor = new Color(0.8f, 0.4f, 0.3f),
            equatorColor = new Color(0.9f, 0.5f, 0.3f),
            fogColor = new Color(0.7f, 0.5f, 0.4f),
            fogDensity = 0.025f,
            lightColor = new Color(1f, 0.7f, 0.5f),
            lightIntensity = 0.7f,
            ambientColor = new Color(0.6f, 0.4f, 0.3f)
        };
    }
}

