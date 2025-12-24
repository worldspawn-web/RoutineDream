using UnityEngine;

public class SimpleInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    
    private Camera playerCamera;
    private InteractableObject currentInteractable;

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
        
        if (Physics.Raycast(playerCamera.transform.position, 
                           playerCamera.transform.forward, 
                           out hit, 
                           interactRange))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            
            if (interactable != null && interactable.canInteract)
            {
                currentInteractable = interactable;
                return;
            }
        }
        
        currentInteractable = null;
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

