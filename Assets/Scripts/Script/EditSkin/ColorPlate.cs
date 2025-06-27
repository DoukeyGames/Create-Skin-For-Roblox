using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlate : MonoBehaviour
{
    
    public Sprite CurrentTexture;
    public Texture CurrentRawTexture;
    public Color CurrentColor;
    
    
    public void ApplyTexture()
    {
        RobloxBodyHandler.Instance.CurrentTexture = CurrentTexture;
        RobloxBodyHandler.Instance.CurrentRawTexture = CurrentRawTexture;
        RobloxBodyHandler.Instance.CurrentColor = CurrentColor;
    }
    public void FinishApplyingTexture()
    {
        RobloxBodyHandler.Instance.CurrentTexture = null;
        RobloxBodyHandler.Instance.CurrentRawTexture = null;
        RobloxBodyHandler.Instance.CurrentColor = Color.clear;
    }
}
