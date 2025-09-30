using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    [Header("Dialogue")]
    public TextAsset inkJSON; // Drag your "New Test" ink file here

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

    public void StartNewGame()
    {
        // Initialize game state here
        Debug.Log("New Game Started!");
        uiManager.ShowGameScreen();
        DialougeManager.GetInstance().EnterDialougeMode(inkJSON);
    }
}
