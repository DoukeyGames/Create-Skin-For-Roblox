using PaintIn3D;
using UnityEngine;
using UnityEngine.EventSystems;
public class StickerDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform initialParent;
    private Transform currentParent;
    private CwPaintDecal m_paintDecal;

    public Transform originalParent; 
    public GameObject border; 
    public Canvas canvas;
    public GameObject decal;

    private bool isParented = false; 
    private string targetTag = "Target";

    public static bool stickerApplyig;
 
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = transform.root.GetComponent<Canvas>();

        initialParent = transform.parent;
        originalPosition = rectTransform != null ? rectTransform.anchoredPosition : (Vector2)transform.position;
        stickerApplyig = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform != null ? rectTransform.anchoredPosition : (Vector2)transform.position;
        canvasGroup.blocksRaycasts = false;  

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!border.activeInHierarchy)
            return;
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        }
        else
        {
            transform.position += (Vector3)eventData.delta / canvas.scaleFactor;
        }
    }

    private Transform closestTarget;
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

         closestTarget = null;
        float maxOverlap = 0f;

        foreach (var target in FindObjectsOfType<Transform>())
        {
            if (target.CompareTag(targetTag)) 
            {
                RectTransform targetRect = target.GetComponent<RectTransform>();
                if (targetRect == null || target == transform) continue;

                float overlap = CalculateOverlap(rectTransform, targetRect);
                if (overlap > maxOverlap)
                {
                    maxOverlap = overlap;
                    closestTarget = target;
                }
            }
        }

        if (closestTarget != null && maxOverlap > 0f)  // Ensure there is an actual overlap
        {
           
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
            currentParent = initialParent;
            isParented = false;
        }
    }

    public void SetSprite()
    {
        if (closestTarget)
        {
            rectTransform.SetParent(closestTarget);

            currentParent = closestTarget;
            if (currentParent)
            {
                decal.transform.SetParent(currentParent.GetComponent<LabelHolder>().stickerParent);
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
            canvasGroup.blocksRaycasts = true;

            closestTarget = null;
            float maxOverlap = 0f;

            foreach (var target in FindObjectsOfType<Transform>())
            {
                if (target.CompareTag(targetTag)) 
                {
                    RectTransform targetRect = target.GetComponent<RectTransform>();
                    if (targetRect == null || target == transform) continue;

                    float overlap = CalculateOverlap(rectTransform, targetRect);
                    if (overlap > maxOverlap)
                    {
                        maxOverlap = overlap;
                        closestTarget = target;
                    }
                }
            }

            if (closestTarget != null && maxOverlap > 0f)
            {
                currentParent = closestTarget;
                if (currentParent)
                {
                    decal.transform.SetParent(currentParent.GetComponent<LabelHolder>().stickerParent);
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
                rectTransform.anchoredPosition = originalPosition;
                currentParent = initialParent;
                isParented = false;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isParented)
        {
            rectTransform.SetParent(originalParent);
            isParented = false;
            currentParent = null;
            border.SetActive(true);
            stickerApplyig = true;
            decal.transform.SetParent(transform);
            
        }
    }

    private float CalculateOverlap(RectTransform rect1, RectTransform rect2)
    {
        Rect r1 = GetWorldRect(rect1);
        Rect r2 = GetWorldRect(rect2);

        if (!r1.Overlaps(r2)) return 0f;

        Rect intersection = Rect.MinMaxRect(
            Mathf.Max(r1.xMin, r2.xMin),
            Mathf.Max(r1.yMin, r2.yMin),
            Mathf.Min(r1.xMax, r2.xMax),
            Mathf.Min(r1.yMax, r2.yMax)
        );

        return intersection.width * intersection.height;
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
    }
    public void DeleteSticker()
    {
        Destroy(decal);
        Destroy(gameObject);
    }
}