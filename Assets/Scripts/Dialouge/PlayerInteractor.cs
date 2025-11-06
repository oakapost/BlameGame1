using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the Player. Finds nearest Interactable within radius and invokes its events when player presses the key.
/// Uses a LayerMask to limit which colliders are considered interactable.
/// Works with 3D colliders (Collider). If you need 2D, you can extend with Physics2D overlap checks.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Origin used for overlap checks (usually the player's transform or a child).")]
    public Transform origin;
    [Tooltip("Radius to search for interactable objects (meters).")]
    public float interactionRadius = 1.6f;
    [Tooltip("Which layers contain Interactable colliders.")]
    public LayerMask interactableLayer;

    [Header("Input / UI")]
    [Tooltip("Optional UI Text to show prompts. Can be null; you can wire your own prompt system.")]
    public Text promptText;
    [Tooltip("Prompt format, {0} will be replaced by Interactable.displayName")]
    public string promptFormat = "Press [Space/Enter] to interact with {0}";

    Interactable focusedInteractable;

    void Reset()
    {
        origin = transform;
    }

    void Update()
    {
        FindNearestInteractable();
        CheckInput();
    }

    void FindNearestInteractable()
    {
        if (origin == null) origin = transform;

        Collider[] cols = Physics.OverlapSphere(origin.position, interactionRadius, interactableLayer);
        Interactable nearest = null;
        float nearestSqr = float.MaxValue;

        foreach (var c in cols)
        {
            var interact = c.GetComponentInParent<Interactable>();
            if (interact == null) continue;

            // If the interactable has a custom radius, check that
            float allowed = interact.customInteractionRadius > 0f ? interact.customInteractionRadius : interactionRadius;
            float sqr = (interact.transform.position - origin.position).sqrMagnitude;
            if (sqr <= allowed * allowed && sqr < nearestSqr)
            {
                nearest = interact;
                nearestSqr = sqr;
            }
        }

        if (nearest != focusedInteractable)
        {
            if (focusedInteractable != null)
                focusedInteractable.Defocus();

            focusedInteractable = nearest;

            if (focusedInteractable != null)
            {
                focusedInteractable.Focus();
                if (promptText != null)
                    promptText.text = string.Format(promptFormat, focusedInteractable.displayName);
            }
            else
            {
                if (promptText != null)
                    promptText.text = "";
            }
        }
    }

    void CheckInput()
    {
        var inputHandler = InputHandler.GetInstance();
        if (focusedInteractable != null && inputHandler != null && inputHandler.GetInteractPressed())
        {
            focusedInteractable.Interact();
            // optionally clear focus so player won't immediately re-interact
            // focusedInteractable.Defocus();
            // focusedInteractable = null;
        }
    }

    // Visualize the detection radius in editor
    void OnDrawGizmosSelected()
    {
        if (origin == null) origin = transform;
        Gizmos.color = new Color(0.2f, 0.8f, 0.6f, 0.3f);
        Gizmos.DrawWireSphere(origin.position, interactionRadius);
    }
}