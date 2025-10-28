using UnityEngine;

public class PlayerWalkController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Character Sprite")]
    [SerializeField] private SpriteRenderer characterSpriteRenderer;
    [SerializeField] private Sprite[] walkSprites; // Different directional sprites (optional)
    
    [Header("Physics")]
    [SerializeField] private Rigidbody playerRigidbody;
    
    [Header("Step Climbing")]
    [SerializeField] private float stepHeight = 0.3f; // Maximum height player can step over
    [SerializeField] private float stepCheckDistance = 0.6f; // How far ahead to check for steps
    [SerializeField] private LayerMask groundLayerMask = -1; // What layers count as ground
    
    private InputHandler inputHandler;
    private Vector3 movementDirection;
    private bool isMoving = false;
    
    private void Awake()
    {
        // Get components
        if (playerRigidbody == null)
            playerRigidbody = GetComponent<Rigidbody>();
            
        if (characterSpriteRenderer == null)
            characterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    private void Start()
    {
        inputHandler = InputHandler.GetInstance();
        
        if (inputHandler == null)
        {
            Debug.LogError("InputHandler not found! Make sure InputHandler exists in the scene.");
        }
        
        // Set up Rigidbody for character controller movement
        if (playerRigidbody != null)
        {
            playerRigidbody.freezeRotation = true; // Prevent physics rotation
            playerRigidbody.useGravity = true; // Keep gravity for ground collision
        }
    }
    
    private void Update()
    {
        // Only handle movement input if we're in Walk mode
        if (GameModeManager.GetInstance() != null && 
            GameModeManager.GetInstance().currentMode == GameModeManager.GameMode.Walk)
        {
            HandleMovementInput();
            HandleSpriteDirection();
        }
        else
        {
            // Stop movement when not in Walk mode
            movementDirection = Vector3.zero;
            isMoving = false;
        }
    }
    
    private void FixedUpdate()
    {
        // Apply movement in FixedUpdate for smooth physics-based movement
        if (movementDirection != Vector3.zero && playerRigidbody != null)
        {
            Vector3 movement = movementDirection * moveSpeed * Time.fixedDeltaTime;
            Vector3 targetPosition = transform.position + movement;
            
            // Check for step climbing
            targetPosition = HandleStepClimbing(targetPosition, movement);
            
            playerRigidbody.MovePosition(targetPosition);
        }
    }
    
    private void HandleMovementInput()
    {
        if (inputHandler == null) return;
        
        // Get movement input from InputHandler  
        Vector2 inputVector = inputHandler.GetMovementInput();
        
        // Convert 2D input to 3D movement (X and Z axis, Y stays the same)
        movementDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        
        // Normalize diagonal movement to prevent faster diagonal speed
        if (movementDirection.magnitude > 1f)
        {
            movementDirection.Normalize();
        }
        
        isMoving = movementDirection.magnitude > 0.1f;
    }
    
    private void HandleSpriteDirection()
    {
        if (characterSpriteRenderer == null || !isMoving) return;
        
        // Flip sprite based on horizontal movement direction
        if (movementDirection.x > 0.1f)
        {
            // Moving right
            characterSpriteRenderer.flipX = false;
        }
        else if (movementDirection.x < -0.1f)
        {
            // Moving left
            characterSpriteRenderer.flipX = true;
        }
        
        // Optional: Change sprite based on direction if you have directional sprites
        // This is where you could implement different walking animations
    }
    
    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        if (!enabled)
        {
            movementDirection = Vector3.zero;
            isMoving = false;
        }
    }
    
    public bool IsMoving()
    {
        return isMoving;
    }
    
    public Vector3 GetMovementDirection()
    {
        return movementDirection;
    }
    
    private Vector3 HandleStepClimbing(Vector3 targetPosition, Vector3 movement)
    {
        // Get the capsule collider for step detection
        CapsuleCollider capsule = GetComponentInChildren<CapsuleCollider>();
        if (capsule == null) return targetPosition;
        
        // Calculate raycast positions
        Vector3 currentPos = transform.position;
        Vector3 rayStart = currentPos + Vector3.up * 0.1f; // Slightly above ground
        Vector3 rayDirection = movement.normalized;
        float rayDistance = stepCheckDistance;
        
        // Check if there's an obstacle ahead at ground level
        if (Physics.Raycast(rayStart, rayDirection, rayDistance, groundLayerMask))
        {
            // Check if we can step over it by raycasting from higher up
            Vector3 stepRayStart = currentPos + Vector3.up * stepHeight;
            
            // If no obstacle at step height, try to find the top of the step
            if (!Physics.Raycast(stepRayStart, rayDirection, rayDistance, groundLayerMask))
            {
                // Cast down from the step height to find the top of the obstacle
                Vector3 downRayStart = targetPosition + Vector3.up * stepHeight;
                RaycastHit stepHit;
                
                if (Physics.Raycast(downRayStart, Vector3.down, out stepHit, stepHeight + 0.1f, groundLayerMask))
                {
                    // Found the top of the step - move to that height
                    float stepUpHeight = stepHit.point.y - currentPos.y;
                    
                    // Only step up if it's within our step height limit
                    if (stepUpHeight > 0.01f && stepUpHeight <= stepHeight)
                    {
                        return new Vector3(targetPosition.x, stepHit.point.y + 0.01f, targetPosition.z);
                    }
                }
            }
        }
        
        return targetPosition;
    }
}
