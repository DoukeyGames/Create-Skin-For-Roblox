using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullCellController : MonoBehaviour
{
    public Image m_StyleImg;
    public GameObject border;
    public int index;
    
    public void Clickbtn()
    {
        ScreenManager.Instance.ActivePanel(1);
        FullBodyTexController.Instance.ApplyTexture(index);
    }
}
