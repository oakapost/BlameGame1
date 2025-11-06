using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Parallax controller that creates a depth effect by moving UI elements and images
/// based on mouse movement. Elements move at different speeds to simulate depth.
/// Uses the new Input System for mouse tracking.
/// </summary>
public class ParallaxController : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("How sensitive the parallax effect is to mouse movement")]
    [Range(0.1f, 5f)]
    public float sensitivity = 1f;
    
    [Tooltip("How smoothly elements move to their target positions")]
    [Range(0.1f, 10f)]
    public float smoothing = 2f;
    
    [Tooltip("Maximum distance elements can move from their original positions")]
    [Range(10f, 200f)]
    public float maxOffset = 50f;
    
    [Header("Parallax Elements")]
    [Tooltip("List of all elements that should move with parallax effect")]
    public List<ParallaxElement> parallaxElements = new List<ParallaxElement>();
    
    // Mouse position tracking
    private Vector2 mousePosition;
    private Vector2 screenCenter;
    private Camera mainCamera;
    
    /// <summary>
    /// Data structure to hold information about each parallax element
    /// </summary>
    [System.Serializable]
    public class ParallaxElement
    {
        [Tooltip("The transform to apply parallax movement to")]
        public Transform elementTransform;
        
        [Tooltip("How much this element moves relative to mouse movement (0 = no movement, 1 = full movement)")]
        [Range(-2f, 5f)]
        public float parallaxStrength = 0.5f;
        
        [Tooltip("Should this element move in the opposite direction? (for background elements)")]
        public bool invertMovement = false;
        
        // Store original position to return to
        [HideInInspector]
        public Vector3 originalPosition;
    }
    
    void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
        
        // Calculate screen center
        screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        
        // Store original positions of all parallax elements
        foreach (ParallaxElement element in parallaxElements)
        {
            if (element.elementTransform != null)
            {
                element.originalPosition = element.elementTransform.position;
            }
        }
    }
    
    void Update()
    {
        // Get current mouse position using new Input System
        mousePosition = Mouse.current.position.ReadValue();
        
        // Calculate mouse offset from screen center (-1 to 1 range)
        Vector2 mouseOffset = new Vector2(
            (mousePosition.x - screenCenter.x) / screenCenter.x,
            (mousePosition.y - screenCenter.y) / screenCenter.y
        );
        
        // Clamp the offset to prevent extreme movement
        mouseOffset = Vector2.ClampMagnitude(mouseOffset, 1f);
        
        // Apply parallax effect to all elements
        ApplyParallaxEffect(mouseOffset);
    }
    
    /// <summary>
    /// Applies the parallax movement to all registered elements
    /// </summary>
    /// <param name="mouseOffset">Normalized mouse offset from screen center</param>
    private void ApplyParallaxEffect(Vector2 mouseOffset)
    {
        foreach (ParallaxElement element in parallaxElements)
        {
            if (element.elementTransform == null)
                continue;
            
            // Calculate the target offset for this element
            float strengthMultiplier = element.invertMovement ? -1f : 1f;
            Vector2 targetOffset = mouseOffset * element.parallaxStrength * sensitivity * maxOffset * strengthMultiplier;
            
            // Calculate target position
            Vector3 targetPosition = element.originalPosition + new Vector3(targetOffset.x, targetOffset.y, 0);
            
            // Smoothly move to target position
            element.elementTransform.position = Vector3.Lerp(
                element.elementTransform.position,
                targetPosition,
                Time.deltaTime * smoothing
            );
        }
    }
    
    /// <summary>
    /// Adds a new element to the parallax system at runtime
    /// </summary>
    /// <param name="transform">The transform to add</param>
    /// <param name="strength">Parallax strength for this element</param>
    /// <param name="invert">Should movement be inverted?</param>
    public void AddParallaxElement(Transform transform, float strength = 0.5f, bool invert = false)
    {
        ParallaxElement newElement = new ParallaxElement
        {
            elementTransform = transform,
            parallaxStrength = strength,
            invertMovement = invert,
            originalPosition = transform.position
        };
        
        parallaxElements.Add(newElement);
    }
    
    /// <summary>
    /// Removes an element from the parallax system
    /// </summary>
    /// <param name="transform">The transform to remove</param>
    public void RemoveParallaxElement(Transform transform)
    {
        for (int i = parallaxElements.Count - 1; i >= 0; i--)
        {
            if (parallaxElements[i].elementTransform == transform)
            {
                // Return element to original position before removing
                transform.position = parallaxElements[i].originalPosition;
                parallaxElements.RemoveAt(i);
                break;
            }
        }
    }
    
    /// <summary>
    /// Resets all elements to their original positions
    /// </summary>
    public void ResetAllElements()
    {
        foreach (ParallaxElement element in parallaxElements)
        {
            if (element.elementTransform != null)
            {
                element.elementTransform.position = element.originalPosition;
            }
        }
    }
    
    void OnDisable()
    {
        // Reset positions when script is disabled
        ResetAllElements();
    }
}