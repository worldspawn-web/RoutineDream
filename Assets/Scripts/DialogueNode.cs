using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    [System.Serializable]
    public class Choice
    {
        [TextArea(2, 4)]
        public string choiceText;
        public AudioClip playerResponseAudio;
        public DialogueNode nextNode;
    }

    [Header("Реплика собеседника")]
    public AudioClip enemyLineAudio;
    
    [Header("Варианты ответа игрока")]
    public Choice[] choices;
    
    [Header("Настройки")]
    public bool isEndNode = false;
}

