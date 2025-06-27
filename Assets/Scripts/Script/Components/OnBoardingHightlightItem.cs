using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OnBoardingHightlightItem : MonoBehaviour
{
    public Image overlay;
    private Tween fadeTween;
    private Tween scaleTween;

    private void OnDisable()
    {
        fadeTween?.Kill();
        scaleTween?.Kill();
    }

    private void OnDestroy()
    {
        fadeTween?.Kill();
        scaleTween?.Kill();
    }

    public void Show()
    {
        fadeTween?.Kill();
        scaleTween?.Kill();
        var childCount = transform.parent.childCount;
        transform.SetSiblingIndex(childCount - 1);
        var parentChildCount = transform.parent.parent.childCount;
        transform.parent.SetSiblingIndex(parentChildCount - 1);
        fadeTween = overlay.DOFade(0f, 0.5f).SetLink(gameObject);
        scaleTween = transform.DOScale(1.3f, 0.5f).SetLink(gameObject);
    }

    public void Hide()
    {
        fadeTween?.Kill();
        scaleTween?.Kill();
        fadeTween = overlay.DOFade(0.5f, 0.5f).SetLink(gameObject);
        scaleTween = transform.DOScale(1f, 0.5f).SetLink(gameObject);
        transform.SetSiblingIndex(0);
        transform.parent.SetSiblingIndex(0);
    }
}
