using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(ScrollRect))]
public class SnapScroll : MonoBehaviour
{
    public RectTransform content;
    public RectTransform highlightArea;
    public float snapSpeed = 10f;

    private ScrollRect scrollRect;
    private bool isDragging;
    private Coroutine snappingCoroutine;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.inertia = true;
    }

    public void OnBeginDrag()
    {
        isDragging = true;
        if (snappingCoroutine != null)
        {
            StopCoroutine(snappingCoroutine);
            snappingCoroutine = null;
        }
    }

    public void OnEndDrag()
    {
        isDragging = false;
        snappingCoroutine = StartCoroutine(SnapToClosestItemInCenter());
    }

    private IEnumerator SnapToClosestItemInCenter()
    {
        yield return new WaitForEndOfFrame();

        // Vị trí highlight theo world, chuyển sang local của content
        Vector3 worldHighlightCenter = highlightArea.position;
        Vector3 localHighlightCenter = content.InverseTransformPoint(worldHighlightCenter);

        RectTransform closestItem = null;
        float minDistance = float.MaxValue;

        foreach (RectTransform item in content)
        {
            float itemHeight = item.rect.height;
            float itemPivotOffset = itemHeight * (0.5f - item.pivot.y);
            float itemCenterY = item.localPosition.y + itemPivotOffset;

            float distance = Mathf.Abs(itemCenterY - localHighlightCenter.y);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestItem = item;
            }
        }

        if (closestItem != null)
        {
            float itemHeight = closestItem.rect.height;
            float highlightHeight = highlightArea.rect.height;

            float itemCenterY = closestItem.localPosition.y + itemHeight * (0.5f - closestItem.pivot.y);
            float highlightCenterY = localHighlightCenter.y + highlightHeight * (0.5f - highlightArea.pivot.y);

            float offsetY = itemCenterY - highlightCenterY + 50f;

            Vector2 startPos = content.anchoredPosition;
            Vector2 targetPos = startPos + new Vector2(0, offsetY);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * snapSpeed;
                content.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            content.anchoredPosition = targetPos;
        }

        scrollRect.velocity = Vector2.zero;
        snappingCoroutine = null;
    }

}
