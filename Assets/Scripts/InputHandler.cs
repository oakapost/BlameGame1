using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private static InputHandler instance;
    private bool inputEnabled = true; 

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one InputHandler in scene.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static InputHandler GetInstance()
    {
        return instance;
    }

    private void Update()
    {
        // Check for esc key which will pause
        if (GetPausePressed())
        {
            HandlePauseInput();
        }
    }

    private void HandlePauseInput()
    {
        // Priority 0: Don't allow pausing if we're in MainMenu scene
        if (IsMainMenuActive())
        {
            Debug.Log("Pause disabled in MainMenu scene");
            return;
        }
        
        var pauseManager = FindFirstObjectByType<PauseManager>(FindObjectsInactive.Include);
        var optionsManager = FindFirstObjectByType<OptionsManager>(FindObjectsInactive.Include);
        

        
        if (pauseManager == null) 
        {
            Debug.Log("No PauseManager found in scene - pause disabled");
            return;
        }
        
        // Check if PauseManager has valid UI references
        Debug.Log($"PauseManager found: {pauseManager.gameObject.name}");
        Debug.Log($"PauseScreen reference: {(pauseManager.pauseScreen != null ? pauseManager.pauseScreen.name : "NULL")}");
        Debug.Log($"PauseScreen active in hierarchy: {(pauseManager.pauseScreen != null ? pauseManager.pauseScreen.activeInHierarchy.ToString() : "N/A")}");
        
        if (pauseManager.pauseScreen == null)
        {
            Debug.LogError($"PauseManager found in {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} scene but pauseScreen is not assigned! Please assign it in the Inspector.");
            return;
        }

        // Priority 1: If any options sub-panel is active, close it and return to main options
        if (optionsManager != null && optionsManager.optionsScreen != null && optionsManager.optionsScreen.activeSelf)
        {
            // Check if any sub-panel is active
            bool anySubPanelActive = false;
            
            if (optionsManager.settingsPanel != null && optionsManager.settingsPanel.activeSelf)
            {
                optionsManager.settingsPanel.SetActive(false);
                anySubPanelActive = true;
            }
            else if (optionsManager.controlsPanel != null && optionsManager.controlsPanel.activeSelf)
            {
                optionsManager.controlsPanel.SetActive(false);
                anySubPanelActive = true;
            }
            else if (optionsManager.aboutPanel != null && optionsManager.aboutPanel.activeSelf)
            {
                optionsManager.aboutPanel.SetActive(false);
                anySubPanelActive = true;
            }
            
            // If no sub-panel was active, close the entire options screen
            if (!anySubPanelActive)
            {
                optionsManager.OnExitOptionsButtonClicked();
            }
            return;
        }

        // Priority 2: If areYouSureMMPanel is active, just close it
        if (pauseManager.areYouSureMMPanel != null && pauseManager.areYouSureMMPanel.activeSelf)
        {
            pauseManager.HideAreYouSureMMPanel();
            return;
        }

        // Priority 3: Don't allow pausing if main menu is active
        if (IsMainMenuActive())
        {
            return;
        }

        // Priority 4: Toggle pause state
        // Check if pause screen is already active
        if (pauseManager.pauseScreen != null && pauseManager.pauseScreen.activeSelf)
        {
            // If paused, unpause
            pauseManager.HidePauseScreen();
        }
        else
        {
            // If not paused, pause (with additional null check)
            if (pauseManager.pauseScreen != null)
            {
                pauseManager.ShowPauseScreen();
            }
            else
            {
                Debug.LogError("Cannot show pause screen - pauseScreen is null!");
            }
        }
    }

    private bool IsMainMenuActive()
    {
        // Check if we're in the MainMenu scene
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu";
    }

    /// <summary>
    /// Enable input handling
    /// </summary>
    public void EnableInput()
    {
        inputEnabled = true;
        Debug.Log("Input enabled");
    }

    /// <summary>
    /// Disable input handling
    /// </summary>
    public void DisableInput()
    {
        inputEnabled = false;
        Debug.Log("Input disabled");
    }

    /// <summary>
    /// Check if input is currently enabled
    /// </summary>
    public bool IsInputEnabled()
    {
        return inputEnabled;
    }

    /// <summary>
    /// Returns true when ESC key is pressed for pausing
    /// Works in all modes except when main menu is active
    /// </summary>
    public bool GetPausePressed()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return false;

        return keyboard.escapeKey.wasPressedThisFrame;
    }

    /// <summary>
    /// Returns true when Enter, Spacebar, or Left Mouse Click is pressed this frame
    /// Only works in Visual Novel mode and when input is enabled
    /// </summary>
    public bool GetSubmitPressed()
    {
        // Check if input is disabled
        if (!inputEnabled)
        {
            return false;
        }

        // Only allow input if we're in Visual Novel mode
        if (GameModeManager.GetInstance() == null || 
            GameModeManager.GetInstance().currentMode != GameModeManager.GameMode.VisualNovel)
        {
            return false;
        }

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (keyboard == null && mouse == null) return false;

        bool keyPressed = false;
        bool mousePressed = false;

        if (keyboard != null)
        {
            keyPressed = keyboard.enterKey.wasPressedThisFrame || 
                        keyboard.numpadEnterKey.wasPressedThisFrame || 
                        keyboard.spaceKey.wasPressedThisFrame;
        }

        if (mouse != null)
        {
            mousePressed = mouse.leftButton.wasPressedThisFrame;
        }

        return keyPressed || mousePressed;
    }

    /// <summary>
    /// Returns true while Enter, Spacebar, or Left Mouse Button is being held down
    /// Only works in Visual Novel mode and when input is enabled
    /// </summary>
    public bool GetSubmitHeld()
    {
        // Check if input is disabled
        if (!inputEnabled)
        {
            return false;
        }

        // Only allow input if we're in Visual Novel mode
        if (GameModeManager.GetInstance() == null || 
            GameModeManager.GetInstance().currentMode != GameModeManager.GameMode.VisualNovel)
        {
            return false;
        }

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (keyboard == null && mouse == null) return false;

        bool keyHeld = false;
        bool mouseHeld = false;

        if (keyboard != null)
        {
            keyHeld = keyboard.enterKey.isPressed || 
                     keyboard.numpadEnterKey.isPressed || 
                     keyboard.spaceKey.isPressed;
        }

        if (mouse != null)
        {
            mouseHeld = mouse.leftButton.isPressed;
        }

        return keyHeld || mouseHeld;
    }

    /// <summary>
    /// Returns true when Escape key is pressed (for general canceling - pausing is handled automatically)
    /// </summary>
    public bool GetCancelPressed()
    {
        var keyboard = Keyboard.current;
        return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
    }

    /// <summary>
    /// Helper method to check if we're in Visual Novel mode
    /// </summary>
    public bool IsInVisualNovelMode()
    {
        return GameModeManager.GetInstance() != null && 
               GameModeManager.GetInstance().currentMode == GameModeManager.GameMode.VisualNovel;
    }
}
