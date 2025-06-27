using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class WallpaperHandler : MonoBehaviour
{
    private const string API_URL = "http://104.131.173.123:8000";
    public int limit = 0;
    public GameObject wallpaperItemPrefab;
    public GameObject wallpaperContentPanel;
    public List<WallpaperData> wallpapers = new List<WallpaperData>();
    
    public UnityAction OnWallpapersFetched;

    private void Start()
    {
        FetchWallpapers();
    }
    
    public void FetchWallpapers()
    {
        StartCoroutine(GetWallpapers());
    }
    
    private IEnumerator GetWallpapers()
    {

        string wallpaperUrl = API_URL+ "/wallpapers?limit=" + limit;
        using UnityWebRequest www = UnityWebRequest.Get(wallpaperUrl);
        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Request failed: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            Debug.Log("Response: " + json);

            WallpaperResponse response = JsonUtility.FromJson<WallpaperResponse>(json);

            if (response is { wallpapers: not null })
            {
                wallpapers = response.wallpapers;
                foreach (WallpaperData wallpaper in wallpapers)
                {
                    wallpaper.image_url = API_URL + wallpaper.image_url;
                    GameObject item = Instantiate(wallpaperItemPrefab, wallpaperContentPanel.transform);
                    WallpaperItemController itemController = item.GetComponent<WallpaperItemController>();
                    if (itemController != null)
                    {
                        itemController.SetData(wallpaper);
                    }
                }
                OnWallpapersFetched?.Invoke();
            }
            else
            {
                Debug.LogError("Failed to parse or success = false.");
            }
        }
    }
}

[Serializable]
public class WallpaperData
{
    public int id;
    public string title;
    public string category;
    public string image_url;
    public string prompt;
}

[Serializable]
public class WallpaperResponse
{
    public List<WallpaperData> wallpapers;
    public int total;
    public int limit;
}

