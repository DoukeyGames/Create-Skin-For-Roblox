using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ShapeController : Singleton<ShapeController>
{
    public List<TextureGroup> textureGroups = new List<TextureGroup>();

    public GameObject m_catalogPrefab;
    
    public Transform m_catalogParent;
    
    
    public GameObject m_catalogPatternPrefab;
    
    public Transform m_catalogPatternParent;
    
    public List<GameObject> PatternList;
    public List<GameObject> ColorList;
    
    public List<Color> textureColor = new List<Color>();
    public Color CurrentColor;
    
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
        for (int i = 0; i < textureColor.Count; i++)
        {
            
            GameObject g = Instantiate(m_catalogPrefab, m_catalogParent);
            int x = i;
            g.GetComponent<Button>().onClick.AddListener(delegate { ChangeColor(textureColor[x]); });
            g.transform.GetChild(0).GetComponent<Image>().color = textureColor[x];
            ColorList.Add(g);
            Text text= g.transform.GetChild(1).GetComponent<Text>();
            text.color = new Color(1, 1, 1, 0);

        }

        ShowCatalogPattern(0);
    }

    public void ChangeColor(Color color)
    {
        for (int i = 0; i < PatternList.Count; i++)
        {
            Image img =  PatternList[i].transform.GetChild(0).GetComponent<Image>();
            CurrentColor = color;
            img.color = color;
        }
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
            g.GetComponent<PatternButton>()._stickerImg = textureGroups[CurrentCatalog].Texture[i];
            g.GetComponent<PatternButton>()._StickertTexture = textureGroups[CurrentCatalog].RawTexture[i];
            PatternList.Add(g);
        }
    }
   

    public void ApplyTexture(int index)
    {
        CurrentPattern = index;
        CurrentTexture = textureGroups[CurrentCatalog].Texture[CurrentPattern];
        CurrentRawTexture = textureGroups[CurrentCatalog].RawTexture[CurrentPattern];
        
      
        PatternList[index].GetComponent<PatternButton>()._stickerHandler.currentSticker.GetComponent<Image>().color = CurrentColor;
       PatternList[index].GetComponent<PatternButton>()._stickerHandler.currentSticker.GetComponent<StickerDragger>().decal.GetComponent<Image>().color = CurrentColor;//.GetComponent<CwPaintDecal>().Color = CurrentColor;
        
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
        string resourcesPath = Path.Combine(Application.dataPath, "Resources/stickers");
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

            Sprite[] textures = Resources.LoadAll<Sprite>($"stickers/collar");
            if (textures.Length > 0)
            {
                for (int i = 0; i < textures.Length; i++)
                {
                    if (i % 2 != 0)
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

                //group.Texture.AddRange(textures);
                Debug.Log($"Loaded {textures.Length} textures for group: {folderName}");
            }
            // Add the group to the list
            textureGroups.Add(group);
        }
    }
}




