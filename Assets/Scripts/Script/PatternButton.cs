using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatternButton : MonoBehaviour
{
    public StickerHandler _stickerHandler;
    public Sprite _stickerImg;
    public Texture _StickertTexture;
    
    void Start()
    {
        _stickerHandler = StickerHandler.Instance;
    }

    public void ApplySticker()
    {
        if (_stickerHandler)
        {
              _stickerHandler.CreateSticker();
              _stickerHandler.currentSticker.GetComponent<Image>().sprite = _stickerImg;
              _stickerHandler.currentSticker.GetComponent<StickerDragger>().decal.GetComponent<Image>().sprite = _stickerImg;

              if (AnalyticsManager.Instance != null)
              {
                  Dictionary<string, string> stickerParams = new Dictionary<string, string>
                  {
                      { "sticker_name", _stickerImg != null ? _stickerImg.name : "unknown" },
                      { "action", "sticker_applied" }
                  };
                  AnalyticsManager.Instance.TrackFeatureUsed("sticker_selection", stickerParams);
              }

              if (SubTabController.Instance)
              {
                  SubTabController.Instance.CloseTab();
              }
        }
    }
    

}
