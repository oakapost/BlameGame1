using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Ink.Runtime;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")] 
    [SerializeField] private GameObject Textbox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    
    [Header("Character Images")]
    [SerializeField] private UnityEngine.UI.Image blakeSprite;      // Always on the left
    [SerializeField] private UnityEngine.UI.Image otherCharacterSprite; // Always on the right
    [SerializeField] private UnityEngine.UI.Image thirdCharacterSprite; // Center or special position
    
    [Header("Character Sprites")]
    [SerializeField] private Sprite[] characterSprites;
    
    [Header("Animation Settings")]
    [SerializeField] private float hopHeight = 20f;
    [SerializeField] private float hopDuration = 0.3f;
    
    [Header("Skip Settings")]
    [SerializeField] private float skipSpeed = 0.1f;
    
    [Header("Typewriter Effect")]
    [SerializeField] private float typewriterSpeed = 0.05f; 
    [SerializeField] private bool enableTypewriter = true; // Toggle typewriter effect on/off
    
    [Header("Character Name Colors")]
    [SerializeField] private Color blakeNameColor = Color.red;
    [SerializeField] private Color avaNameColor = Color.magenta; 
    [SerializeField] private Color jellyfishNameColor = Color.yellow;
    [SerializeField] private Color lydiaNameColor = Color.blue;
    [SerializeField] private Color vivianNameColor = new Color(1f, 0.5f, 0f); 
    [SerializeField] private Color jayNameColor = new Color(0.5f, 0f, 1f);
    [SerializeField] private Color noahNameColor = Color.green;
    [SerializeField] private Color defaultNameColor = Color.black;
    [SerializeField] private bool useCharacterColors = true; // Toggle colored names on/off

    private static DialogueManager instance;
    private Story currentStory;
    private bool dialogueIsPlaying;
    private string currentSpeaker = "";
    
    // Skip functionality
    private float skipTimer = 0f;
    private bool isSkipping = false;
    
    // Typewriter effect functionality
    private Coroutine typewriterCoroutine;
    private bool isTyping = false;
    private string currentDialogueLine = "";

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one DialougeManager in scene.");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }
    
    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        
        var inputHandler = InputHandler.GetInstance();
        
        // Check for skip input (Tab key held down)
        if (inputHandler.GetSkipPressed())
        {
            isSkipping = true;
            // If currently typing, complete the typewriter and continue skipping
            if (isTyping)
            {
                CompleteTypewriter();
            }
            
            // Rapidly advance dialogue while Tab is held
            skipTimer -= Time.deltaTime;
            if (skipTimer <= 0f)
            {
                ContinueStory();
                skipTimer = skipSpeed; // Reset timer for next skip advance
            }
        }
        else
        {
            isSkipping = false;
            // Reset skip timer when not skipping
            skipTimer = 0f;
            
            // Normal input handling
            if (inputHandler.GetSubmitPressed())
            {
                // If currently typing, complete the typewriter effect
                if (isTyping)
                {
                    CompleteTypewriter();
                }
                else
                {
                    // If not typing, advance to next dialogue line
                    ContinueStory();
                }
            }
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        GameModeManager.GetInstance().SetMode(GameModeManager.GameMode.VisualNovel);
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        GameModeManager.GetInstance().SetMode(GameModeManager.GameMode.Walk);
        dialogueText.text = "";

        // Clear the name text
        if (nameText != null)
        {
            nameText.text = "";
        }

        // Reset current speaker for next dialogue
        currentSpeaker = "";

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
            
            // Display the dialogue with typewriter effect
            if (enableTypewriter)
            {
                StartTypewriter(dialogueLine);
            }
            else
            {
                // Instant display if typewriter is disabled
                dialogueText.text = dialogueLine;
                currentDialogueLine = dialogueLine;
            }
        }
        else
        {
            ExitDialogueMode();
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
                case "third_sprite":
                    DisplayThirdCharacterSprite(tagValue);
                    break;
                // You can add more tag types here later (like emotions, etc.)
                default:
                    Debug.Log("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void StartTypewriter(string line)
    {
        // Stop any existing typewriter effect
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }
        
        currentDialogueLine = line;
        isTyping = true;
        typewriterCoroutine = StartCoroutine(TypewriterEffect(line));
    }

    private System.Collections.IEnumerator TypewriterEffect(string line)
    {
        dialogueText.text = "";
        
        for (int i = 0; i <= line.Length; i++)
        {
            dialogueText.text = line.Substring(0, i);
            
            // Check if skipping is active - if so, speed up dramatically
            float currentSpeed = isSkipping ? typewriterSpeed * 0.1f : typewriterSpeed;
            
            yield return new WaitForSeconds(currentSpeed);
        }
        
        isTyping = false;
        typewriterCoroutine = null;
    }

    public void CompleteTypewriter()
    {
        if (isTyping && typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            dialogueText.text = currentDialogueLine;
            isTyping = false;
            typewriterCoroutine = null;
        }
    }

    private void DisplaySpeakerName(string speakerName)
    {
        if (nameText != null)
        {
            if (useCharacterColors)
            {
                // Apply color based on character name
                string coloredName = GetColoredSpeakerName(speakerName);
                nameText.text = coloredName;
            }
            else
            {
                // No coloring, just display the name
                nameText.text = speakerName;
            }
        }
        else
        {
            Debug.LogWarning("Name text component not assigned!");
        }
        
        // Check if speaker changed and trigger hop animation (but not when skipping)
        if (currentSpeaker != speakerName)
        {
            if (!isSkipping)
            {
                TriggerSpeakerHop(speakerName);
            }
            currentSpeaker = speakerName;
        }
    }

    private string GetColoredSpeakerName(string speakerName)
    {
        // Convert Unity Color to hex string for TextMeshPro rich text
        string colorHex;
        
        switch (speakerName.ToLower())
        {
            case "blake":
                colorHex = ColorUtility.ToHtmlStringRGB(blakeNameColor);
                break;
            case "ava":
                colorHex = ColorUtility.ToHtmlStringRGB(avaNameColor);
                break;
            case "jellyfish":
                colorHex = ColorUtility.ToHtmlStringRGB(jellyfishNameColor);
                break;
            case "lydia":
                colorHex = ColorUtility.ToHtmlStringRGB(lydiaNameColor);
                break;
            case "vivian":
                colorHex = ColorUtility.ToHtmlStringRGB(vivianNameColor);
                break;
            case "jay":
                colorHex = ColorUtility.ToHtmlStringRGB(jayNameColor);
                break;
            case "noah":
                colorHex = ColorUtility.ToHtmlStringRGB(noahNameColor);
                break;
            default:
                colorHex = ColorUtility.ToHtmlStringRGB(defaultNameColor);
                break;
        }
        
        // Return the name wrapped in TextMeshPro color tags
        return $"<color=#{colorHex}>{speakerName}</color>";
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
                //Debug.Log($"Changed Blake sprite to: {spriteName}");
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
                Debug.Log($"Changed other character sprite to: {spriteName}");
            }
            else
            {
                Debug.LogWarning("Other character sprite component not assigned!");
            }
        }
    }

    private void DisplayThirdCharacterSprite(string spriteName)
    {
        // Find the sprite with the matching name
        Sprite foundSprite = FindSpriteByName(spriteName);
        
        if (foundSprite == null)
        {
            Debug.LogWarning($"Third character sprite '{spriteName}' not found in character sprites array!");
            return;
        }

        // Display the sprite on the third character image
        if (thirdCharacterSprite != null)
        {
            thirdCharacterSprite.sprite = foundSprite;
            Debug.Log($"Changed third character sprite to: {spriteName}");
        }
        else
        {
            Debug.LogWarning("Third character sprite component not assigned!");
        }
    }

    private void TriggerSpeakerHop(string speakerName)
    {
        // Determine which character sprite to animate based on speaker
        UnityEngine.UI.Image targetSprite = null;
        
        if (speakerName == "Blake")
        {
            targetSprite = blakeSprite;
        }
        else
        {
            targetSprite = otherCharacterSprite;
        }
        
        // Only animate if the sprite exists and has a sprite assigned
        if (targetSprite != null && targetSprite.sprite != null)
        {
            StartCoroutine(HopAnimation(targetSprite.transform));
        }
    }

    private System.Collections.IEnumerator HopAnimation(Transform target)
    {
        Vector3 originalPosition = target.localPosition;
        Vector3 hopPosition = originalPosition + Vector3.up * hopHeight;
        
        float elapsedTime = 0f;
        float halfDuration = hopDuration / 2f;
        
        // Hop up
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / halfDuration;
            
            // Use easing for smooth animation
            float easedProgress = Mathf.Sin(progress * Mathf.PI * 0.5f); // Ease out
            target.localPosition = Vector3.Lerp(originalPosition, hopPosition, easedProgress);
            
            yield return null;
        }
        
        elapsedTime = 0f;
        
        // Hop down
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / halfDuration;
            
            // Use easing for smooth animation
            float easedProgress = 1f - Mathf.Cos(progress * Mathf.PI * 0.5f); // Ease in
            target.localPosition = Vector3.Lerp(hopPosition, originalPosition, easedProgress);
            
            yield return null;
        }
        
        // Ensure we end exactly at the original position
        target.localPosition = originalPosition;
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
