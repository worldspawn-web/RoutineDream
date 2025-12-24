using UnityEngine;

public class InteractableSound : MonoBehaviour
{
    [Header("Звуки для управления")]
    public AmbientSound[] soundsToToggle;
    
    private InteractableObject interactable;

    void Start()
    {
        interactable = GetComponent<InteractableObject>();
        if (interactable != null)
        {
            interactable.onInteract.AddListener(OnInteract);
        }
    }

    void OnInteract()
    {
        foreach (var sound in soundsToToggle)
        {
            if (sound != null)
            {
                sound.Toggle();
            }
        }
    }
}

