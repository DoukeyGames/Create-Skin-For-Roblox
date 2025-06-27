using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AutoSelectItem : MonoBehaviour
{
    public Image overlay;
    public void Show()
    {
        var childCount = transform.parent.childCount;
        transform.SetSiblingIndex(childCount - 1);
        var parentChildCount = transform.parent.parent.childCount;
        transform.parent.SetSiblingIndex(parentChildCount - 1);
        overlay.DOFade(0f, 0.5f);
        transform.DOScale(1.3f, 0.5f);
    }
    
    public void Hide()
    {
        overlay.DOFade(0.5f, 0.5f);
        transform.DOScale(1f, 0.5f);
        transform.SetSiblingIndex(0);
        transform.parent.SetSiblingIndex(0);
    }
}
