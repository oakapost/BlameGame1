using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Management")]
    public GameSceneManager gameSceneManager;
    
    /// <summary>
    /// Called when New Game button is clicked
    /// </summary>
    public void OnNewGameButtonClicked()
    {
        StartNewGame();
    }
    
    /// <summary>
    /// Called when Quit button is clicked
    /// </summary>
    public void OnQuitButtonClicked()
    {
        gameSceneManager.Quit();
    }
    
    /// <summary>
    /// Starts a new game by loading the NewGame scene
    /// </summary>
    private void StartNewGame()
    {
        // Find GameSceneManager if not assigned
        if (gameSceneManager == null)
        {
            gameSceneManager = FindFirstObjectByType<GameSceneManager>();
        }
        
        if (gameSceneManager != null)
        {
            gameSceneManager.LoadNewGame();
        }
    }
}