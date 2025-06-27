using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPanelController : Singleton<MenuPanelController>
{
    public Notification notification;
    public SmoothOrbitCam smoothOrbitCam;
    public RenderTextureScaler renderTextureScaler;
    public List<GameObject> Panels;
    public GameObject LoadingPanel ;
    public RawImage TextureToDownload ;

    public GameObject Canvas,Camera,Podium,DownloadWholeTex;

    public TextMeshProUGUI m_InputName;
    public TextMeshProUGUI Name;

    public GameObject[] Models;

    public FullBodyTexController FullBodyTexController;

    private void Start()
    {
        // Set target frame rate
        Application.targetFrameRate = 60;
#if UNITY_IOS
        ActivePanel(0);
#else 
        ActivePanel(5);
#endif
        // if (PlayerPrefs.HasKey("Name"))
        // {
        //     Name.text = PlayerPrefs.GetString("Name");
        // }
        // else
        //      ActivePanel(3);
    }

    // Update is called once per frame
    public void ActivePanel(int i)
    {
        for (int j = 0; j < Panels.Count; j++)
        {
            Panels[j].SetActive(false);
        }

        Panels[i].SetActive(true);

        // Track screen view with AppsFlyer
        TrackScreenView(i);
    }

    private void TrackScreenView(int panelIndex)
    {
        string screenName = GetScreenNameFromIndex(panelIndex);

        // Track screen view through AnalyticsManager
        AnalyticsManager.Instance.TrackScreenView(screenName);
    }

    private string GetScreenNameFromIndex(int panelIndex)
    {
        // Map panel indices to meaningful screen names
        switch (panelIndex)
        {
            case 0: return "main_menu";
            case 1: return "skin_editor";
            case 2: return "3d_preview";
            case 3: return "user_profile";
            case 4: return "settings";
            case 5: return "subscription";
            case 6: return "purchase_confirmation";
            default: return "screen_" + panelIndex;
        }
    }

    public void Active3d()
    {
        Podium.SetActive(true);
        DownloadWholeTex.gameObject.SetActive(false);
        Canvas.SetActive(false);
        Camera.GetComponent<Camera>().enabled = false;

        SetModel(0);
        smoothOrbitCam.EnableOrbit();
        renderTextureScaler.BakeTextureModel();
        if (MenuController.Instance.CurrentCatalog == 3)
        {
            DownloadWholeTex.gameObject.SetActive(true);
        }

        // Track 3D preview mode
        AnalyticsManager.Instance.TrackScreenView("3d_preview");

        // Track content view with more details
        string contentType = "unknown";
        string contentId = "unknown";

        if (MenuController.Instance != null)
        {
            switch (MenuController.Instance.CurrentCatalog)
            {
                case 0:
                    contentType = "shirt";
                    break;
                case 1:
                    contentType = "pants";
                    break;
                case 2:
                    contentType = "tshirt";
                    break;
                case 3:
                    contentType = "fullbody";
                    break;
            }

            // Try to get pattern info for content ID
            contentId = contentType + "_" + MenuController.Instance.CurrentPattern;
        }

        AnalyticsManager.Instance.TrackContentView(contentId, contentType, contentId);
    }

    public void SetTextureDownload()
    {
        LoadingPanel.SetActive(true);
        TextureToDownload.texture = FullBodyTexController.CurrentRawTexture;

        // Track texture download event
        string skinType = "unknown";
        if (MenuController.Instance != null)
        {
            switch (MenuController.Instance.CurrentCatalog)
            {
                case 0: skinType = "shirt"; break;
                case 1: skinType = "pants"; break;
                case 2: skinType = "tshirt"; break;
                case 3: skinType = "fullbody"; break;
            }
        }

        AnalyticsManager.Instance.TrackSkinDownload("texture_download", skinType);

        // Save to gallery
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(
            (Texture2D)TextureToDownload.texture, "Roblox", "RobloxSkin.png",
            (success, path) => {
                Debug.Log("Media save result: " + success + " " + path);

                // Track success or failure
                Dictionary<string, string> saveParams = new Dictionary<string, string>
                {
                    { "success", success.ToString() },
                    { "skin_type", skinType }
                };
                AnalyticsManager.Instance.LogEvent("texture_save_to_gallery", saveParams);
            }
        );
        Debug.Log("Permission result: " + permission);

#if UNITY_EDITOR || UNITY_STANDALONE
        string path = System.IO.Path.Combine(Application.persistentDataPath, "RobloxSkin.png");

        // Ensure the texture is a Texture2D before encoding
        if (TextureToDownload.texture is Texture2D texture2D)
        {
            byte[] bytes = texture2D.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log("Image saved to: " + path);
        }
        else
        {
            Debug.LogError("TextureToDownload.texture is not a Texture2D and cannot be encoded to PNG.");
            AnalyticsManager.Instance.TrackError("texture_save", "Texture is not a Texture2D");
        }
#endif
    }

    public void SetModel(int x)
    {
        for (int i = 0; i < Models.Length; i++)
        {
            Models[i].SetActive(false);
        }
        Models[x].SetActive(true);

        if (x != 0)
        {
            Models[x].GetComponent<Animator>().Play("walking");
        }
    }

    public void Active2d()
    {
        Canvas.SetActive(true);
        Camera.GetComponent<Camera>().enabled = true;
        LoadingPanel.SetActive(false);
      //  Podium.SetActive(false);
        if (MenuController.Instance.CurrentCatalog == 3)
        {
            SceneManager.LoadScene(0);
        }


    }


    public void setName()
    {
        PlayerPrefs.SetString("Name",m_InputName.text);
        Name.text = PlayerPrefs.GetString("Name");
    }



}


