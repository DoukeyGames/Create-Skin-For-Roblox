using UnityEngine;
using UnityEngine.UI;

public class ItemSelector : MonoBehaviour
{
    public Sprite m_normalImage, m_highlightedImage;
    public Color m_normalText, m_highlightedText;
    public Color m_normalImageColor, m_highlightedImageColor;
    public Text m_Text;
    public Image m_Image;
    public bool BgColor;
  
    public void Selected()
    {
        if (BgColor) m_Image.color = m_highlightedImageColor;
        if (m_Image) m_Image.sprite = m_highlightedImage;
        if (m_Text) m_Text.color = m_highlightedText;
    }

    
    public void UnSelected()
    {
        if (BgColor) m_Image.color = m_normalImageColor;
        if (m_Image) m_Image.sprite = m_normalImage;
        if (m_Text) m_Text.color = m_normalText;
    }
}
