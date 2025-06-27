using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinController : MonoBehaviour
{
    public Image m_StyleImg;
    public TextMeshProUGUI m_Styletext;
    
    public void Clickbtn()
    {
        MenuPanelController.Instance.ActivePanel(1);
    }
}
