using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach this to the Button (or any UI element that receives pointer events).
/// Set "targetImage" to the separate Image you want tinted while hovering.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class HoverChangeImageColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("The Image (UI) that should change color when the pointer hovers this element.")]
    [SerializeField] private Image targetImage;

    [Tooltip("Color applied to the target image while pointer is over this object.")]
    [SerializeField] private Color hoverColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    [Tooltip("Color applied to the target image when the pointer is not over this object.")]
    [SerializeField] private Color normalColor = Color.white;

    // Optionally restore the original color if you don't want to hardcode normalColor:
    private Color originalColor;
    private bool originalSaved;

    private void Reset()
    {
        // Try to auto-find a targetImage in the scene (optional convenience).
        if (targetImage == null)
            targetImage = FindObjectOfType<Image>();
    }

    private void Start()
    {
        if (targetImage == null)
        {
            Debug.LogWarning($"{nameof(HoverChangeImageColor)} on '{gameObject.name}' has no targetImage assigned.", this);
            return;
        }

        // Save original color so Exit restores it if normalColor wasn't set explicitly
        originalColor = targetImage.color;
        originalSaved = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage == null) return;
        targetImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage == null) return;
        targetImage.color = normalColor != default ? normalColor : (originalSaved ? originalColor : Color.white);
    }

    // Public helpers if you prefer to call from EventTrigger or from other scripts:
    public void ApplyHoverColor()
    {
        if (targetImage != null) targetImage.color = hoverColor;
    }

    public void ApplyNormalColor()
    {
        if (targetImage != null) targetImage.color = normalColor != default ? normalColor : (originalSaved ? originalColor : Color.white);
    }
}