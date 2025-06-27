using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AIGenerator : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject loadingPanel;
    public GameObject completePanel;
    
    public TMP_InputField promptInput;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI loadingText;
    public Image uiImage;
    private string access_token;
    private string refresh_token;
    
    private string apiUrl = "http://104.131.173.123:8000/";
    private string appId = "skins.maker.for.rbx";
    
    private bool isLoading = false;
    private float loadingStep = 0.05f;
    private bool isValidated;
    private bool isDownloading = false;
    private bool isSharing = false;
    private Coroutine generateCoroutine;

    public void ResetAll()
    {
        mainPanel.SetActive(true);
        completePanel.SetActive(false);
        loadingPanel.SetActive(false);
        uiImage.sprite = null;
        warningText.gameObject.SetActive(false);
        isLoading = false;
        isDownloading = false;
        isSharing = false;
        StopAllCoroutines();
        if(generateCoroutine != null)
        {
            StopCoroutine(generateCoroutine);
        }
    }
    
    private void Start()
    {
        warningText.gameObject.SetActive(false);
        promptInput.onSelect.AddListener((text) => {
            warningText.gameObject.SetActive(false);
        });
        access_token = PlayerPrefs.GetString("access_token", "");
        refresh_token = PlayerPrefs.GetString("refresh_token", "");

        Debug.Log("Access Token: " + access_token);
        Debug.Log("Refresh Token: " + refresh_token);
        
        if (string.IsNullOrEmpty(access_token) || string.IsNullOrEmpty(refresh_token))
        {
            StartCoroutine(RegisterDevice());
        }
        else
        {
            StartCoroutine(ValidateToken(access_token, refresh_token));
            Debug.Log("Device already registered.");
        }
    }

    IEnumerator OnLoading()
    {
        float progress = 0f;
        
        while (!isLoading)
        {
            progress = Mathf.Min(progress + Time.deltaTime * loadingStep, 0.95f);
            loadingText.text = Mathf.RoundToInt(progress * 100) + "%";
            yield return null;
        }
        
        loadingText.text = "100%";
        yield return new WaitForSeconds(0.5f);
    }
    
    public void GenerateImage()
    {
        if( generateCoroutine != null )
        {
            StopCoroutine(generateCoroutine);
        }
        if (!isValidated)
        {
            warningText.text = "Token not validated. Please try again later.";
            warningText.gameObject.SetActive(true);
            Debug.LogWarning("Token not validated, cannot generate image.");
            return;
        }
        warningText.gameObject.SetActive(false);
        AnalyticsManager.Instance.TrackFeatureUsed("generate_image_create_with_ai", 
            new Dictionary<string, string> { { "prompt", promptInput.text } });
        warningText.gameObject.SetActive(false);
        string prompt = promptInput.text;
        if (string.IsNullOrEmpty(prompt))
        {
            warningText.text = "Prompt cannot be empty!";
            warningText.gameObject.SetActive(true);
            return;
        }

        generateCoroutine = StartCoroutine(RequestGen(prompt));
    }
    
    public void Share()
    {
        if (isSharing)
        {
            return;
        }
        AnalyticsManager.Instance.TrackFeatureUsed("share_image_create_with_ai", 
            new Dictionary<string, string> { { "prompt", promptInput.text } });
        StartCoroutine( TakeScreenshotAndShare() );
    }
    
    private IEnumerator TakeScreenshotAndShare()
    {
        isSharing = true;
        yield return new WaitForEndOfFrame();
        new NativeShare().AddFile(uiImage.sprite.texture, "RobloxSkin.png")
            .SetText("Hey Check Out this New Roblox Game")
            .SetCallback( ( result, shareTarget ) => {
                isSharing = false;
                Debug.Log( "Share result: " + result + ", selected app: " + shareTarget );

                // Track share result
                Dictionary<string, string> shareResultParams = new Dictionary<string, string>
                {
                    { "result", result.ToString() },
                    { "target_app", shareTarget ?? "unknown" }
                };
                AnalyticsManager.Instance.LogEvent("share_image_create_with_ai", shareResultParams);
            })
            .Share();
    }

    public void Download()
    {
        if (isDownloading)
        {
            return;
        }
        AnalyticsManager.Instance.TrackFeatureUsed("download_image_create_with_ai", 
            new Dictionary<string, string> { { "prompt", promptInput.text } });
        isDownloading = true;
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(
            uiImage.sprite.texture, "Roblox", "RobloxSkinGenerate.png",
            (success, path) => {
                ScreenManager.Instance.notification.Show();
                if (success)
                {
                    ScreenManager.Instance.notification.SetNotificationText("Image Saved", "Your generated image has been saved to the gallery.");
                }
                else
                {
                    ScreenManager.Instance.notification.SetNotificationText("Save Failed", "Failed to save the image. Please try again.");
                }
                
                isDownloading = false;
                Debug.Log("Media save result: " + success + " " + path);

                Dictionary<string, string> saveParams = new Dictionary<string, string>
                {
                    { "success", success.ToString() },
                };
                AnalyticsManager.Instance.LogEvent("ai_gen_save_to_gallery", saveParams);
            }
        );
        Debug.Log("Permission result: " + permission);

#if UNITY_EDITOR || UNITY_STANDALONE
        string path = System.IO.Path.Combine(Application.persistentDataPath, "RobloxSkin.png");

        if (uiImage.sprite.texture)
        {
            byte[] bytes = uiImage.sprite.texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log("Image saved to: " + path);
        }
        else
        {
            Debug.LogError("TextureToDownload.texture is not a Texture2D and cannot be encoded to PNG.");
        }
#endif
    }

    IEnumerator RequestGen(string prompt)
    {
        isLoading = false;
        loadingPanel.SetActive(true);
        StartCoroutine(OnLoading());
        string url = apiUrl + "generate/unified";
        GenerateImageRequest request = new GenerateImageRequest { prompt = prompt };
        string jsonFinal = JsonUtility.ToJson(request);
        Debug.Log("Request prompt: " + prompt);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonFinal);

        access_token = PlayerPrefs.GetString("access_token", "");
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + access_token);
        Debug.Log("Gen with access_token: " + access_token);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            isLoading = true;
            loadingPanel.SetActive(false);
            mainPanel.SetActive(true);
            warningText.gameObject.SetActive(true);
            var errorResponse = JsonUtility.FromJson<ErrorResponse>(www.downloadHandler.text);
            warningText.text = errorResponse.detail.error;
            Debug.LogError("Error: " + www.error);
            Debug.LogError("Server response: " + www.downloadHandler.text);
            AnalyticsManager.Instance.TrackError("ai_gen_request_err", errorResponse.detail.error);
        }
        else
        {
            isLoading = true;
            loadingPanel.SetActive(false);
            mainPanel.SetActive(false);
            warningText.gameObject.SetActive(false);
            AnalyticsManager.Instance.LogEvent("ai_gen_request_success", 
                new Dictionary<string, string> { { "prompt", prompt } });
            LoadImage(www.downloadHandler.text);
            Debug.Log("Response: " + www.downloadHandler.text);
        }
    }

    private void LoadImage(string json)
    {
        isLoading = false;
        loadingPanel.SetActive(false);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Received empty response.");
            return;
        }
        ImageResponse response = JsonUtility.FromJson<ImageResponse>(json);

        if (response.success && response.images.Length > 0)
        {
            completePanel.SetActive(true);
            string base64 = response.images[0].image_data;
            if (base64.StartsWith("data:image"))
            {
                int commaIndex = base64.IndexOf(',');
                base64 = base64.Substring(commaIndex + 1);
            }
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64);

                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(imageBytes))
                {
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    uiImage.sprite = sprite;
                }
                else
                {
                    Debug.LogError("Failed to load texture from byte data.");
                }
            }
            catch (FormatException ex)
            {
                Debug.LogError("Invalid base64: " + ex.Message);
                Debug.LogError("Base64 snippet: " + base64.Substring(0, 100));
            }
        }
        else
        {
            Debug.LogError("Invalid image response.");
        }
    }

    IEnumerator ValidateToken(string accessToken, string refreshToken)
    {
        string validateUrl = apiUrl+ "device-auth/validate?token=" + accessToken;

        UnityWebRequest www = UnityWebRequest.PostWwwForm(validateUrl, "");

        www.SetRequestHeader("Accept", "application/json");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Validate failed: " + www.error);
            Debug.LogError("Response: " + www.downloadHandler.text);
            isValidated = false;
            AnalyticsManager.Instance.TrackError("device_validate_err", www.error);
            StartCoroutine(RefreshToken());
        }
        else
        {
            Debug.Log("Token validated: " + www.downloadHandler.text);
            isValidated = true;
            PlayerPrefs.SetString("access_token", accessToken);
            PlayerPrefs.SetString("refresh_token", refreshToken);
            PlayerPrefs.Save();
            AnalyticsManager.Instance.LogEvent("device_validate_success",
                new Dictionary<string, string>
                {
                    { "access_token", accessToken },
                    { "refresh_token", refresh_token },
                    { "device_id", SystemInfo.deviceUniqueIdentifier },
                    { "device_type", "ios" },
                    { "device_name", SystemInfo.deviceName },
                    { "app_version", Application.version },
                    { "os_version", SystemInfo.operatingSystem },
                    { "app_bundle_id", appId }
                });
        }
    }

    IEnumerator RefreshToken()
    {
        string refreshUrl = apiUrl + "device-auth/refresh";
        string jsonFinal = JsonUtility.ToJson(new { refresh_token = refresh_token });

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonFinal);

        UnityWebRequest www = new UnityWebRequest(refreshUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Refresh failed: " + www.error);
            Debug.LogError("Response: " + www.downloadHandler.text);
        }
        else
        {
            var response = JsonUtility.FromJson<RegisterResponse>(www.downloadHandler.text);
            if (response != null && !string.IsNullOrEmpty(response.access_token))
            {
                PlayerPrefs.SetString("access_token", response.access_token);
                PlayerPrefs.SetString("refresh_token", response.refresh_token);
                PlayerPrefs.Save();
                Debug.Log("Token refreshed successfully.");
                StartCoroutine(ValidateToken(response.access_token, response.refresh_token));
            }
            else
            {
                Debug.LogError("Invalid refresh response.");
            }
        }
    }

    IEnumerator RegisterDevice()
    {
        string url = apiUrl + "device-auth/register";
        string jsonFinal = JsonUtility.ToJson(new DeviceData
        {
            device_id = SystemInfo.deviceUniqueIdentifier,
            device_type = "ios",
            device_name = SystemInfo.deviceName,
            app_version = Application.version,
            os_version = SystemInfo.operatingSystem,
            app_bundle_id = appId
        });

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonFinal);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + www.error);
            Debug.LogError("Server response: " + www.downloadHandler.text);
            isValidated = false;
            AnalyticsManager.Instance.TrackError("device_register_error", www.error);
        }
        else
        {
            Debug.Log("Response: " + www.downloadHandler.text);
            var response = JsonUtility.FromJson<RegisterResponse>(www.downloadHandler.text);
            if (response != null && !string.IsNullOrEmpty(response.access_token))
            {
                PlayerPrefs.SetString("access_token", response.access_token);
                PlayerPrefs.SetString("refresh_token", response.refresh_token);
                PlayerPrefs.Save();
                AnalyticsManager.Instance.LogEvent("register_device_success",
                    new Dictionary<string, string>
                    {
                        { "access_token", response.access_token },
                        { "refresh_token", response.refresh_token },
                        {"device_id", SystemInfo.deviceUniqueIdentifier},
                        {"device_type", "ios"},
                        {"device_name", SystemInfo.deviceName},
                        {"app_version", Application.version},
                        {"os_version", SystemInfo.operatingSystem},
                        {"app_bundle_id", appId}
                    });
                StartCoroutine(ValidateToken(response.access_token, response.refresh_token));
            }
        }
    }
}

[System.Serializable]
public class DeviceData
{
    public string device_id;
    public string device_type;
    public string device_name;
    public string app_version;
    public string os_version;
    public string app_bundle_id;
}

[System.Serializable]
public class GenerateImageRequest
{
    public string prompt;
}


[System.Serializable]
public class RegisterResponse
{
    public string access_token;
    public string refresh_token;
}

[System.Serializable]
public class ImageItem
{
    public string image_data;
}

[System.Serializable]
public class ImageResponse
{
    public bool success;
    public ImageItem[] images;
}

[Serializable]
public class ErrorDetails
{
    public string original_error;
}

[Serializable]
public class Detail
{
    public bool success;
    public string error;
    public string error_code;
    public ErrorDetails details;
    public string timestamp;
}

[Serializable]
public class ErrorResponse
{
    public Detail detail;
}

