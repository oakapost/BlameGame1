using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles dialogue progression state independently of the Ink-based presentation manager.
/// - Tracks global flags (strings)
/// - Tracks per-interactable node play counts (uses stableId on Interactable for persistent keys)
/// - Selects the next playable DialogueNode for a given Interactable and returns its TextAsset (Ink JSON)
/// - Increments play counts and sets unlock flags when a node is chosen
/// 
/// Keep this separate so your existing DialogueManager (Ink UI/presentation) stays unchanged.
/// </summary>
public class DialogueStateManager : MonoBehaviour
{
    public static DialogueStateManager Instance { get; private set; }

    // playCounts key: "{interactableStableId}_{nodeIndex}"
    Dictionary<string, int> playCounts = new Dictionary<string, int>();

    // global flags unlocked by playing nodes or external events
    HashSet<string> flags = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject); // Temporarily disabled for testing
        
        // Clear state on awake to ensure fresh start in editor
        playCounts.Clear();
        flags.Clear();
        Debug.Log("DialogueStateManager: State cleared in Awake");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            playCounts.Clear();
            flags.Clear();
        }
    }

    /// <summary>
    /// Choose the first DialogueNode on the interactable whose requiredFlags are met and which still has remaining plays.
    /// If a node is chosen, its playcount is incremented and its unlocksFlags are set.
    /// Returns the chosen TextAsset (Ink JSON) or null if nothing is available.
    /// </summary>
    public TextAsset TriggerDialogue(Interactable interactable)
    {
        if (interactable == null)
            return null;

        if (interactable.dialogueNodes == null)
            return null;

        Debug.Log($"=== TriggerDialogue for '{interactable.displayName}' ===");
        Debug.Log($"Current global flags: {string.Join(", ", flags)}");

        for (int i = 0; i < interactable.dialogueNodes.Count; i++)
        {
            var node = interactable.dialogueNodes[i];
            // Validate node
            if (node == null)
                continue;

            Debug.Log($"Checking Node {i}: {(node.inkJSON != null ? node.inkJSON.name : "NULL")}");
            
            // Debug: Show what's in required flags
            if (node.requiredFlags != null && node.requiredFlags.Count > 0)
            {
                Debug.Log($"  Node {i} requires flags: [{string.Join(", ", node.requiredFlags)}]");
            }
            else
            {
                Debug.Log($"  Node {i} has NO required flags (empty or null list)");
            }

            // Check required flags
            bool ok = true;
            if (node.requiredFlags != null)
            {
                foreach (string req in node.requiredFlags)
                {
                    if (!string.IsNullOrEmpty(req) && !HasFlag(req))
                    {
                        Debug.Log($"  Node {i} REJECTED: Missing required flag '{req}'");
                        ok = false;
                        break;
                    }
                }
            }

            if (ok && node.requiredFlags != null && node.requiredFlags.Count > 0)
            {
                Debug.Log($"  Node {i}: All required flags present");
            }

            // Check max plays
            string key = ComposeKey(interactable, i);
            int played = GetPlayCount(key);
            if (node.maxPlays >= 0 && played >= node.maxPlays)
            {
                Debug.Log($"  Node {i} REJECTED: Already played {played}/{node.maxPlays} times");
                ok = false;
            }

            if (!ok)
                continue;

            Debug.Log($"  Node {i} SELECTED! Playing: {(node.inkJSON != null ? node.inkJSON.name : "NULL")}");

            // Node is playable. Apply side effects and return its ink JSON.
            SetPlayCount(key, played + 1);
            Debug.Log($"  Play count for key '{key}' incremented from {played} to {played + 1}");

            if (node.unlocksFlags != null)
            {
                foreach (string f in node.unlocksFlags)
                    if (!string.IsNullOrEmpty(f))
                        SetFlag(f);
            }

            // Notify the interactable if it wants to react to a specific node index
            interactable.onNodePlayed?.Invoke(i);

            return node.inkJSON;
        }

        // nothing playable
        return null;
    }

    string ComposeKey(Interactable interactable, int nodeIndex)
    {
        // Use stableId when available, otherwise fallback to instance id (NOT persistent across runs)
        string id = !string.IsNullOrEmpty(interactable.stableId) ? interactable.stableId : interactable.GetInstanceID().ToString();
        return id + "_" + nodeIndex;
    }

    int GetPlayCount(string key)
    {
        if (playCounts.TryGetValue(key, out int v)) return v;
        return 0;
    }

    void SetPlayCount(string key, int count)
    {
        playCounts[key] = count;
    }

    public bool HasFlag(string flag)
    {
        return flags.Contains(flag);
    }

    public void SetFlag(string flag)
    {
        if (string.IsNullOrEmpty(flag)) return;
        flags.Add(flag);
    }

    public void ClearFlag(string flag)
    {
        if (string.IsNullOrEmpty(flag)) return;
        flags.Remove(flag);
    }

    /// <summary>
    /// Clear all dialogue state - useful for testing or new game
    /// </summary>
    public void ResetAllDialogueState()
    {
        playCounts.Clear();
        flags.Clear();
        Debug.Log("All dialogue state has been reset!");
    }

    // --- Optional: methods to export/import state for saving ---
    public DialogueStateSnapshot CreateSnapshot()
    {
        return new DialogueStateSnapshot(playCounts, flags);
    }

    public void RestoreSnapshot(DialogueStateSnapshot snapshot)
    {
        if (snapshot == null) return;
        playCounts = new Dictionary<string, int>(snapshot.playCounts);
        flags = new HashSet<string>(snapshot.flags);
    }
}

/// <summary>
/// Simple serializable snapshot for saving/loading dialogue state.
/// Convert to/from JSON in your save system.
/// </summary>
[System.Serializable]
public class DialogueStateSnapshot
{
    public List<string> playCountKeys = new List<string>();
    public List<int> playCountValues = new List<int>();
    public List<string> flags = new List<string>();

    // not serialized fields
    [System.NonSerialized] public Dictionary<string, int> playCounts = new Dictionary<string, int>();
    [System.NonSerialized] public HashSet<string> flagsSet = new HashSet<string>();

    public DialogueStateSnapshot() { }

    public DialogueStateSnapshot(Dictionary<string, int> playCountsDict, HashSet<string> flagsSetIn)
    {
        playCounts = new Dictionary<string, int>(playCountsDict);
        flagsSet = new HashSet<string>(flagsSetIn);

        // flatten for serialization
        foreach (var kv in playCounts)
        {
            playCountKeys.Add(kv.Key);
            playCountValues.Add(kv.Value);
        }

        foreach (var f in flagsSet)
            flags.Add(f);
    }

    // call after deserializing JSON to rebuild non-serialized collections
    public void Rebuild()
    {
        playCounts = new Dictionary<string, int>();
        for (int i = 0; i < playCountKeys.Count && i < playCountValues.Count; i++)
            playCounts[playCountKeys[i]] = playCountValues[i];

        flagsSet = new HashSet<string>(flags);
    }
}