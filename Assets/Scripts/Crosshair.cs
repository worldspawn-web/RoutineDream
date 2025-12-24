using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("Настройки")]
    public float rotationSpeed = 100f;
    
    private bool isRotating = false;
    private static Crosshair instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }
    }

    public static void SetRotating(bool rotating)
    {
        if (instance != null)
        {
            instance.isRotating = rotating;
        }
    }
}

