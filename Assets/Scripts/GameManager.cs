using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;

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

    public void StartNewGame()
    {
        // Initialize game state here
        Debug.Log("New Game Started!");
        uiManager.ShowGameScreen();
    }
}
