using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Ink.Runtime;
using TMPro;

public class DialougeManager : MonoBehaviour
{
    [Header("Dialouge UI")] 
    [SerializeField] private GameObject Textbox;
    [SerializeField] private TextMeshProUGUI dialougeText;
    [SerializeField] private TextMeshProUGUI nameText;
    
    [Header("Character Images")]
    [SerializeField] private UnityEngine.UI.Image blakeSprite;      // Always on the left
    [SerializeField] private UnityEngine.UI.Image otherCharacterSprite; // Always on the right
    
    [Header("Character Sprites")]
    [SerializeField] private Sprite[] characterSprites;
    private static DialougeManager instance;
    private Story currentStory;
    private bool dialougeIsPlaying;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one DialougeManager in scene.");
        }
        instance = this;
    }

    public static DialougeManager GetInstance()
    {
        return instance;
    }
    
    private void Update()
    {
        if (!dialougeIsPlaying)
        {
            return;
        }
        
        if (InputHandler.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    public void EnterDialougeMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialougeIsPlaying = true;
        GameModeManager.GetInstance().SetMode(GameModeManager.GameMode.VisualNovel);
        ContinueStory();
    }

    private void ExitDialougeMode()
    {
        dialougeIsPlaying = false;
        GameModeManager.GetInstance().SetMode(GameModeManager.GameMode.Walk);
        dialougeText.text = "";
        
        // Clear the name text
        if (nameText != null)
        {
            nameText.text = "";
        }

        // Hide all character sprites
        if (blakeSprite != null)
        {
            blakeSprite.gameObject.SetActive(false);
        }
        if (otherCharacterSprite != null)
        {
            otherCharacterSprite.gameObject.SetActive(false);
        }
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            // Get the next line of dialogue
            string dialogueLine = currentStory.Continue();
            
            // Handle tags (including speaker name)
            HandleTags(currentStory.currentTags);
            
            // Set the dialogue text
            dialougeText.text = dialogueLine;
        }
        else
        {
            ExitDialougeMode();
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        // Process each tag in the current line
        foreach (string tag in currentTags)
        {
            // Parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
                continue;

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // Handle different tag types
            switch (tagKey)
            {
                case "speaker":
                    DisplaySpeakerName(tagValue);
                    break;
                case "sprite":
                    DisplayCharacterSprite(tagValue);
                    break;
                // You can add more tag types here later (like emotions, etc.)
                default:
                    Debug.Log("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void DisplaySpeakerName(string speakerName)
    {
        if (nameText != null)
        {
            nameText.text = speakerName;
        }
        else
        {
            Debug.LogWarning("Name text component not assigned!");
        }
    }

    private void DisplayCharacterSprite(string spriteName)
    {
        // Find the sprite with the matching name
        Sprite foundSprite = FindSpriteByName(spriteName);
        
        if (foundSprite == null)
        {
            Debug.LogWarning($"Sprite '{spriteName}' not found in character sprites array!");
            return;
        }

        // Determine which character this sprite belongs to based on name
        if (spriteName.StartsWith("Blake"))
        {
            // This is a Blake sprite - show on left
            if (blakeSprite != null)
            {
                blakeSprite.sprite = foundSprite;
                blakeSprite.gameObject.SetActive(true);
                Debug.Log($"Changed Blake sprite to: {spriteName}");
            }
            else
            {
                Debug.LogWarning("Blake sprite component not assigned!");
            }
        }
        else
        {
            // This is another character's sprite - show on right
            if (otherCharacterSprite != null)
            {
                otherCharacterSprite.sprite = foundSprite;
                otherCharacterSprite.gameObject.SetActive(true);
                Debug.Log($"Changed other character sprite to: {spriteName}");
            }
            else
            {
                Debug.LogWarning("Other character sprite component not assigned!");
            }
        }
    }

    // Optional: Method to hide specific characters
    public void HideBlakeSprite()
    {
        if (blakeSprite != null)
        {
            blakeSprite.gameObject.SetActive(false);
            Debug.Log("Hidden Blake sprite");
        }
    }

    public void HideOtherCharacterSprite()
    {
        if (otherCharacterSprite != null)
        {
            otherCharacterSprite.gameObject.SetActive(false);
            Debug.Log("Hidden other character sprite");
        }
    }

    private Sprite FindSpriteByName(string spriteName)
    {
        foreach (Sprite sprite in characterSprites)
        {
            if (sprite != null && sprite.name == spriteName)
            {
                return sprite;
            }
        }
        return null;
    }
}
