using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject areYouSureMMPanel;
    public GameObject optionsScreen;
    public GameObject settingsPanel;
    public GameObject controlsPanel;
    public GameObject aboutPanel;

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

    public void ShowAreYouSureMMPanel()
    {
        areYouSureMMPanel.SetActive(true);
    }

    public void HideAreYouSureMMPanel()
    {
        areYouSureMMPanel.SetActive(false);
    }

    public void ShowOptionsScreen()
    {
        optionsScreen.SetActive(true);
        ShowSettingsPanel();
    }

    public void HideOptionsScreen()
    {
        optionsScreen.SetActive(false);
    }

    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
        controlsPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    public void ShowControlsPanel()
    {
        controlsPanel.SetActive(true);
        settingsPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    public void ShowAboutPanel()
    {
        aboutPanel.SetActive(true);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }


}
