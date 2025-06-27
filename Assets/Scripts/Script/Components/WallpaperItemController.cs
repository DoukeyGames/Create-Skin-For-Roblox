using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WallpaperItemController : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI categoryText;
    public Image thumbnailImage;
    public Button downloadButton;
    private bool isDownloading = false;
    private WallpaperData wallpaperData;

    private void Start()
    {
        downloadButton.onClick.AddListener(Download);
    }

    public void SetData(WallpaperData data)
    {
        wallpaperData = data;
        titleText.text = data.title;
        categoryText.text = data.category;
        if (!string.IsNullOrEmpty(data.image_url))
        {
            CreateThumbnail(data.image_url);
        }
        else
        {
            thumbnailImage.sprite = null;
        }
    }

    private void CreateThumbnail(string imageUrl)
    {
        StartCoroutine(LoadImage(imageUrl));
    }

    IEnumerator LoadImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(www);
            thumbnailImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError("Failed to load image: " + www.error);
        }
    }

    private void Download()
    {
        if (isDownloading)
        {
            return;
        }

        if (wallpaperData != null)
        {
            AnalyticsManager.Instance.TrackFeatureUsed("download_wall_paper",
                new Dictionary<string, string>
                    { { "wallpaper_id", wallpaperData.id.ToString() }, { "wallpaper_title", wallpaperData.title } });
        }
        else
        {
            AnalyticsManager.Instance.TrackFeatureUsed("download_wall_paper",
                new Dictionary<string, string>
                    { { "wallpaper_id", "default_wallpaper" }, { "wallpaper_title", "default_wallpaper" } });
        }
        
        isDownloading = true;
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(
            thumbnailImage.sprite.texture, "Roblox", "RobloxSkinGenerate.png",
            (success, path) =>
            {
                isDownloading = false;
                Debug.Log("Media save result: " + success + " " + path);
                if (success)
                {
                    Debug.Log("Image saved successfully to: " + path);
                    MenuPanelController.Instance.notification.Show();
                    MenuPanelController.Instance.notification.SetNotificationText("Image Saved", "Image saved successfully to gallery");
                }
                else
                {
                    Debug.LogError("Failed to save image to gallery.");
                    MenuPanelController.Instance.notification.Show();
                    MenuPanelController.Instance.notification.SetNotificationText("Save Failed", "Failed to save image to gallery");
                }

                Dictionary<string, string> saveParams = new Dictionary<string, string>
                {
                    { "success", success.ToString() },
                };
                AnalyticsManager.Instance.LogEvent("ai_gen_save_to_gallery", saveParams);
            }
        );
        Debug.Log("Permission result: " + permission);

        // Handle permission denied case
        if (permission == NativeGallery.Permission.Denied)
        {
            isDownloading = false;
            MenuPanelController.Instance.notification.Show();
            MenuPanelController.Instance.notification.SetNotificationText("Permission Denied", "Please go to <b>settings</b> and allow gallery access to save images");

            // Log analytics for permission denied
            Dictionary<string, string> permissionParams = new Dictionary<string, string>
            {
                { "permission_status", "denied" },
                { "wallpaper_id", wallpaperData?.id.ToString() ?? "unknown" }
            };
            AnalyticsManager.Instance.LogEvent("gallery_permission_denied", permissionParams);
            return;
        }
        else if (permission == NativeGallery.Permission.ShouldAsk)
        {
            isDownloading = false;
            MenuPanelController.Instance.notification.Show();
            MenuPanelController.Instance.notification.SetNotificationText("Permission Required", "Gallery access is required to save images. Please try again and allow access.");

            // Log analytics for permission should ask
            Dictionary<string, string> permissionParams = new Dictionary<string, string>
            {
                { "permission_status", "should_ask" },
                { "wallpaper_id", wallpaperData?.id.ToString() ?? "unknown" }
            };
            AnalyticsManager.Instance.LogEvent("gallery_permission_should_ask", permissionParams);
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        string path = System.IO.Path.Combine(Application.persistentDataPath, "RobloxSkin.png");

        if (thumbnailImage.sprite.texture)
        {
            byte[] bytes = thumbnailImage.sprite.texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log("Image saved to: " + path);
        }
        else
        {
            Debug.LogError("TextureToDownload.texture is not a Texture2D and cannot be encoded to PNG.");
        }
#endif
    }
}
