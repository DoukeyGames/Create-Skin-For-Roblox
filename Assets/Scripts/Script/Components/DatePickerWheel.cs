using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DatePickerWheel : MonoBehaviour
{
    [Header("Scroll Settings")]
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform viewport;
    
    [Header("Item Settings")]
    public GameObject itemPrefab;
    public float itemHeight = 80f;
    public int visibleItemCount = 0;
    
    [Header("Snap Settings")]
    public float snapSpeed = 10f;
    public float snapThreshold = 0.1f;
    
    [Header("Highlight")]
    public RectTransform highlightArea;
    
    private List<DatePickerItem> items = new List<DatePickerItem>();
    private bool isSnapping = false;
    private int targetIndex = 0;
    private int currentSelectedIndex = 0;
    
    // Events
    public System.Action<int, string> OnValueChanged;
    
    private void Awake()
    {
        SetupScrollRect();
    }
    
    private void SetupScrollRect()
    {
        // ScrollRect should be set up in Inspector
        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect reference is missing! Please assign it in Inspector.");
            return;
        }
        
        // Add event listeners
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }
    
    public void SetData(List<string> data)
    {
        ClearItems();
        
        // Add padding items for proper centering
        int paddingCount = visibleItemCount / 2;
        
        // Add top padding 
        for (int i = 0; i < paddingCount; i++)
        {
            CreateItem("", true);
        }
        
        // Add actual data
        for (int i = 0; i < data.Count; i++)
        {
            CreateItem(data[i], false);
        }
        
        // Add bottom padding
        for (int i = 0; i < paddingCount; i++)
        {
            CreateItem("", true);
        }
        
        // Set initial position to show first real item
        SetSelectedIndex(5);
    }
    
    private void CreateItem(string text, bool isPadding)
    {
        GameObject itemGO = Instantiate(itemPrefab, content);
        DatePickerItem item = itemGO.GetComponent<DatePickerItem>();
        
        if (item == null)
        {
            item = itemGO.AddComponent<DatePickerItem>();
        }
        
        item.SetData(text, isPadding);
        items.Add(item);
        
        // Position item
        RectTransform itemRect = itemGO.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(200, itemHeight);
        // itemRect.anchoredPosition = new Vector2(0, -items.Count * itemHeight + itemHeight/2);
    }
    
    private void ClearItems()
    {
        foreach (var item in items)
        {
            if (item != null && item.gameObject != null)
            {
                DestroyImmediate(item.gameObject);
            }
        }
        items.Clear();
    }
    
    private void OnScrollValueChanged(Vector2 value)
    {
        UpdateItemVisuals();
    }
    
    private bool IsScrolling()
    {
        return Mathf.Abs(scrollRect.velocity.y) > snapThreshold;
    }
    
     private void UpdateItemVisuals()
    {
        foreach (var item in items)
        {
            if (item == null) continue;
            
            Vector3 worldHighlightCenter = highlightArea.position;
            Vector3 localHighlightCenter = content.InverseTransformPoint(worldHighlightCenter);
            
            float distanceFromCenter = Mathf.Abs(item.transform.localPosition.y - localHighlightCenter.y);
            float maxDistance = itemHeight * 2;
            
            float normalizedDistance = Mathf.Clamp01(distanceFromCenter / maxDistance);
            float scale = Mathf.Lerp(1f, 0.8f, normalizedDistance);
            float alpha = Mathf.Lerp(1f, 0.2f, normalizedDistance);
            item.SetVisualState(scale, alpha);
        }
    }
     
    public void OnBeginDrag()
    {
        isSnapping = false;
    }

    public void OnEndDrag()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            isSnapping = true;
            StartSnapping();
        });
    }
    
    private void StartSnapping()
    {
        int nearestIndex = GetNearestItemIndex();
        targetIndex = nearestIndex;
    }
    
    private int GetNearestItemIndex()
    {
        Vector3 worldHighlightCenter = highlightArea.position;
        Vector3 localHighlightCenter = content.InverseTransformPoint(worldHighlightCenter);
        
        float minDistance = float.MaxValue;
        int nearestIndex = 0;
        
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) continue;
            
            float itemCenterY = items[i].transform.localPosition.y + itemHeight * (0.5f - items[i].GetComponent<RectTransform>().pivot.y);
            float distance = Mathf.Abs(itemCenterY - localHighlightCenter.y);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }
        
        return nearestIndex;
    }
    
    private void Update()
    {
        if (isSnapping)
        {
            float targetY = targetIndex * itemHeight;
            float currentY = content.anchoredPosition.y;
            
            float newY = Mathf.Lerp(currentY, targetY, snapSpeed * Time.deltaTime);
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, newY);
            
            if (Mathf.Abs(newY - targetY) < 1f)
            {
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetY);
                isSnapping = false;
                OnSnapComplete();
            }
        }
    }
    
    private void OnSnapComplete()
    {
        int paddingCount = visibleItemCount / 2;
        int dataIndex = targetIndex - paddingCount;
        
        if (dataIndex != currentSelectedIndex)
        {
            currentSelectedIndex = dataIndex;
            string selectedValue = items[targetIndex].GetText();
            OnValueChanged?.Invoke(currentSelectedIndex, selectedValue);
        }
    }
    
    public void SetSelectedIndex(int index)
    {
        int paddingCount = visibleItemCount / 2;
        targetIndex = index + paddingCount;
        
        float targetY = targetIndex * itemHeight;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetY);
        
        currentSelectedIndex = index;
        UpdateItemVisuals();
        var item = GetSelectedItem();
        if (item != null)
        {
            item.SetVisualState(1 , 1);
        }
    }
    
    public int GetSelectedIndex()
    {
        return currentSelectedIndex;
    }
    
    public DatePickerItem GetSelectedItem()
    {
        if (targetIndex >= 0 && targetIndex < items.Count)
        {
            return items[targetIndex];
        }
        return null;
    }
    
    public string GetSelectedValue()
    {
        if (targetIndex >= 0 && targetIndex < items.Count)
        {
            return items[targetIndex].GetText();
        }
        return "";
    }
}