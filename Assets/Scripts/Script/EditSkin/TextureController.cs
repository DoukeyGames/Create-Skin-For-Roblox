using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TextureController : Singleton<TextureController>
{
    public List<TextureGroup> textureGroups = new List<TextureGroup>();

    public GameObject m_catalogPrefab;
    
    public Transform m_catalogParent;
    
    public GameObject m_catalogPatternPrefab;
    
    public Transform m_catalogPatternParent;
    
    public List<GameObject> PatternList;
    
    public Sprite CurrentTexture;
    public Texture CurrentRawTexture;
    public int CurrentCatalog;
    public int CurrentPattern;

    public Color Selected, UnSelected;
    public List<ItemSelector> CatalogBtnList;

    protected override void Awake()
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
                CatalogBtnList.Add(g.GetComponent<ItemSelector>());
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
            PatternList.Add(g);
        }
        CatalogBtnList[CurrentCatalog].Selected(); 
    }

   

    public void ApplyTexture(int index)
    {
        CurrentPattern = index;
        CurrentTexture = textureGroups[CurrentCatalog].Texture[CurrentPattern];
        CurrentRawTexture = textureGroups[CurrentCatalog].RawTexture[CurrentPattern];
        RobloxBodyHandler.Instance.CurrentTexture = CurrentTexture;
        RobloxBodyHandler.Instance.CurrentRawTexture = CurrentRawTexture;
        RobloxBodyHandler.Instance.CurrentColor = Color.white;
        SelectedPattern(index);
    }

    public void SelectedPattern(int index)
    {
        foreach (var UPPER in PatternList)
        {
            UPPER.transform.GetChild(2).GetComponent<Image>().color =UnSelected;
        }
        PatternList[index].transform.GetChild(2).GetComponent<Image>().color =Selected;

    }

    public void ClearPatternList()
    {
        for (int i = 0; i < CatalogBtnList.Count; i++)
        {
            CatalogBtnList[i].UnSelected(); 
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
        string resourcesPath = Path.Combine(Application.dataPath, "Resources/textures");
        if (!Directory.Exists(resourcesPath))
        {
            Debug.LogError($"Path not found: {resourcesPath}. Please ensure your folders are under Resources/textures.");
            return;
        }

        string[] folders = Directory.GetDirectories(resourcesPath);

        foreach (string folder in folders)
        {
            string folderName = Path.GetFileName(folder);

            TextureGroup group = new TextureGroup(folderName);

            Sprite[] textures = Resources.LoadAll<Sprite>($"textures/{folderName}");
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
                    group.RawTexture.Add(texture[i]);

                }

                Debug.Log($"Loaded {textures.Length} textures for group: {folderName}");
            }
            textureGroups.Add(group);
        }
    }
}
[Serializable]
public class TextureGroup
{
    public string GroupName;
    public bool Show;

    public List<Sprite> Texture;
    public List<Texture> RawTexture;

    public TextureGroup(string groupName)
    {
        GroupName = groupName;
        Texture = new List<Sprite>();
        RawTexture = new List<Texture>();
    }
}




