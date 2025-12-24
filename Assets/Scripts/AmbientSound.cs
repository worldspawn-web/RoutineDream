using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientSound : MonoBehaviour
{
    [Header("Основные настройки")]
    public AudioClip soundClip;
    public bool playOnStart = true;
    
    [Header("Повтор")]
    public bool loop = true;
    public int maxPlayCount = -1;
    public float delayBetweenLoops = 0f;
    
    [Header("Пространственные настройки")]
    public bool is3DSound = true;
    public float minDistance = 1f;
    public float maxDistance = 50f;
    
    [Header("Громкость")]
    [Range(0f, 1f)]
    public float volume = 1f;
    
    private AudioSource audioSource;
    private int currentPlayCount = 0;
    private bool isEnabled = true;
    private float nextPlayTime = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("AudioListener не найден! Добавь AudioListener на Main Camera");
            return;
        }
        
        if (soundClip == null)
        {
            Debug.LogError($"AmbientSound [{gameObject.name}]: Sound Clip не назначен!");
            return;
        }
        
        audioSource.clip = soundClip;
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.priority = 128;
        
        if (is3DSound)
        {
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            
            float distance = Vector3.Distance(transform.position, listener.transform.position);
            Debug.Log($"AmbientSound [{gameObject.name}]: Расстояние до слушателя: {distance:F2}m (Max: {maxDistance}m)");
        }
        else
        {
            audioSource.spatialBlend = 0f;
        }
        
        Debug.Log($"AmbientSound [{gameObject.name}]: Инициализирован. Volume: {volume}, 3D: {is3DSound}, Clip: {soundClip.name}");
        
        if (playOnStart && isEnabled)
        {
            Invoke("Play", 0.1f);
        }
    }

    void Update()
    {
        if (!isEnabled || soundClip == null)
            return;

        if (!audioSource.isPlaying && Time.time >= nextPlayTime)
        {
            if (loop)
            {
                if (maxPlayCount == -1 || currentPlayCount < maxPlayCount)
                {
                    Play();
                }
            }
        }
    }

    public void Play()
    {
        if (!isEnabled || soundClip == null)
        {
            Debug.LogWarning($"AmbientSound [{gameObject.name}]: Не могу играть. Enabled: {isEnabled}, Clip: {soundClip != null}");
            return;
        }

        audioSource.Play();
        currentPlayCount++;
        nextPlayTime = Time.time + soundClip.length + delayBetweenLoops;
        
        Debug.Log($"AmbientSound [{gameObject.name}]: Играет звук. IsPlaying: {audioSource.isPlaying}, Volume: {audioSource.volume}");
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void Enable()
    {
        isEnabled = true;
        if (!audioSource.isPlaying)
        {
            currentPlayCount = 0;
            Play();
        }
    }

    public void Disable()
    {
        isEnabled = false;
        Stop();
    }

    public void Toggle()
    {
        if (isEnabled)
        {
            Disable();
        }
        else
        {
            Enable();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (is3DSound)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, minDistance);
            
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }
    }
}

