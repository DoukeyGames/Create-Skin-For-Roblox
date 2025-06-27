using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OnBoardingHighlight : MonoBehaviour
{
    public List<OnBoardingHightlightItem> highlightableImages1;
    public List<OnBoardingHightlightItem> highlightableImages2;

    private OnBoardingHightlightItem currentImage1 => highlightableImages1[0];
    private OnBoardingHightlightItem currentImage2 => highlightableImages1[1];
    private OnBoardingHightlightItem currentImage3 => highlightableImages2[0];
    private OnBoardingHightlightItem currentImage4 => highlightableImages2[1];

    private void Start()
    {
        Play();
    }

    private void Play()
    {
        var doVirtual1 = DOVirtual.DelayedCall(0.4f, () =>
        {
            currentImage4.Hide();
            currentImage3.Hide();
            currentImage2.Hide();
            currentImage1.Show();
        });
        
        var doVirtual2 = DOVirtual.DelayedCall(2f, () =>
        {
            currentImage4.Hide();
            currentImage3.Hide();
            currentImage1.Hide();
            currentImage2.Show();
        });
        
        var doVirtual3 = DOVirtual.DelayedCall(3.6f, () =>
        {
            currentImage4.Hide();
            currentImage1.Hide();
            currentImage2.Hide();
            currentImage3.Show();
        });
        
        
        var doVirtual4 = DOVirtual.DelayedCall(5.2f, () =>
        {
            currentImage1.Hide();
            currentImage2.Hide();
            currentImage3.Hide();
            currentImage4.Show();
        });
        
        var doVirtual5 = DOVirtual.DelayedCall(6.8f, ()=> {});
        
        var squence = DOTween.Sequence();
        squence.Append(doVirtual1)
            .Append(doVirtual2)
            .Append(doVirtual3)
            .Append(doVirtual4)
            .Append(doVirtual5)
            .SetLoops(-1, LoopType.Restart)
            .OnComplete(() => { Play(); });
    }
}
