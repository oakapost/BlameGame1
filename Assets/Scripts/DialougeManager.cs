using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea(2,5)]
        public string text;
        public Sprite portrait;
    }

    public DialogueLine[] lines;
    public Image portraitImage;
    public Text speakerText;      // Legacy Text component
    public Text dialogueText;     // Legacy Text component
    public GameObject dialoguePanel;

    private int currentLine = 0;

    void Start()
    {
        ShowLine(0);
    }

    public void NextLine()
    {
        currentLine++;
        if (currentLine < lines.Length)
        {
            ShowLine(currentLine);
        }
        else
        {
            dialoguePanel.SetActive(false); // End of dialogue
        }
    }

    void ShowLine(int index)
    {
        speakerText.text = lines[index].speaker;
        dialogueText.text = lines[index].text;
        portraitImage.sprite = lines[index].portrait;
    }
}