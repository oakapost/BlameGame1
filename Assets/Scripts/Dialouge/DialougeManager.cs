using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialougeManager : MonoBehaviour
{
    [Header("Dialouge UI")] 
    [SerializeField] private GameObject Textbox;
    [SerializeField] private TextMeshProUGUI dialougeText;
    private static DialougeManager instance;
    private Story currentStory;
    private bool dialougeIsPlaying;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one DialougeManager in scene.");
        }
        instance = this;
    }

    public static DialougeManager GetInstance()
    {
        return instance;
    }
    
    private void Update()
    {
        if (!dialougeIsPlaying)
        {
            return;
        }
        
        if (InputHandler.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    public void EnterDialougeMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialougeIsPlaying = true;
        GameModeManager.GetInstance().SetMode(GameModeManager.GameMode.VisualNovel);
        ContinueStory();
    }

    private void ExitDialougeMode()
    {
        dialougeIsPlaying = false;
        GameModeManager.GetInstance().SetMode(GameModeManager.GameMode.Walk);
        dialougeText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialougeText.text = currentStory.Continue();
        }
        else
        {
            ExitDialougeMode();
        }
    }
}
