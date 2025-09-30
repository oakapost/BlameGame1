using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject areYouSureMMPanel;

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        pauseScreen.SetActive(false);
        gameScreen.SetActive(false);
        areYouSureMMPanel.SetActive(false);
    }

    public void HideMainMenu()
    {
        mainMenu.SetActive(false);
        pauseScreen.SetActive(false);
    }

    public void ShowGameScreen()
    {
        gameScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }

    public void ShowPauseScreen()
    {
        pauseScreen.SetActive(true);
    }

    public void HidePauseScreen()
    {
        pauseScreen.SetActive(false);
    }

    public void ShowAreYouSureMMPanel()
    {
        areYouSureMMPanel.SetActive(true);
    }

    public void HideAreYouSureMMPanel()
    {
        areYouSureMMPanel.SetActive(false);
    }
    
}
