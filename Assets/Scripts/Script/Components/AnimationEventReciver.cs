using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventReciver : MonoBehaviour
{
    public UnityEvent CompleteEvent;

    public void OnAnimationComplete()
    {
        CompleteEvent?.Invoke();
    }
}