using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class StickerController : Singleton<StickerController>
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

   
    public List<ItemSelector> CatalogBtnList;

    protected override void Awake()
    {
        ShowCatalog();
    }

    public void ShowCatalog()
    {
        for (int i = 0; i < textureGroups.Count; i++)
        {
            {
                GameObject g = Instantiate(m_catalogPrefab, m_catalogParent);
                 Text text= g.transform.GetChild(1).GetComponent<Text>();
                 text.text = textureGroups[i].GroupName;
                 m_catalogPatternParent = g.transform.GetChild(0);
                 for (int z = 0; z < textureGroups[i].Texture.Count; z++)
                 {
                     GameObject gobj = Instantiate(m_catalogPatternPrefab, m_catalogPatternParent);
                     int x = i;
                     
                     Image img = gobj.transform.GetChild(0).GetComponent<Image>();
                     img.sprite = textureGroups[i].Texture[z];
                     gobj.GetComponent<PatternButton>()._stickerImg = textureGroups[i].Texture[z];
                     gobj.GetComponent<PatternButton>()._StickertTexture = textureGroups[i].RawTexture[z];
                     
                     PatternList.Add(gobj);
                 }
            }
        }

    }

    public void ApplyTexture(int index)
    {
        CurrentPattern = index;
        CurrentTexture = textureGroups[CurrentCatalog].Texture[CurrentPattern];
        CurrentRawTexture = textureGroups[CurrentCatalog].RawTexture[CurrentPattern];
        RobloxBodyHandler.Instance.CurrentTexture = CurrentTexture;
        RobloxBodyHandler.Instance.CurrentRawTexture = CurrentRawTexture;
        RobloxBodyHandler.Instance.CurrentColor = Color.white;

    }

    [ContextMenu("Load Textures")]
    private void LoadFoldersAndTextures()
    {
        string resourcesPath = Path.Combine(Application.dataPath,  "Resources/stickers");
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

                Debug.Log($"Loaded {textures.Length} textures for group: {folderName}");
            }
            else
            {
                Debug.LogWarning($"No textures found in folder: {folderName}");
            }

            
            Texture[] texture = Resources.LoadAll<Texture>($"stickers/{folderName}");
            if (texture.Length > 0)
            {
                foreach (var t in texture)
                {
                    group.RawTexture.Add(t);
                }

                Debug.Log($"Loaded {textures.Length} textures for group: {folderName}");
            }
            textureGroups.Add(group);
        }
    }
}




