
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public DialogueManager dialogueManager;
    [Header("Dialogue")]
    public TextAsset IntroInk; // intro ink file

    public void LoadMainMenu()
    {
        Debug.Log("Loading MainMenu scene");
        // Use LoadScene with Single mode to completely unload previous scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void LoadNewGame()
    {
        Debug.Log("LoadNewGame called - attempting to load NewGame scene");
        // Use regular LoadScene instead of async to ensure clean loading
        UnityEngine.SceneManagement.SceneManager.LoadScene("NewGame");
    }
    
    public void Quit()
    { 
        Application.Quit();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    /*
    private System.Collections.IEnumerator LoadNewGameCoroutine()
    {
        // Load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("NewGame");

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Wait one more frame to ensure everything is initialized
        yield return null;

        // Now start the dialogue
        if (DialogueManager.GetInstance() != null)
        {
            DialogueManager.GetInstance().EnterDialogueMode(IntroInk);
        }
        else
        {
            Debug.LogError("DialogueManager not found in the loaded scene!");
        }
    }
    */

}
