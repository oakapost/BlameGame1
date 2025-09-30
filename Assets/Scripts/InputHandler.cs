using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private static InputHandler instance;

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

    /// <summary>
    /// Returns true when Enter, Spacebar, or Left Mouse Click is pressed this frame
    /// Only works in Visual Novel mode
    /// </summary>
    public bool GetSubmitPressed()
    {
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
    /// Only works in Visual Novel mode
    /// </summary>
    public bool GetSubmitHeld()
    {
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
    /// Returns true when Escape key is pressed (for pausing/canceling)
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
