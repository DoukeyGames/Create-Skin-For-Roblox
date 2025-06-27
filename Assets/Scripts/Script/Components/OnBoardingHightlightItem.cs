using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OnBoardingHightlightItem : MonoBehaviour
{
    public Image overlay;
    public void Show()
    {
        var childCount = transform.parent.childCount;
        transform.SetSiblingIndex(childCount - 1);
        var parentChildCount = transform.parent.parent.childCount;
        transform.parent.SetSiblingIndex(parentChildCount - 1);
        overlay.DOFade(0f, 0.5f).SetLink(gameObject);
        transform.DOScale(1.3f, 0.5f).SetLink(gameObject);
    }

    public void Hide()
    {
        overlay.DOFade(0.5f, 0.5f).SetLink(gameObject);
        transform.DOScale(1f, 0.5f).SetLink(gameObject);
        transform.SetSiblingIndex(0);
        transform.parent.SetSiblingIndex(0);
    }
}
