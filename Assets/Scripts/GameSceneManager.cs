
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public DialogueManager dialogueManager;


    public void LoadMainMenu()
    {
        Debug.Log("Loading MainMenu scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void LoadNewGame()
    {
        Debug.Log("LoadNewGame called - attempting to load NewGame scene");
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
