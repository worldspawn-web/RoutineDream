using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public string objectName = "Объект";
    [TextArea(3, 10)]
    public string interactionText = "Это интересный объект для размышлений...";
    
    public bool canInteract = true;
    public float interactionDistance = 3f;
    
    [Header("События")]
    public UnityEvent onInteract;
    
    private bool isPlayerNearby = false;

    public void Interact()
    {
        if (!canInteract) return;
        
        Debug.Log($"[{objectName}] {interactionText}");
        
        // Показываем текст на экране
        UIThoughtDisplay.ShowThought(interactionText);
        
        onInteract?.Invoke();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}

