using PaintIn3D;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragScale :  MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Settings")]
    public RectTransform targetImage; // The image to scale
    public CwPaintDecal targetObject; // The image to scale
    public float minScale = 0.5f; // Minimum scale (e.g., 0.5 = 50% of the original size)
    public float maxScale = 2.0f; // Maximum scale (e.g., 2.0 = 200% of the original size)
    public float scaleSensitivity = 0.01f; // Scaling speed sensitivity
    public float lerpSpeed = 10f; // Speed of lerping
    public float transparencyDuringDrag = 0.5f; // Transparency while dragging

    private CanvasGroup targetCanvasGroup;
    private Vector2 initialTouchPosition;
    private Vector3 initialScale;
    private Vector3 targetScale; // Desired scale value
    private bool isDragging = false;

    void Awake()
    {
        if (targetImage == null)
        {
            Debug.LogError("Target Image not assigned!");
            return;
        }

        // Ensure the target image has a CanvasGroup component
        targetCanvasGroup = targetImage.GetComponent<CanvasGroup>();
        if (targetCanvasGroup == null)
        {
            targetCanvasGroup = targetImage.gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Begin dragging
        initialTouchPosition = eventData.position;
        initialScale = targetImage.localScale;
        targetScale = initialScale;
        isDragging = true;

        // Adjust transparency and interactivity
        targetCanvasGroup.alpha = transparencyDuringDrag;
        targetCanvasGroup.blocksRaycasts = false; // Make it non-interactable during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Calculate drag delta
        Vector2 currentTouchPosition = eventData.position;
        Vector2 dragDelta = currentTouchPosition - initialTouchPosition;

        // Compute the distance dragged outward/inward
        float dragMagnitude = dragDelta.magnitude; // Overall distance
        float initialDistance = initialTouchPosition.magnitude;
        float directionFactor = (dragMagnitude - initialDistance) > 0 ? 1 : -1;

        // Calculate new scale based on direction
        float scaleFactor = dragMagnitude * scaleSensitivity * directionFactor;
        float newScale = Mathf.Clamp(initialScale.x + scaleFactor, minScale, maxScale);

        // Update the target scale
        targetScale = new Vector3(newScale, newScale, 1f);
        // Lerp to the target scale smoothly
        targetImage.localScale = Vector3.Lerp(targetImage.localScale, targetScale, Time.deltaTime * lerpSpeed);
        targetObject.scale = Vector3.Lerp(targetObject.scale, targetScale, Time.deltaTime * lerpSpeed);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        // End dragging
        isDragging = false;

        // Restore transparency and interactivity
        targetCanvasGroup.alpha = 1f; // Full opacity
        targetCanvasGroup.blocksRaycasts = true; // Interactable again
    }
}
