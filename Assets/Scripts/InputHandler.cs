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
        var uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null) return;

        // Priority 1: If options screen is active, just close it
        if (uiManager.optionsScreen != null && uiManager.optionsScreen.activeSelf)
        {
            uiManager.HideOptionsScreen();
            return;
        }

        // Priority 2: If areYouSureMMPanel is active, just close it
        if (uiManager.areYouSureMMPanel != null && uiManager.areYouSureMMPanel.activeSelf)
        {
            uiManager.HideAreYouSureMMPanel();
            return;
        }

        // Priority 3: Don't allow pausing if main menu is active
        if (IsMainMenuActive())
        {
            return;
        }

        // Priority 4: Toggle pause state
        // Check if pause screen is already active
        if (uiManager.pauseScreen.activeSelf)
        {
            // If paused, unpause
            uiManager.HidePauseScreen();
        }
        else
        {
            // If not paused, pause
            uiManager.ShowPauseScreen();
        }
    }

    private bool IsMainMenuActive()
    {
        var uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null && uiManager.mainMenu != null)
        {
            return uiManager.mainMenu.activeSelf;
        }
        return false;
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
