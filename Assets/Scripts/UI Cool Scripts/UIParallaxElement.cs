using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple parallax effect for individual UI elements.
/// Attach this to any UI element (Image, Text, etc.) to make it respond to mouse movement.
/// Works with both World Space and Screen Space canvases.
/// Uses the new Input System for mouse tracking.
/// </summary>
public class UIParallaxElement : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("How much this element moves relative to mouse movement")]
    [Range(-2f, 2f)]
    public float parallaxStrength = 0.5f;
    
    [Tooltip("Maximum distance this element can move from its original position")]
    [Range(5f, 500f)]
    public float maxMovement = 25f;
    
    [Tooltip("How smoothly the element moves to its target position")]
    [Range(0.1f, 10f)]
    public float smoothing = 3f;
    
    [Tooltip("Should this element move in the opposite direction?")]
    public bool invertMovement = false;
    
    [Header("Movement Constraints")]
    [Tooltip("Allow movement on X axis")]
    public bool moveX = true;
    
    [Tooltip("Allow movement on Y axis")]
    public bool moveY = true;
    
    // Private variables
    private Vector3 originalPosition;
    private Vector2 screenCenter;
    private RectTransform rectTransform;
    private bool isUIElement;
    
    void Start()
    {
        // Store original position
        originalPosition = transform.position;
        
        // Calculate screen center
        screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        
        // Check if this is a UI element
        rectTransform = GetComponent<RectTransform>();
        isUIElement = rectTransform != null;
        
        // If it's a UI element, use local position instead
        if (isUIElement)
        {
            originalPosition = rectTransform.anchoredPosition;
        }
    }
    
    void Update()
    {
        // Get mouse position using new Input System and calculate offset from center
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 mouseOffset = new Vector2(
            (mousePosition.x - screenCenter.x) / screenCenter.x,
            (mousePosition.y - screenCenter.y) / screenCenter.y
        );
        
        // Apply movement constraints
        if (!moveX) mouseOffset.x = 0f;
        if (!moveY) mouseOffset.y = 0f;
        
        // Clamp mouse offset
        mouseOffset = Vector2.ClampMagnitude(mouseOffset, 1f);
        
        // Calculate target offset
        float strengthMultiplier = invertMovement ? -1f : 1f;
        Vector2 targetOffset = mouseOffset * parallaxStrength * maxMovement * strengthMultiplier;
        
        // Calculate and apply target position
        if (isUIElement)
        {
            // For UI elements, use anchored position
            Vector2 targetPosition = (Vector2)originalPosition + targetOffset;
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * smoothing
            );
        }
        else
        {
            // For world space objects
            Vector3 targetPosition = originalPosition + new Vector3(targetOffset.x, targetOffset.y, 0);
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * smoothing
            );
        }
    }
    
    /// <summary>
    /// Resets the element to its original position
    /// </summary>
    public void ResetPosition()
    {
        if (isUIElement)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
        else
        {
            transform.position = originalPosition;
        }
    }
    
    void OnDisable()
    {
        // Reset position when disabled
        ResetPosition();
    }
    
    /// <summary>
    /// Updates the original position (useful if the element's base position changes)
    /// </summary>
    public void UpdateOriginalPosition()
    {
        if (isUIElement)
        {
            originalPosition = rectTransform.anchoredPosition;
        }
        else
        {
            originalPosition = transform.position;
        }
    }
}