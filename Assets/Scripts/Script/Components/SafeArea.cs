using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    [SerializeField]
    private bool safeAreaEnabled = true;

    [SerializeField]
    private float minAreaLeft;

    [SerializeField]
    private float minAreaRight;

    [SerializeField]
    private float minAreaTop;

    [SerializeField]
    private float minAreaBottom;

    public bool SafeAreaEnabled
    {
        get => safeAreaEnabled;
        set
        {
            safeAreaEnabled = value;
            UpdateSafeArea();
        }
    }

    private void Awake()
    {
        UpdateSafeArea();
    }

    public void UpdateSafeArea()
    {
        var rectTransform = GetComponent<RectTransform>();
        var safeArea = UnityEngine.Screen.safeArea;
        if(safeAreaEnabled)
        {
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;
            var safeAreaLeft = safeArea.xMin - 0;
            var safeAreaRight = UnityEngine.Screen.width - safeArea.xMax;

            if (safeAreaLeft > 0 && safeAreaRight > 0)
            {
                if (safeAreaLeft < minAreaLeft)
                {
                    anchorMin.x = minAreaLeft;
                }

                if (safeAreaRight < minAreaRight)
                {
                    anchorMax.x = UnityEngine.Screen.width - minAreaRight;
                }

            } else if (safeAreaLeft <= 0 && safeAreaRight <= 0)
            {
                anchorMin.x = minAreaLeft;
                anchorMax.x = UnityEngine.Screen.width - minAreaRight;

            } else if (safeAreaLeft > 0 && safeAreaRight <= 0)
            {
                anchorMin.x = safeAreaLeft + 10;
                anchorMax.x = UnityEngine.Screen.width - (safeAreaLeft + 10);
            }
            else if(safeAreaLeft <= 0 && safeAreaRight > 0)
            {
                anchorMin.x = safeAreaRight + 10;
                anchorMax.x = UnityEngine.Screen.width - (safeAreaRight + 10);
            }

            if (safeArea.yMin < minAreaBottom)
            {
                anchorMin.y = minAreaBottom;
            }

            if (UnityEngine.Screen.height - safeArea.yMax < minAreaTop)
            {
                anchorMax.y = UnityEngine.Screen.height - minAreaTop;
            }

            anchorMin.x /= UnityEngine.Screen.width;
            anchorMin.y /= UnityEngine.Screen.height;

            anchorMax.x /= UnityEngine.Screen.width;
            anchorMax.y /= UnityEngine.Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
        else
        {
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.one;
        }
    }
}