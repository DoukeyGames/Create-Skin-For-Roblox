using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text messageText;


    public void OpenSetting()
    {
        NativeGallery.OpenSettings();
    }
    
    public void SetNotificationText(string title, string message)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }
        
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
