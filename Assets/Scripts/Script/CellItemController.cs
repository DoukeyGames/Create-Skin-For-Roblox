using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellItemController : MonoBehaviour
{
    public Image m_StyleImg;
    public TextMeshProUGUI m_StyleText;
    
    public void Tap()
    {
        ScreenManager.Instance.ActivePanel(1);
    }
}
