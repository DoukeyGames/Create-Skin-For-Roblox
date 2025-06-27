using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
    public Image m_StyleImg;
    public TextMeshProUGUI m_StyleText;
    
    public void Clickbtn()
    {
        MenuPanelController.Instance.ActivePanel(1);
    }
}
