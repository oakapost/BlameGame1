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
        var pauseManager = FindFirstObjectByType<PauseManager>(FindObjectsInactive.Include);
        var optionsManager = FindFirstObjectByType<OptionsManager>(FindObjectsInactive.Include);
        
        // Priority 1: Handle options menu closing FIRST (works in any scene)
        if (optionsManager != null && optionsManager.optionsScreen != null && optionsManager.optionsScreen.activeSelf)
        {
            // Close ALL sub-panels and the main options screen in one ESC press
            if (optionsManager.settingsPanel != null)
                optionsManager.settingsPanel.SetActive(false);
            
            if (optionsManager.controlsPanel != null)
                optionsManager.controlsPanel.SetActive(false);
            
            if (optionsManager.aboutPanel != null)
                optionsManager.aboutPanel.SetActive(false);
            
            // Always close the entire options screen
            optionsManager.OnExitOptionsButtonClicked();
            return;
        }
        
        // Priority 2: Don't allow pause functionality in MainMenu scene
        if (IsMainMenuActive())
        {
            return;
        }
        
        // From here on, we're in a game scene where pause is allowed
        if (pauseManager == null) 
        {
            return;
        }
        
        if (pauseManager.pauseScreen == null)
        {
            return;
        }

        // Priority 4: If areYouSureMMPanel is active, just close it
        if (pauseManager.areYouSureMMPanel != null && pauseManager.areYouSureMMPanel.activeSelf)
        {
            pauseManager.HideAreYouSureMMPanel();
            return;
        }

        // Priority 5: Toggle pause state
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
    /// Returns true when Tab key is held down (for dialogue skipping in Visual Novel mode)
    /// </summary>
    public bool GetSkipPressed()
    {
        // Only allow skip in Visual Novel mode
        if (GameModeManager.GetInstance() == null || 
            GameModeManager.GetInstance().currentMode != GameModeManager.GameMode.VisualNovel)
        {
            return false;
        }

        // Don't allow skipping if pause menu is open
        var pauseManager = FindFirstObjectByType<PauseManager>(FindObjectsInactive.Include);
        if (pauseManager != null && pauseManager.pauseScreen != null && pauseManager.pauseScreen.activeSelf)
        {
            return false;
        }

        // Don't allow skipping if options menu is open
        var optionsManager = FindFirstObjectByType<OptionsManager>(FindObjectsInactive.Include);
        if (optionsManager != null && optionsManager.optionsScreen != null && optionsManager.optionsScreen.activeSelf)
        {
            return false;
        }

        var keyboard = Keyboard.current;
        return keyboard != null && keyboard.tabKey.isPressed;
    }

    /// <summary>
    /// Returns WASD movement input as Vector2 (only works in Walk mode)
    /// </summary>
    public Vector2 GetMovementInput()
    {
        // Only allow movement input in Walk mode
        if (GameModeManager.GetInstance() == null || 
            GameModeManager.GetInstance().currentMode != GameModeManager.GameMode.Walk)
        {
            return Vector2.zero;
        }

        // Don't allow movement if pause menu is open
        var pauseManager = FindFirstObjectByType<PauseManager>(FindObjectsInactive.Include);
        if (pauseManager != null && pauseManager.pauseScreen != null && pauseManager.pauseScreen.activeSelf)
        {
            return Vector2.zero;
        }

        // Don't allow movement if options menu is open
        var optionsManager = FindFirstObjectByType<OptionsManager>(FindObjectsInactive.Include);
        if (optionsManager != null && optionsManager.optionsScreen != null && optionsManager.optionsScreen.activeSelf)
        {
            return Vector2.zero;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null) return Vector2.zero;

        Vector2 movement = Vector2.zero;

        // WASD input
        if (keyboard.wKey.isPressed) movement.y += 1f; // Forward
        if (keyboard.sKey.isPressed) movement.y -= 1f; // Backward
        if (keyboard.aKey.isPressed) movement.x -= 1f; // Left
        if (keyboard.dKey.isPressed) movement.x += 1f; // Right

        return movement;
    }

    /// <summary>
    /// Helper method to check if we're in Visual Novel mode
    /// </summary>
    public bool IsInVisualNovelMode()
    {
        return GameModeManager.GetInstance() != null && 
               GameModeManager.GetInstance().currentMode == GameModeManager.GameMode.VisualNovel;
    }

    /// <summary>
    /// Helper method to check if we're in Walk mode
    /// </summary>
    public bool IsInWalkMode()
    {
        return GameModeManager.GetInstance() != null && 
               GameModeManager.GetInstance().currentMode == GameModeManager.GameMode.Walk;
    }
}
