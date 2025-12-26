using UnityEngine;

public class PhoneController : MonoBehaviour
{
    public enum PhoneState
    {
        Idle,
        Ringing,
        Talking,
        Ended
    }

    [Header("Настройки телефона")]
    public float delayBeforeRing = 15f;
    public GameObject phoneModel;
    
    [Header("Звуки")]
    public AudioClip ringingSound;
    public AudioClip pickupSound;
    public AudioClip hangupSound;
    
    [Header("Диалог")]
    public DialogueNode startDialogueNode;
    
    [Header("Настройки звонка")]
    public bool loopRinging = true;
    [Range(0f, 1f)]
    public float ringingVolume = 1f;
    
    private PhoneState currentState = PhoneState.Idle;
    private AudioSource audioSource;
    private InteractableObject interactable;
    private float timer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.volume = ringingVolume;
        
        interactable = GetComponent<InteractableObject>();
        if (interactable != null)
        {
            interactable.canInteract = false;
            interactable.onInteract.AddListener(PickupPhone);
        }
        
        if (phoneModel == null)
        {
            phoneModel = transform.GetChild(0).gameObject;
        }
        
        if (phoneModel != null)
        {
            phoneModel.SetActive(true);
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case PhoneState.Idle:
                timer += Time.deltaTime;
                if (timer >= delayBeforeRing)
                {
                    StartRinging();
                }
                break;
                
            case PhoneState.Ringing:
                if (loopRinging && !audioSource.isPlaying && ringingSound != null)
                {
                    audioSource.PlayOneShot(ringingSound);
                }
                break;
        }
    }

    void StartRinging()
    {
        currentState = PhoneState.Ringing;
        
        if (ringingSound != null)
        {
            audioSource.clip = ringingSound;
            audioSource.loop = loopRinging;
            audioSource.Play();
        }
        
        if (interactable != null)
        {
            interactable.canInteract = true;
        }
        
        Debug.Log("Телефон звонит!");
    }

    void PickupPhone()
    {
        if (currentState != PhoneState.Ringing) return;
        
        currentState = PhoneState.Talking;
        
        audioSource.Stop();
        
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
        
        if (phoneModel != null)
        {
            phoneModel.SetActive(false);
        }
        
        if (interactable != null)
        {
            interactable.canInteract = false;
        }
        
        if (startDialogueNode != null)
        {
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
            
            Invoke("StartDialogue", pickupSound != null ? pickupSound.length : 0.5f);
        }
        
        Debug.Log("Трубка поднята");
    }

    void StartDialogue()
    {
        Debug.Log("Начинаю диалог...");
        
        if (startDialogueNode == null)
        {
            Debug.LogError("Start Dialogue Node не назначен!");
            return;
        }
        
        DialogueSystem dialogueSystem = FindObjectOfType<DialogueSystem>();
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem не найден в сцене!");
            return;
        }
        
        DialogueSystem.StartDialogue(startDialogueNode, audioSource);
    }

    public void EndCall()
    {
        currentState = PhoneState.Ended;
        
        if (hangupSound != null)
        {
            audioSource.PlayOneShot(hangupSound);
        }
        
        Debug.Log("Звонок завершён");
    }
}

