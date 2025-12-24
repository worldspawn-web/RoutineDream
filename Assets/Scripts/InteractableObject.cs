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
    public float interactionDistance = 3f;
    
    [Header("События")]
    public UnityEvent onInteract;
    
    private bool hasInteracted = false;

    public void Interact()
    {
        if (!canInteract) return;
        if (interactOnce && hasInteracted) return;
        
        Debug.Log($"[{objectName}] {interactionText}");
        
        UIThoughtDisplay.ShowThought(interactionText);
        
        onInteract?.Invoke();
        
        if (interactOnce)
        {
            hasInteracted = true;
        }
    }
    
    public void SetHighlight(bool enabled)
    {
        Outline outline = GetComponent<Outline>();
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

