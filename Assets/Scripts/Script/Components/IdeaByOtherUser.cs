using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IdeaByOtherUser : MonoBehaviour
{
    private const string API_URL = "http://104.131.173.123:8000";
    public List<WallpaperItemController> otherUserIdeas = new List<WallpaperItemController>();
    private List<WallpaperData> ideas = new List<WallpaperData>();

    private void Start()
    {
        StartCoroutine(GetIdeas());
    }
    
    private IEnumerator GetIdeas()
    {

        string wallpaperUrl = API_URL + "/wallpapers/ideas";
        using UnityWebRequest www = UnityWebRequest.Get(wallpaperUrl);
        www.SetRequestHeader("Accept", "application/json");
        Debug.Log("Requesting ideas from: " + wallpaperUrl);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Request failed: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            Debug.Log("Response: " + json);

            IdeasResponse response = JsonUtility.FromJson<IdeasResponse>(json);

            if (response is { ideas: not null })
            {
                ideas = response.ideas;
                for (int i = 0; i < ideas.Count; i++)
                {
                    ideas[i].image_url = API_URL + ideas[i].image_url;
                    otherUserIdeas[i].SetData(ideas[i]);
                }
            }
            else
            {
                Debug.LogError("Failed to parse or success = false.");
            }
        }
    }
}

[Serializable]
public class IdeasResponse
{
    public List<WallpaperData> ideas;
    public int total_available;
}
