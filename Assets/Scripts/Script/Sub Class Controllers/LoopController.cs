
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class LoopController : Singleton<LoopController>
{
    public List<TextureGroup> textureGroups = new List<TextureGroup>();
    public GameObject m_catalogPrefab;
    
    public Transform m_catalogParent;
    public GameObject m_catalogPatternPrefab;
    
    public Transform m_catalogPatternParent;
    
    //[HideInInspector]
    public List<GameObject> PatternList;
    
    public Sprite CurrentTexture;
    public Texture CurrentRawTexture;
    public int CurrentCatalog;
    public int CurrentPattern;

    public List<ThemeController> CatalogBtnList;
    public void Awake()
    {
        ShowCatalog();
    }

    public void ShowCatalog()
    {
        for (int i = 0; i < textureGroups.Count; i++)
        {
            if (textureGroups[i].Show)
            {
                GameObject g = Instantiate(m_catalogPrefab, m_catalogParent);
                int x = i;
                g.GetComponent<Button>().onClick.AddListener(delegate { ShowCatalogPattern(x); });
                CatalogBtnList.Add(g.GetComponent<ThemeController>());
                Text text= g.transform.GetChild(1).GetComponent<Text>();
                text.text = textureGroups[i].GroupName;
            }
        }

        ShowCatalogPattern(0);
    }
    
    public void ShowCatalogPattern(int index)
    {
        
        CurrentCatalog = index;
        ClearPatternList();
        for (int i = 0; i < textureGroups[CurrentCatalog].Texture.Count; i++)
        {
            GameObject g = Instantiate(m_catalogPatternPrefab, m_catalogPatternParent);
            int x = i;
            g.GetComponent<Button>().onClick.AddListener(delegate { ApplyTexture(x); });
            Image img = g.transform.GetChild(0).GetComponent<Image>();
            img.sprite = textureGroups[CurrentCatalog].Texture[i];
            g.GetComponent<PatternBtn>()._stickerImg = textureGroups[CurrentCatalog].Texture[i];
            g.GetComponent<PatternBtn>()._StickertTexture = textureGroups[CurrentCatalog].RawTexture[i];
            PatternList.Add(g);
        }
        CatalogBtnList[CurrentCatalog].SelectedState(); 
    }

   

    public void ApplyTexture(int index)
    {
        CurrentPattern = index;
        CurrentTexture = textureGroups[CurrentCatalog].Texture[CurrentPattern];
        CurrentRawTexture = textureGroups[CurrentCatalog].RawTexture[CurrentPattern];
        BodyController.Instance.CurrentTexture = CurrentTexture;
        BodyController.Instance.CurrentRawTexture = CurrentRawTexture;
        BodyController.Instance.CurrentColor = Color.white;

    }


    public void ClearPatternList()
    {
        for (int i = 0; i < CatalogBtnList.Count; i++)
        {
            CatalogBtnList[i].NormalState(); 
        }
        for (int i = 0; i < PatternList.Count; i++)
        {
            Destroy(PatternList[i]);
        }
        PatternList.Clear();
        
        
    }
    [ContextMenu("Load Textures")]
    private void LoadFoldersAndTextures()
    {
        string resourcesPath = Path.Combine(Application.dataPath, "Resources/stickers");
        if (!Directory.Exists(resourcesPath))
        {
            Debug.LogError($"Path not found: {resourcesPath}. Please ensure your folders are under Resources/textures.");
            return;
        }

        // Get all folders inside "Resources/textures"
        string[] folders = Directory.GetDirectories(resourcesPath);

        foreach (string folder in folders)
        {
            // Extract the folder name (e.g., "Group1")
            string folderName = Path.GetFileName(folder);

            // Create a new TextureGroup with the folder name as GroupName
            TextureGroup group = new TextureGroup(folderName);

            // Load all textures from this folder
            Sprite[] textures = Resources.LoadAll<Sprite>($"stickers/{folderName}");
            if (textures.Length > 0)
            {
                for (int i = 0; i < textures.Length; i++)
                {
                    if (i % 2 != 0) // Even index
                    {
                        if(group.Texture.Count <=10)
                        group.Texture.Add(textures[i]);
                    }
                }

                //group.Texture.AddRange(textures);
                Debug.Log($"Loaded {textures.Length} textures for group: {folderName}");
            }
            else
            {
                Debug.LogWarning($"No textures found in folder: {folderName}");
            }

            
            Texture[] texture = Resources.LoadAll<Texture>($"textures/{folderName}");
            if (texture.Length > 0)
            {
                for (int i = 0; i < texture.Length; i++)
                {
                    // if (i % 2 == 0) // Even index
                    // {
                    //     if(group.RawTexture.Count <=20)
                            group.RawTexture.Add(texture[i]);
               //     }
                }

                //group.Texture.AddRange(textures);
                Debug.Log($"Loaded {textures.Length} textures for group: {folderName}");
            }
            // Add the group to the list
            textureGroups.Add(group);
        }
    }
}




