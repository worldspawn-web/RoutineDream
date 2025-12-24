using UnityEngine;
using System.Collections;

public class ThoughtTrigger : MonoBehaviour
{
    [Header("Настройки мысли")]
    [TextArea(3, 10)]
    public string thoughtText = "Здесь должна быть философская мысль...";
    
    [Tooltip("Показывать только один раз?")]
    public bool triggerOnce = true;
    
    [Tooltip("Задержка перед показом текста")]
    public float delayBeforeShow = 0.5f;
    
    [Header("Визуальные эффекты")]
    public bool fadeWorldOnThought = false;
    public float fadeAmount = 0.5f;
    
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered)
                return;
            
            StartCoroutine(ShowThought());
            hasTriggered = true;
        }
    }

    IEnumerator ShowThought()
    {
        yield return new WaitForSeconds(delayBeforeShow);
        
        Debug.Log($"💭 {thoughtText}");
        
        UIThoughtDisplay.ShowThought(thoughtText);
        
        if (fadeWorldOnThought)
        {
            // TODO: сюда добавить эффекты
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0f, 1f, 0.3f);
        
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

