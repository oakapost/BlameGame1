using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public GameObject optionsScreen;
    public GameObject settingsPanel;
    public GameObject controlsPanel;
    public GameObject aboutPanel;

    public void OnOptionsButtonClicked()
    {
        optionsScreen.SetActive(true);

        // Default
        OnSettingsButtonClicked();
    }
    
    public void OnExitOptionsButtonClicked()
    {
        optionsScreen.SetActive(false);
    }

    public void OnSettingsButtonClicked()
    {
        settingsPanel.SetActive(true);
        controlsPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    public void OnControlsButtonClicked()
    {
        controlsPanel.SetActive(true);
        settingsPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    public void OnAboutButtonClicked()
    {
        aboutPanel.SetActive(true);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }
}
