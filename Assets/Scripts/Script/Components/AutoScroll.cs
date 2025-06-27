using System.Collections.Generic;
using UnityEngine;

public class AutoScroll: MonoBehaviour
{
    public float scrollSpeed = 100f;
    public List<RectTransform> scrollableElements;
    
    private void Update()
    {
        if (scrollableElements == null || scrollableElements.Count == 0)
        {
            return;
        }

        foreach (var element in scrollableElements)
        {
            if (element != null)
            {
                Vector3 position = element.localPosition;
                position.x -= scrollSpeed * Time.deltaTime;
                element.localPosition = position;
                
                if (position.x < -Screen.width)
                {
                    position.x = scrollableElements.Count * element.rect.width - Screen.width;
                    element.localPosition = position;
                }
            }
        }
    }
}