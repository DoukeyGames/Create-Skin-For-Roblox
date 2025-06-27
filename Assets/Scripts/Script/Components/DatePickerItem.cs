using TMPro;
using UnityEngine;

[System.Serializable]
public class DatePickerItem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textComponent;
    public CanvasGroup canvasGroup;
    
    private string itemText;
    private bool isPaddingItem;
    
    
    public void SetData(string text, bool isPadding)
    {
        itemText = text;
        isPaddingItem = isPadding;
        
        if (textComponent != null)
        {
            textComponent.text = isPaddingItem ? "" : text;
        }
    }
    
    public void SetVisualState(float scale, float alpha)
    {
        if (!isPaddingItem)
        {
            transform.localScale = Vector3.one * scale;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
        }
        else
        {
            transform.localScale = Vector3.one;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }
    }
    
    public string GetText()
    {
        return itemText;
    }
}