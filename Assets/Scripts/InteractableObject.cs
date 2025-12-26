using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public string objectName = "Объект";
    [TextArea(3, 10)]
    public string interactionText = "Это интересный объект для размышлений...";
    
    public bool canInteract = true;
    public bool interactOnce = false;
    public bool showThoughtText = true;
    public float interactionDistance = 3f;
    
    [Header("Звук взаимодействия")]
    public AudioClip interactionSound;
    public bool playSoundGlobally = false;
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    
    [Header("События")]
    public UnityEvent onInteract;
    
    private bool hasInteracted = false;
    private AudioSource audioSource;

    void Start()
    {
        if (interactionSound != null && !playSoundGlobally)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
            audioSource.volume = soundVolume;
        }
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        
        Debug.Log($"[{objectName}] {interactionText}");
        
        if (showThoughtText)
        {
            UIThoughtDisplay.ShowThought(interactionText);
        }
        
        PlayInteractionSound();
        
        onInteract?.Invoke();
        
        if (interactOnce)
        {
            hasInteracted = true;
        }
    }
    
    void PlayInteractionSound()
    {
        if (interactionSound == null) return;

        if (playSoundGlobally)
        {
            AudioSource.PlayClipAtPoint(interactionSound, Camera.main.transform.position, soundVolume);
        }
        else
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(interactionSound);
            }
        }
    }
    
    public bool CanInteract()
    {
        if (!canInteract) return false;
        if (interactOnce && hasInteracted) return false;
        return true;
    }
    
    public void SetHighlight(bool enabled)
    {
        Outline outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = GetComponentInChildren<Outline>();
        }
        
        if (outline != null)
        {
            outline.enabled = enabled;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}

