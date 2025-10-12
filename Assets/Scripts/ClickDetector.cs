using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour, IPointerClickHandler
{
    public GameManager gameManager;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager == null)
        {
            // Fallback: try to find GameManager in scene
            gameManager = FindFirstObjectByType<GameManager>();
        }
        
        if (gameManager != null)
        {
            // Determine which button was clicked based on GameObject name
            string buttonName = gameObject.name;
            
            if (buttonName.Contains("NewGame") || buttonName.Contains("New Game"))
            {
                gameManager.OnNewGameButtonClicked();
            }
            else if (buttonName.Contains("Quit") || buttonName.Contains("Exit"))
            {
                gameManager.OnQuitButtonClicked();
            }
        }
    }
}