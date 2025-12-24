using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIThoughtDisplay : MonoBehaviour
{
    [Header("UI Ссылки")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI thoughtText;
    
    [Header("Анимация")]
    public float fadeInDuration = 1f;
    public float displayDuration = 4f;
    public float fadeOutDuration = 1.5f;
    
    [Header("Эффекты")]
    public bool useTypewriterEffect = true;
    public float typewriterSpeed = 0.05f;
    
    private Coroutine currentDisplayCoroutine;
    private static UIThoughtDisplay instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public static void ShowThought(string text)
    {
        if (instance != null)
        {
            instance.DisplayThought(text);
        }
        else
        {
            Debug.LogWarning("UIThoughtDisplay не найден в сцене!");
        }
    }

    public void DisplayThought(string text)
    {
        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
        }
        
        currentDisplayCoroutine = StartCoroutine(DisplayThoughtCoroutine(text));
    }

    IEnumerator DisplayThoughtCoroutine(string text)
    {
        thoughtText.text = "";
        canvasGroup.alpha = 0;
        
        // Fade in
        float elapsed = 0;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
        
        // Typewriter эффект
        if (useTypewriterEffect)
        {
            for (int i = 0; i <= text.Length; i++)
            {
                thoughtText.text = text.Substring(0, i);
                yield return new WaitForSeconds(typewriterSpeed);
            }
        }
        else
        {
            thoughtText.text = text;
        }
        
        // Держим текст на экране
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        elapsed = 0;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        thoughtText.text = "";
    }

    public void HideImmediately()
    {
        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
        }
        
        canvasGroup.alpha = 0;
        thoughtText.text = "";
    }
}

