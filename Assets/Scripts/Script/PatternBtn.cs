using System.Collections;
using System.Collections.Generic;
using PaintIn3D;
using UnityEngine;
using UnityEngine.UI;

public class PatternBtn : MonoBehaviour
{
    public StickerHandler _stickerHandler;
    public Sprite _stickerImg;
    public Texture _StickertTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        _stickerHandler = StickerHandler.Instance;
    }

    public void setSticker()
    {
        if (_stickerHandler)
        {
              _stickerHandler.CreateSticker();
              _stickerHandler.currentSticker.GetComponent<Image>().sprite = _stickerImg;
              _stickerHandler.currentSticker.GetComponent<UIDragAndSnap>().decal.GetComponent<Image>().sprite = _stickerImg;
//              _stickerHandler.currentSticker.GetComponent<UIDragAndSnap>().decal.GetComponent<CwPaintDecal>().Texture =_StickertTexture;

              // Track sticker/pattern selection with AppsFlyer
              if (AnalyticsManager.Instance != null)
              {
                  Dictionary<string, string> stickerParams = new Dictionary<string, string>
                  {
                      { "sticker_name", _stickerImg != null ? _stickerImg.name : "unknown" },
                      { "action", "sticker_applied" }
                  };
                  AnalyticsManager.Instance.TrackFeatureUsed("sticker_selection", stickerParams);
              }

              if (SubMenuController.Instance)
              {
                  SubMenuController.Instance.closeCategory();
              }
        }
    }
    

}
