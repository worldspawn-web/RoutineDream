using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI")]
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;
    
    [Header("Аудио")]
    public AudioSource dialogueAudioSource;
    
    private DialogueNode currentNode;
    private bool isPlaying = false;
    private static DialogueSystem instance;

    void Awake()
    {
        instance = this;
        
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }

    public static void StartDialogue(DialogueNode startNode, AudioSource audioSource = null)
    {
        if (instance != null)
        {
            instance.BeginDialogue(startNode, audioSource);
        }
    }

    void BeginDialogue(DialogueNode startNode, AudioSource audioSource)
    {
        if (isPlaying)
        {
            Debug.LogWarning("Диалог уже идёт!");
            return;
        }
        
        Debug.Log("DialogueSystem: Начинаю диалог");
        
        isPlaying = true;
        currentNode = startNode;
        
        if (audioSource != null)
        {
            dialogueAudioSource = audioSource;
            Debug.Log($"AudioSource получен. SpatialBlend: {audioSource.spatialBlend}");
        }
        else
        {
            Debug.LogError("AudioSource не передан в BeginDialogue!");
        }
        
        InteractionBlocker.BlockInteractions(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        PlayCurrentNode();
    }

    void PlayCurrentNode()
    {
        Debug.Log($"PlayCurrentNode вызван. CurrentNode: {currentNode?.name ?? "null"}");
        
        if (currentNode == null)
        {
            Debug.LogError("Current Node = null! Завершаю диалог.");
            EndDialogue();
            return;
        }

        if (currentNode.enemyLineAudio != null)
        {
            Debug.Log($"Проигрываю аудио: {currentNode.enemyLineAudio.name}, длина: {currentNode.enemyLineAudio.length}s");
            
            if (dialogueAudioSource == null)
            {
                Debug.LogError("dialogueAudioSource = null!");
                return;
            }
            
            dialogueAudioSource.loop = false;
            dialogueAudioSource.clip = currentNode.enemyLineAudio;
            dialogueAudioSource.Play();
            
            Debug.Log($"AudioSource.isPlaying: {dialogueAudioSource.isPlaying}, loop: {dialogueAudioSource.loop}");
            
            StartCoroutine(WaitForAudioEnd());
        }
        else
        {
            Debug.LogWarning("Enemy Line Audio отсутствует! Показываю выборы сразу.");
            ShowChoices();
        }
    }

    IEnumerator WaitForAudioEnd()
    {
        Debug.Log("Корутина запущена. Жду окончания аудио...");
        
        float timer = 0f;
        while (dialogueAudioSource.isPlaying)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                Debug.Log($"Ожидание... isPlaying: {dialogueAudioSource.isPlaying}, time: {dialogueAudioSource.time:F2}/{dialogueAudioSource.clip.length:F2}, loop: {dialogueAudioSource.loop}");
                timer = 0f;
            }
            yield return null;
        }
        
        Debug.Log($"Аудио закончилось. End Node: {currentNode.isEndNode}, Choices: {currentNode.choices?.Length ?? 0}");
        
        if (currentNode.isEndNode || currentNode.choices == null || currentNode.choices.Length == 0)
        {
            EndDialogue();
        }
        else
        {
            ShowChoices();
        }
    }

    void ShowChoices()
    {
        if (currentNode.choices == null || currentNode.choices.Length == 0)
        {
            Debug.LogWarning("Нет вариантов ответа! Завершаю диалог.");
            EndDialogue();
            return;
        }

        Debug.Log($"Показываю {currentNode.choices.Length} вариантов ответа");
        
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Choice Panel не назначен в DialogueSystem!");
            return;
        }
        
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentNode.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = currentNode.choices[i].choiceText;
                
                int index = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
                
                Debug.Log($"Вариант {i}: {currentNode.choices[i].choiceText}");
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnChoiceSelected(int choiceIndex)
    {
        choicePanel.SetActive(false);
        
        var choice = currentNode.choices[choiceIndex];
        
        if (choice.playerResponseAudio != null)
        {
            dialogueAudioSource.clip = choice.playerResponseAudio;
            dialogueAudioSource.Play();
            
            StartCoroutine(WaitForPlayerResponse(choice));
        }
        else
        {
            currentNode = choice.nextNode;
            PlayCurrentNode();
        }
    }

    IEnumerator WaitForPlayerResponse(DialogueNode.Choice choice)
    {
        yield return new WaitWhile(() => dialogueAudioSource.isPlaying);
        
        currentNode = choice.nextNode;
        PlayCurrentNode();
    }

    void EndDialogue()
    {
        isPlaying = false;
        choicePanel.SetActive(false);
        InteractionBlocker.BlockInteractions(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Диалог завершён");
    }
}

