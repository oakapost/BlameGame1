
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public DialogueManager dialogueManager;


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

}
