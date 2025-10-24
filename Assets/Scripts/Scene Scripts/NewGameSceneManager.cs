using UnityEngine;
using UnityEngine.Android;

public class NewGameSceneManager : MonoBehaviour
{
    public DialogueManager dialogueManager;
    
    [Header("Dialogue")]
    public TextAsset Introduction; //the very start of the visual novel

    private void Awake()
    {
        Debug.Log($"NewGameSceneManager Awake() called on GameObject: {gameObject.name}");
    }

    private void Start()
    {
        Debug.Log($"NewGameSceneManager Start() called on GameObject: {gameObject.name}");
        // Use coroutine to ensure all managers are initialized
        StartCoroutine(StartDialogueAfterDelay());
    }
    
    private System.Collections.IEnumerator StartDialogueAfterDelay()
    {
        // Wait one frame to ensure all Awake/Start methods have completed
        yield return null;
        
        Debug.Log("Starting dialogue after delay...");
        
        // Check if all required components are ready
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager is null!");
            yield break;
        }
        
        if (Introduction == null)
        {
            Debug.LogError("Introduction TextAsset is null!");
            yield break;
        }
        
        Debug.Log("All components ready, starting dialogue...");
        dialogueManager.EnterDialogueMode(Introduction);
        
        // Check if InputHandler is working
        yield return new WaitForSeconds(0.5f); // Wait a bit for everything to settle
        
        var inputHandler = InputHandler.GetInstance();
        if (inputHandler == null)
        {
            Debug.LogError("InputHandler instance is null!");
        }
        else
        {
            Debug.Log("InputHandler found and ready");
        }
    }
}
