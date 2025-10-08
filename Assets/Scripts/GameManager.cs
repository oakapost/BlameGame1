using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    [Header("Dialogue")]
    public TextAsset IntroInk; // intro ink file

    public void OnNewGameButtonClicked()
    {
        uiManager.HideMainMenu();
        StartNewGame();
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnContinueButtonClicked()
    {
        uiManager.HidePauseScreen();
    }

    public void OnPauseButtonClicked()
    {
        uiManager.ShowPauseScreen();
    }

    public void OnMainMenuButtonClicked()
    {
        uiManager.ShowAreYouSureMMPanel();
    }

    public void OnAreYouSureMMYesButtonClicked()
    {
        uiManager.ShowMainMenu();
    }

    public void OnAreYouSureMMNoButtonClicked()
    {
        uiManager.HideAreYouSureMMPanel();
    }

    public void OnOptionsButtonClicked()
    {
        uiManager.ShowOptionsScreen();
    }

    public void OnExitOptionsButtonClicked()
    {
        uiManager.HideOptionsScreen();
    }

    public void OnSettingsButtonClicked()
    {
        uiManager.ShowSettingsPanel();
    }

    public void OnControlsButtonClicked()
    {
        uiManager.ShowControlsPanel();
    }

    public void OnAboutButtonClicked()
    {
        uiManager.ShowAboutPanel();
    }

    public void StartNewGame()
    {
        // Initialize game state here
        Debug.Log("New Game Started!");
        uiManager.ShowGameScreen();
        DialougeManager.GetInstance().EnterDialougeMode(IntroInk);
    }
}
