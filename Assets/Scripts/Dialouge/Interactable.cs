using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attach to objects the player can interact with.
/// This version stores DialogueNode entries that reference Ink TextAsset assets (ink JSON).
/// When Interact() is called it asks DialogueStateManager which node (if any) to play,
/// then hands the chosen TextAsset to your existing DialogueManager.
/// </summary>
public class Interactable : MonoBehaviour
{
    [Tooltip("Human friendly name for prompts.")]
    public string displayName = "Object";

    [Tooltip("Stable ID used for save/persistent keys. If empty, GetInstanceID() will be used (NOT persistent).")]
    public string stableId;

    [Tooltip("Optional radius override for this object (0 = use player radius).")]
    public float customInteractionRadius = 0f;

    [Header("Dialogues (ordered)")]
    public List<DialogueNode> dialogueNodes = new List<DialogueNode>();

    [Header("Events")]
    public UnityEvent onFocus;
    public UnityEvent onDefocus;

    // called with the node index that was played (useful for animations or logic)
    [Tooltip("Called when a dialogue node (by index) is played.")]
    public NodePlayedEvent onNodePlayed;

    [Serializable]
    public class NodePlayedEvent : UnityEvent<int> { }

    [Serializable]
    public class DialogueNode
    {
        [Tooltip("Ink JSON TextAsset to play for this node.")]
        public TextAsset inkJSON;

        [Tooltip("Flags that MUST be present for this node to be considered playable.")]
        public List<string> requiredFlags;

        [Tooltip("Flags that will be set once this node is played.")]
        public List<string> unlocksFlags;

        [Tooltip("How many times this node may play. -1 = infinite")]
        public int maxPlays = -1;
    }

    public void Focus()
    {
        onFocus?.Invoke();
    }

    public void Defocus()
    {
        onDefocus?.Invoke();
    }

    /// <summary>
    /// Called by your PlayerInteractor when the player executes the interact action.
    /// Delegates to DialogueStateManager to determine which node (if any) to play,
    /// then tells your Ink DialogueManager to enter dialogue mode with the returned TextAsset.
    /// </summary>
    public void Interact()
    {
        if (DialogueStateManager.Instance == null)
        {
            Debug.LogWarning("DialogueStateManager.Instance is null. Make sure one exists in the scene.");
            return;
        }

        TextAsset chosenInk = DialogueStateManager.Instance.TriggerDialogue(this);

        if (chosenInk != null)
        {
            var vm = DialogueManager.GetInstance();
            if (vm != null)
            {
                vm.EnterDialogueMode(chosenInk);
            }
            else
            {
                Debug.LogWarning("DialogueManager (Ink presentation) instance not found. Can't enter dialogue mode.");
            }
        }
        else
        {
            // Nothing available: optional fallback
            Debug.Log($"Interactable '{displayName}' has no available dialogue nodes right now.");
        }
    }

    // Editor debug gizmo to visualize custom radius (if used)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        float r = customInteractionRadius > 0f ? customInteractionRadius : 0.0f;
        if (r > 0f)
            Gizmos.DrawWireSphere(transform.position, r);
    }
}