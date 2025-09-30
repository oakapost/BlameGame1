using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    //public PlayerController playerController; // Assign in Inspector
    public GameObject visualNovelPanel;       // Assign your VN UI panel here
    //public InkManager inkManager;             // Assign your Ink dialogue manager

    private static GameModeManager instance;
    
    public enum GameMode { Walk, VisualNovel }
    public GameMode currentMode = GameMode.VisualNovel;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one GameModeManager in scene.");
        }
        instance = this;
    }

    public static GameModeManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        SetMode(GameMode.VisualNovel);
    }

    public void SetMode(GameMode mode)
    {
        currentMode = mode;
        if (mode == GameMode.Walk)
        {
            //playerController.enabled = true;
            visualNovelPanel.SetActive(false);
        }
        else if (mode == GameMode.VisualNovel)
        {
            //playerController.enabled = false;
            visualNovelPanel.SetActive(true);
        }
    }

    // Example trigger: Call this when player interacts with something that starts dialogue
    /*
    public void StartVisualNovel(Story inkStory)
    {
        SetMode(GameMode.VisualNovel);
        inkManager.StartStory(inkStory, OnVNFinished);
    }

    */
    
    // Callback for when Ink story ends
    public void OnVNFinished()
    {
        SetMode(GameMode.Walk);
    }
}