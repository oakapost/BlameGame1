using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Scene Management")]
    public GameSceneManager gameSceneManager;

    private void Awake()
    {
        // Set up button click detection for MainMenu scene
        SetupMainMenuButtons();
    }
    
    private void OnTestNewGameButtonClicked()
    {
        gameSceneManager.LoadNewGame();
    }
    
    private void SetupMainMenuButtons()
    {
        // Only set up in MainMenu scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu")
            return;
        
        // Set up New Game button
        SetupButtonClickDetector("NewGameButton");
        
        // Set up Quit button (try common names)
        if (!SetupButtonClickDetector("QuitButton"))
        {
            if (!SetupButtonClickDetector("Quit"))
            {
                if (!SetupButtonClickDetector("ExitButton"))
                {
                    SetupButtonClickDetector("Exit");
                }
            }
        }
    }
    
    private bool SetupButtonClickDetector(string buttonName)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj != null)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                // Add ClickDetector component to handle button clicks
                // (workaround for Button OnClick issues after scene transitions)
                var clickDetector = button.gameObject.GetComponent<ClickDetector>();
                if (clickDetector == null)
                {
                    clickDetector = button.gameObject.AddComponent<ClickDetector>();
                }
                clickDetector.gameManager = this;
                return true;
            }
        }
        return false;
    }
    
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