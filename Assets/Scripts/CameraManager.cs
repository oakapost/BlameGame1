using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [SerializeField] private GameObject walkModeCamera;
    [SerializeField] private GameObject visualNovelModeCamera;
    
    [Header("Camera Priorities")]
    [SerializeField] private int activeCameraPriority = 10;
    [SerializeField] private int inactiveCameraPriority = 0;
    
    private static CameraManager instance;
    private GameModeManager gameModeManager;
    private GameModeManager.GameMode currentMode;
    
    private void Awake()
    {
        // Singleton pattern
        if (instance != null)
        {
            Debug.LogWarning("Found more than one CameraManager in scene.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    public static CameraManager GetInstance()
    {
        return instance;
    }
    
    private void Start()
    {
        // Get reference to GameModeManager
        gameModeManager = GameModeManager.GetInstance();
        
        if (gameModeManager == null)
        {
            Debug.LogError("GameModeManager not found! CameraManager needs GameModeManager to function.");
            return;
        }
        
        // Set initial camera based on current game mode
        SwitchCamera(gameModeManager.currentMode);
        currentMode = gameModeManager.currentMode;
        
        // Validate camera references
        if (walkModeCamera == null)
            Debug.LogWarning("WalkModeCamera not assigned in CameraManager!");
            
        if (visualNovelModeCamera == null)
            Debug.LogWarning("VisualNovelModeCamera not assigned in CameraManager!");
    }
    
    private void Update()
    {
        // Check if game mode has changed
        if (gameModeManager != null && currentMode != gameModeManager.currentMode)
        {
            SwitchCamera(gameModeManager.currentMode);
            currentMode = gameModeManager.currentMode;
        }
    }
    
    private void SwitchCamera(GameModeManager.GameMode newMode)
    {
        switch (newMode)
        {
            case GameModeManager.GameMode.Walk:
                SetActiveCamera(walkModeCamera, visualNovelModeCamera);
                break;
                
            case GameModeManager.GameMode.VisualNovel:
                SetActiveCamera(visualNovelModeCamera, walkModeCamera);
                break;
                
            default:
                SetActiveCamera(walkModeCamera, visualNovelModeCamera);
                break;
        }
    }
    
    private void SetActiveCamera(GameObject activeCamera, GameObject inactiveCamera)
    {
        // Enable/Disable the GameObjects for reliable camera switching
        if (activeCamera != null)
        {
            activeCamera.SetActive(true);
        }
        
        if (inactiveCamera != null)
        {
            inactiveCamera.SetActive(false);
        }
        
        // Set priorities as backup
        StartCoroutine(SetPriorityDelayed(activeCamera, inactiveCamera));
    }
    
    private System.Collections.IEnumerator SetPriorityDelayed(GameObject activeCamera, GameObject inactiveCamera)
    {
        yield return null; // Wait one frame
        
        // Set priority on active camera
        if (activeCamera != null && activeCamera.activeInHierarchy)
        {
            var cinemachineComponent = activeCamera.GetComponent("CinemachineCamera");
            if (cinemachineComponent == null)
                cinemachineComponent = activeCamera.GetComponent("CinemachineVirtualCamera");
            
            if (cinemachineComponent != null)
            {
                var priorityProperty = cinemachineComponent.GetType().GetProperty("Priority");
                if (priorityProperty != null)
                {
                    priorityProperty.SetValue(cinemachineComponent, activeCameraPriority);
                }
            }
        }
        
        // Set priority on inactive camera
        if (inactiveCamera != null)
        {
            // Re-enable temporarily to set priority, then disable again
            inactiveCamera.SetActive(true);
            
            var cinemachineComponent = inactiveCamera.GetComponent("CinemachineCamera");
            if (cinemachineComponent == null)
                cinemachineComponent = inactiveCamera.GetComponent("CinemachineVirtualCamera");
            
            if (cinemachineComponent != null)
            {
                var priorityProperty = cinemachineComponent.GetType().GetProperty("Priority");
                if (priorityProperty != null)
                {
                    priorityProperty.SetValue(cinemachineComponent, inactiveCameraPriority);
                }
            }
            
            inactiveCamera.SetActive(false);
        }
    }
    
    /// <summary>
    /// Manually switch to Walk Mode camera
    /// </summary>
    public void SwitchToWalkMode()
    {
        SetActiveCamera(walkModeCamera, visualNovelModeCamera);
    }
    
    /// <summary>
    /// Manually switch to Visual Novel Mode camera
    /// </summary>
    public void SwitchToVisualNovelMode()
    {
        SetActiveCamera(visualNovelModeCamera, walkModeCamera);
    }
}