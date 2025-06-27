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
        BodyController.Instance.CurrentTexture = CurrentTexture;
        BodyController.Instance.CurrentRawTexture = CurrentRawTexture;
        BodyController.Instance.CurrentColor = CurrentColor;
    }
    public void FinishApplyingTexture()
    {
        BodyController.Instance.CurrentTexture = null;
        BodyController.Instance.CurrentRawTexture = null;
        BodyController.Instance.CurrentColor = Color.clear;
    }
}
