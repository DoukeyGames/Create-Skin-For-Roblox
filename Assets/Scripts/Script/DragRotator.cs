using UnityEngine;
using UnityEngine.EventSystems;

public class DragRotator : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler

{
    [Header("Settings")]
    public float rotationSpeed = 200f; // Speed of the rotation
    public RectTransform targetImage; // The image to scale
    public float transparencyDuringDrag = 0.5f; // Transparency while dragging

    private CanvasGroup targetCanvasGroup;
    private Vector2 initialTouchPosition;
    private Vector3 initialRotation;
    private Vector3 targetRotation; // Desired scale value
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
        initialRotation = targetImage.eulerAngles;
        targetRotation = initialRotation;
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
        
        // Get the change in position of the pointer (dragging distance)
        Vector2 deltaPosition = currentTouchPosition - initialTouchPosition;

        // Calculate how much to rotate based on the horizontal drag movement
        float angleDelta = deltaPosition.x * rotationSpeed * Time.deltaTime;

        // Rotate the image that we want to rotate (with CanvasGroup)
        targetImage.transform.Rotate(Vector3.forward, -angleDelta); // Negative sign to reverse direction if needed

        // Update the initial pointer position for the next frame
      //  initialTouchPosition = eventData.position;
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
