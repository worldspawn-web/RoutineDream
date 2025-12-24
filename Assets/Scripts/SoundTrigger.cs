using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SoundTrigger : MonoBehaviour
{
    [Header("Настройки")]
    public AmbientSound[] soundsToControl;
    public bool enableOnEnter = true;
    public bool disableOnExit = false;
    public bool triggerOnce = false;
    
    private bool hasTriggered = false;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnce && hasTriggered)
            return;

        if (enableOnEnter)
        {
            foreach (var sound in soundsToControl)
            {
                if (sound != null)
                {
                    sound.Enable();
                }
            }
        }

        hasTriggered = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (disableOnExit)
        {
            foreach (var sound in soundsToControl)
            {
                if (sound != null)
                {
                    sound.Disable();
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        
        BoxCollider boxCol = GetComponent<BoxCollider>();
        if (boxCol != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCol.center, boxCol.size);
        }
        
        SphereCollider sphereCol = GetComponent<SphereCollider>();
        if (sphereCol != null)
        {
            Gizmos.DrawSphere(transform.position, sphereCol.radius);
        }
    }
}

