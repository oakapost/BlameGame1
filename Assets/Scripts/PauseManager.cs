using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameSceneManager gameSceneManager;
    public GameObject pauseScreen;
    public GameObject areYouSureMMPanel;



    public void ShowPauseScreen()
    {
        pauseScreen.SetActive(true);
        // Disable input when pause screen is shown
        if (InputHandler.GetInstance() != null)
        {
            InputHandler.GetInstance().DisableInput();
        }
    }

    public void HidePauseScreen()
    {
        pauseScreen.SetActive(false);
        // Re-enable input when pause screen is hidden
        if (InputHandler.GetInstance() != null)
        {
            InputHandler.GetInstance().EnableInput();
        }
    }

    public void HideAreYouSureMMPanel()
    {
        areYouSureMMPanel.SetActive(false);
    }
    public void OnContinueButtonClicked()
    {
        HidePauseScreen();
    }

    public void OnPauseButtonClicked()
    {
        ShowPauseScreen();
    }

    public void OnMainMenuButtonClicked()
    {
        areYouSureMMPanel.SetActive(true);
    }

    public void OnAreYouSureMMYesButtonClicked()
    {
        areYouSureMMPanel.SetActive(false);
        
        // Clean up any persistent states before loading main menu
        CleanupBeforeSceneLoad();
        
        gameSceneManager.LoadMainMenu();
    }
    
    private void CleanupBeforeSceneLoad()
    {
        // Re-enable input if it was disabled
        if (InputHandler.GetInstance() != null)
        {
            InputHandler.GetInstance().EnableInput();
        }
        
        // Set game mode back to normal if needed
        if (GameModeManager.GetInstance() != null)
        {
            GameModeManager.GetInstance().SetMode(GameModeManager.GameMode.Walk);
        }
        
        Debug.Log("Cleanup completed before loading main menu");
    }

}
