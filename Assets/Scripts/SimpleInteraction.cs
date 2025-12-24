using UnityEngine;

public class SimpleInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    
    private Camera playerCamera;
    private InteractableObject currentInteractable;
    private InteractableObject lastInteractable;

    void Start()
    {
        FirstPersonController fpc = GetComponent<FirstPersonController>();
        if (fpc != null)
        {
            playerCamera = fpc.playerCamera;
        }
        else
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        CheckForInteractable();
        
        if (Input.GetKeyDown(interactKey) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void CheckForInteractable()
    {
        RaycastHit hit;
        InteractableObject newInteractable = null;
        
        if (Physics.Raycast(playerCamera.transform.position, 
                           playerCamera.transform.forward, 
                           out hit, 
                           interactRange))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            
            if (interactable != null && interactable.canInteract)
            {
                newInteractable = interactable;
            }
        }
        
        if (newInteractable != lastInteractable)
        {
            if (lastInteractable != null)
            {
                lastInteractable.SetHighlight(false);
            }
            
            if (newInteractable != null)
            {
                newInteractable.SetHighlight(true);
            }
            
            lastInteractable = newInteractable;
        }
        
        currentInteractable = newInteractable;
        
        Crosshair.SetRotating(currentInteractable != null);
    }

    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(playerCamera.transform.position, 
                          playerCamera.transform.forward * interactRange);
        }
    }
}


