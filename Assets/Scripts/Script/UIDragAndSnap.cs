using System;
using PaintIn3D;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIDragAndSnap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform initialParent;  // The initial (or manually assigned) parent.
    private Transform currentParent;
    private CwPaintDecal m_paintDecal;

    public Transform originalParent;  // Manually assign this in the Inspector for original parent.
    public GameObject border;  // Manually assign this in the Inspector for original parent.
    public Canvas canvas;            // The canvas this image is part of.
    public GameObject decal;            // The canvas this image is part of.

    private bool isParented = false;  // Track if the object is currently parented to another object
    private string targetTag = "Target";  // The tag for valid targets to snap to

    public static bool stickerApplyig;
 
    

    private void Awake()
    {
        // Get the components
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = transform.root.GetComponent<Canvas>();

        // Store the original parent and position (whether it's RectTransform or regular Transform)
        initialParent = transform.parent;
        originalPosition = rectTransform != null ? rectTransform.anchoredPosition : (Vector2)transform.position;
        stickerApplyig = true;
    }

    private void Start()
    {
    }

    // This is called when the dragging begins.
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store the position of the image before dragging
        originalPosition = rectTransform != null ? rectTransform.anchoredPosition : (Vector2)transform.position;
        canvasGroup.blocksRaycasts = false;  // Allow raycasts to pass through while dragging

    }

    // This is called while the object is being dragged.
    public void OnDrag(PointerEventData eventData)
    {
        if(!border.activeInHierarchy)
       return;
            if (rectTransform != null)
        {
            // Adjust position based on drag for UI elements (RectTransform)
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        }
        else
        {
            // Handle non-UI objects (regular Transforms) by moving them in world space
            transform.position += (Vector3)eventData.delta / canvas.scaleFactor;
        }
    }

    private Transform closestTarget;
    // This is called when the dragging ends (when mouse is released).
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;  // Disable raycast blocking after dragging ends.

        // Find the closest target with the "target" tag to snap to.
         closestTarget = null;
        float maxOverlap = 0f;

        // Loop through all potential target objects in the canvas
        foreach (var target in FindObjectsOfType<Transform>())
        {
            // Ignore non-UI elements (only consider RectTransform objects) and self.
            if (target.CompareTag(targetTag)) 
            {
                RectTransform targetRect = target.GetComponent<RectTransform>();
                if (targetRect == null || target == transform) continue;  // Skip non-UI objects and self.

                // If it's a UI target, check for overlap using RectTransform.
                float overlap = CalculateOverlap(rectTransform, targetRect);
                if (overlap > maxOverlap)
                {
                    maxOverlap = overlap;
                    closestTarget = target;
                }
            }
        }

        // If we found an overlap with a valid "target", make this object a child of that target.
        if (closestTarget != null && maxOverlap > 0f)  // Ensure there is an actual overlap
        {
           
        }
        else
        {
            // Return to the original position if no valid overlap found.
            rectTransform.anchoredPosition = originalPosition;
            currentParent = initialParent;  // Track that we did not snap.
            isParented = false;  // Set the flag to indicate it's not parented.
        }
    }

    public void SetSprite()
    {
        if (closestTarget)
        {
            // If the target is a UI element, make this a child with RectTransform
            rectTransform.SetParent(closestTarget);
            // rectTransform.anchoredPosition = Vector2.zero;  // Center it inside the parent.

            currentParent = closestTarget; // Track the new parent.
            if (currentParent)
            {
                decal.transform.SetParent(currentParent.GetComponent<Decal>().stickerParent);
                decal.GetComponent<RectTransform>().anchoredPosition =
                    GetComponent<RectTransform>().anchoredPosition;
                decal.transform.localScale = transform.localScale;
                decal.transform.eulerAngles = transform.eulerAngles;
            }
            if (StickerHandler.Instance)
            {
                if (StickerHandler.Instance.currentSticker)
                    StickerHandler.Instance.currentSticker = null;
            }
            isParented = true;
            border.SetActive(false);
            stickerApplyig = false;
        }
        else
        {
            canvasGroup.blocksRaycasts = true;  // Disable raycast blocking after dragging ends.

            // Find the closest target with the "target" tag to snap to.
            closestTarget = null;
            float maxOverlap = 0f;

            // Loop through all potential target objects in the canvas
            foreach (var target in FindObjectsOfType<Transform>())
            {
                // Ignore non-UI elements (only consider RectTransform objects) and self.
                if (target.CompareTag(targetTag)) 
                {
                    RectTransform targetRect = target.GetComponent<RectTransform>();
                    if (targetRect == null || target == transform) continue;  // Skip non-UI objects and self.

                    // If it's a UI target, check for overlap using RectTransform.
                    float overlap = CalculateOverlap(rectTransform, targetRect);
                    if (overlap > maxOverlap)
                    {
                        maxOverlap = overlap;
                        closestTarget = target;
                    }
                }
            }

            // If we found an overlap with a valid "target", make this object a child of that target.
            if (closestTarget != null && maxOverlap > 0f)  // Ensure there is an actual overlap
            {
                // If the target is a UI element, make this a child with RectTransform
             //   rectTransform.SetParent(closestTarget);
                // rectTransform.anchoredPosition = Vector2.zero;  // Center it inside the parent.

                currentParent = closestTarget; // Track the new parent.
                if (currentParent)
                {
                    decal.transform.SetParent(currentParent.GetComponent<Decal>().stickerParent);
                    decal.GetComponent<RectTransform>().anchoredPosition =
                        GetComponent<RectTransform>().anchoredPosition;
                    decal.transform.localScale = transform.localScale;
                    decal.transform.eulerAngles = transform.eulerAngles;


                }

                isParented = true;
                border.SetActive(false);
                stickerApplyig = false;
                if (StickerHandler.Instance)
                {
                    if (StickerHandler.Instance.currentSticker)
                        StickerHandler.Instance.currentSticker = null;
                }
            }
            else
            {
                // Return to the original position if no valid overlap found.
                rectTransform.anchoredPosition = originalPosition;
                currentParent = initialParent;  // Track that we did not snap.
                isParented = false;  // Set the flag to indicate it's not parented.
            }
        }
    }

    // This is called when the image is clicked.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isParented)
        {
            // If it's a UI element, unparent it and return it to the original parent.
            rectTransform.SetParent(originalParent);  // Set to the manually assigned parent.
            //rectTransform.anchoredPosition = originalPosition;  // Optionally adjust position.
            isParented = false;
            currentParent = null;
            border.SetActive(true);
            stickerApplyig = true;
            decal.transform.SetParent(transform);
            
        }
    }

    // Calculate the overlap area between two RectTransforms (for UI elements).
    private float CalculateOverlap(RectTransform rect1, RectTransform rect2)
    {
        Rect r1 = GetWorldRect(rect1);
        Rect r2 = GetWorldRect(rect2);

        if (!r1.Overlaps(r2)) return 0f;  // No overlap.

        Rect intersection = Rect.MinMaxRect(
            Mathf.Max(r1.xMin, r2.xMin),
            Mathf.Max(r1.yMin, r2.yMin),
            Mathf.Min(r1.xMax, r2.xMax),
            Mathf.Min(r1.yMax, r2.yMax)
        );

        return intersection.width * intersection.height;  // Return overlap area.
    }

    // Get the world-space bounds of a RectTransform (for UI elements).
    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
    }
    public void setSticker()
    {
    
    }
    public void DeleteSticker()
    {
        Destroy(decal);
        Destroy(gameObject);
    }
}