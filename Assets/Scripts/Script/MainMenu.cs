using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Singleton<MainMenu>
{
    public WallpaperFetcher wallpaperFetcher;
    public TextMeshProUGUI m_selectedText;
    public int m_maxShirts;
    public int m_maxPants;
    public int m_maxSkins;
    public GridLayoutGroup m_SkinBookGridLayout;

    public GameObject m_cellPrefab,m_fullbodycellPrefab, wallpaperPrefab;


    public Transform m_ShirtParent, m_PantParent,m_FullbodyParent, wallpaperParent;


    [HideInInspector]
    public List<GameObject> ShirtsCell,PantsCell,FullbodyCell;


    public List<CategoryTextures> allCategoryTextures = new List<CategoryTextures>();
    public FullBodyTexController FullBodyTexController;
    public ScreenManager screenManager;
    public RobloxBodyHandler mRobloxBodyHandler;
    
    private List<FullCellController> ShirtsCellController = new List<FullCellController>();
    private List<FullCellController> PantsCellController = new List<FullCellController>();
    private List<FullCellController> TShirtControllers = new List<FullCellController>();
    
    private List<WallpaperItemController> WallpaperItemControllers = new List<WallpaperItemController>();
    void Start()
    {
        GenerateShirtCell();
        GeneratePantCell();
        ShowShirtCatalog();
        GenerateFullBodyCell();
    }

    private void OnEnable()
    {
        GenerateWallpaper();
        Resize();
    }

    private void Resize()
    {
        var ratio = Screen.width / (float)Screen.height;
        if (ratio < 0.5f)
        {
            m_SkinBookGridLayout.constraintCount = 2;
        }
        else
        {
            m_SkinBookGridLayout.constraintCount = 1;
        }
    }

    private void GenerateWallpaper()
    {
        if (WallpaperItemControllers.Count >= wallpaperFetcher.wallpapers.Count)
        {
            return;
        }
        for (int i = 0; i < wallpaperFetcher.wallpapers.Count; i++)
        {
            GameObject g = Instantiate(wallpaperPrefab, wallpaperParent);
            WallpaperItemController wallpaperItemController = g.GetComponent<WallpaperItemController>();
            WallpaperItemControllers.Add(wallpaperItemController);
            wallpaperItemController.SetData(wallpaperFetcher.wallpapers[i]);
        }
    }

    public void GenerateImage()
    {
        AnalyticsManager.Instance.TrackFeatureUsed("generate_image");
    }

    public void ShowShirtCatalog()
    {
        CurrentCatalog = 0;
        m_selectedText.text = "Shirt";

        // Track catalog view with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.TrackCatalogView("shirt", 0);
        }
    }
    public void ShowPantCatalog()
    {
        CurrentCatalog = 1;
        m_selectedText.text = "Pant";

        // Track catalog view with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.TrackCatalogView("pants", 1);
        }
    }
    public void ShowtshirtCatalog()
    {
        CurrentCatalog = 2;
        m_selectedText.text = "T-Shirt";

        // Track catalog view with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.TrackCatalogView("tshirt", 2);
        }
    }
    public void ShowFullbodyCatalog()
    {
        CurrentCatalog = 3;
        m_selectedText.text = "Full Body";

        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.TrackCatalogView("fullbody", 3);
        }
    }
    public void GenerateShirtCell()
    {
        for (int i = 0; i < m_maxShirts; i++)
        {
            GameObject g = Instantiate(m_cellPrefab, m_ShirtParent);
            CellItemController cellItemController =g.GetComponent<CellItemController>();
            var sprite = Resources.Load<Sprite>("masks/1_shirt/mask_"+i+"/preview_img_icon");
            int x = i;
            g.GetComponent<Button>().onClick.AddListener(delegate { SetModel(x); });
            cellItemController.m_StyleImg.sprite = sprite;
            var index = i + 1;
            if (cellItemController.m_StyleText)
            {
                cellItemController.m_StyleText.text = $"Shirt "  + (index <= 9 ? "0" + index : index.ToString());
            }
            ShirtsCell.Add(g);
        }
    }
    
    public void GeneratePantCell()
    {
        for (int i = 0; i < m_maxPants; i++)
        {
            GameObject g = Instantiate(m_cellPrefab, m_PantParent);
            CellItemController cellItemController =g.GetComponent<CellItemController>();
            var sprite = Resources.Load<Sprite>("masks/2_pants/mask_"+i+"/preview_img_icon");
            int x = i;
            g.GetComponent<Button>().onClick.AddListener(delegate { SetModel(x); });
            cellItemController.m_StyleImg.sprite = sprite;
            var index = i + 1;
            if (cellItemController.m_StyleText)
            {
                cellItemController.m_StyleText.text = $"Pant "  + (index <= 9 ? "0" + index : index.ToString());
            }
            PantsCell.Add(g);
        }
    }

    public void ShowOnlyShirt()
    {
        ShirtsCellController.ForEach(i => i.gameObject.SetActive(true));
        PantsCellController.ForEach(i => i.gameObject.SetActive(false));
        TShirtControllers.ForEach(i => i.gameObject.SetActive(false));
        SelectFirstShirt();
    }

    private void SelectFirstShirt()
    {
        if (ShirtsCellController.Count > 0)
        {
            ShirtsCellController[0].border.gameObject.SetActive(true);
            SetModelFullBody(0, 0);
        }
    }
    
    private void SelectFirstPant()
    {
        if (PantsCellController.Count > 0)
        {
            PantsCellController[0].border.gameObject.SetActive(true);
            SetModelFullBody(1, 0);
        }
    }
    
    public void SelectFirstTShirt()
    {
        if (TShirtControllers.Count > 0)
        {
            TShirtControllers[0].border.gameObject.SetActive(true);
            SetModelFullBody(0, 0);
        }
    }
    
    public void ShowOnlyPant()
    {
        ShirtsCellController.ForEach(i => i.gameObject.SetActive(false));
        PantsCellController.ForEach(i => i.gameObject.SetActive(true));
        TShirtControllers.ForEach(i => i.gameObject.SetActive(false));
        SelectFirstPant();
    }
    
    public void ShowOnlyTshirt()
    {
        ShirtsCellController.ForEach(i => i.gameObject.SetActive(false));
        PantsCellController.ForEach(i => i.gameObject.SetActive(false));
        TShirtControllers.ForEach(i => i.gameObject.SetActive(true));
        SelectFirstTShirt();
    }
    
    public void GenerateFullBodyCell()
    {
        for (int i = 1; i < m_maxSkins; i++)
        {
            GameObject g = Instantiate(m_fullbodycellPrefab, m_FullbodyParent);
            FullCellController skinController =g.GetComponent<FullCellController>();
            var sprite = Resources.Load<Sprite>($"textures/body_shirt/{i}_icon" );
            int x = i-1;
             g.GetComponent<Button>().onClick.AddListener(() =>
             {
                 SetModelFullBody(0,x);
                 ShirtsCellController.ForEach(i => i.border.gameObject.SetActive(false));
                 skinController.border.gameObject.SetActive(true);
             });

            skinController.m_StyleImg.sprite = sprite;
            ShirtsCellController.Add(skinController);
        }
        
        for (int i = 1; i < m_maxSkins; i++)
        {
            GameObject g = Instantiate(m_fullbodycellPrefab, m_FullbodyParent);
            FullCellController skinController =g.GetComponent<FullCellController>();
            var sprite = Resources.Load<Sprite>($"textures/body_shirt/{i}_icon" );
            int x = i-1;
            g.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetModelFullBody(0,x);
                TShirtControllers.ForEach(i => i.border.gameObject.SetActive(false));
                skinController.border.gameObject.SetActive(true);
            });

            skinController.m_StyleImg.sprite = sprite;
            TShirtControllers.Add(skinController);

        }
        
        for (int i = 1; i < m_maxSkins; i++)
        {
            GameObject g = Instantiate(m_fullbodycellPrefab, m_FullbodyParent);
            FullCellController skinController =g.GetComponent<FullCellController>();
            var sprite = Resources.Load<Sprite>($"textures/body_pant/{i}_icon");
            int x = i-1;
            g.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetModelFullBody(1,x);
                PantsCellController.ForEach(i => i.border.gameObject.SetActive(false));
                skinController.border.gameObject.SetActive(true);
            });

              skinController.m_StyleImg.sprite = sprite;
            PantsCellController.Add(skinController);
        }

    }

    public void highLightFullbodycell(int z)
    {
        for (int i = 0; i < FullbodyCell.Count; i++)
        {
            FullbodyCell[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        FullbodyCell[z].transform.GetChild(0).gameObject.SetActive(true);

    }
    
    public Sprite CurrentShirtTexture;
    public Sprite CurrentBShirtTexture;
    public Sprite CurrentPartTexture;
    public Texture CurrentMaskTexture;
    public int CurrentCatalog;
    public int CurrentPattern;

    public void SetModel(int index)
    {
        // Track skin selection event
        string skinType = "unknown";
        switch (CurrentCatalog)
        {
            case 0: skinType = "shirt"; break;
            case 1: skinType = "pants"; break;
            case 2: skinType = "tshirt"; break;
            case 3: skinType = "fullbody"; break;
        }

        Dictionary<string, string> selectionParams = new Dictionary<string, string>
        {
            { "skin_type", skinType },
            { "pattern_index", index.ToString() }
        };
        AnalyticsManager.Instance.TrackFeatureUsed("skin_selection", selectionParams);

        CurrentPattern = index;
        CurrentShirtTexture = allCategoryTextures[CurrentCatalog].FirstTextures[CurrentPattern];
        if( allCategoryTextures[CurrentCatalog].BackMask.Count>0)
            CurrentBShirtTexture = allCategoryTextures[CurrentCatalog].BackMask[CurrentPattern];
        CurrentPartTexture = allCategoryTextures[CurrentCatalog].SecondTextures[CurrentPattern];
        CurrentMaskTexture= allCategoryTextures[CurrentCatalog].Mask[CurrentPattern];
        RenderTextureScaler.instance.maskedTexture = (Texture2D)CurrentMaskTexture;
        if (CurrentCatalog == 0)
        {
            mRobloxBodyHandler.SetShirtMask(CurrentShirtTexture,CurrentBShirtTexture, CurrentPartTexture,CurrentMaskTexture);
        }
        else if (CurrentCatalog == 2)
        {
            mRobloxBodyHandler.SettShirtMask(CurrentShirtTexture,CurrentBShirtTexture, CurrentPartTexture,CurrentMaskTexture);
        }
        else
        {
             mRobloxBodyHandler.SetPantMask(CurrentShirtTexture,CurrentBShirtTexture, CurrentPartTexture,CurrentMaskTexture);
        }
    }

    private void SetModelFullBody(int i,int index)
    {
        Dictionary<string, string> fullbodyParams = new Dictionary<string, string>
        {
            { "skin_type", "full_body" },
            { "group_index", i.ToString() },
            { "texture_index", index.ToString() }
        };
        AnalyticsManager.Instance.TrackFeatureUsed("full_body_selection", fullbodyParams);

        CurrentPattern = i;
        CurrentCatalog = 3;
        CurrentShirtTexture = allCategoryTextures[CurrentCatalog].FirstTextures[CurrentPattern];
        CurrentPartTexture = allCategoryTextures[CurrentCatalog].SecondTextures[CurrentPattern];
        CurrentMaskTexture= allCategoryTextures[CurrentCatalog].Mask[CurrentPattern];
        RenderTextureScaler.instance.maskedTexture = (Texture2D)CurrentMaskTexture;

        if (i == 0)
        {
            mRobloxBodyHandler.SetShirtMask(CurrentShirtTexture,CurrentBShirtTexture, CurrentPartTexture, CurrentMaskTexture);
        }
        else
        {
            mRobloxBodyHandler.SetPantMask(CurrentShirtTexture,CurrentBShirtTexture, CurrentPartTexture,CurrentMaskTexture);
        }

        string textureName = "unknown";
        if (FullBodyTexController.textureGroups[i].RawTexture[index] != null)
        {
            textureName = FullBodyTexController.textureGroups[i].RawTexture[index].name;
        }

        AnalyticsManager.Instance.TrackContentView(
            "fullbody_" + i + "_" + index,
            "fullbody",
            textureName
        );

        mRobloxBodyHandler.SetFullBodyTexture(FullBodyTexController.textureGroups[i].RawTexture[index]);
        FullBodyTexController.CurrentRawTexture = FullBodyTexController.textureGroups[i].RawTexture[index];
        // screenManager.Active3d();
    }

    private int ExtractNumberFromFolderName(string folderName)
    {
        string numberString = new string(folderName.Where(char.IsDigit).ToArray());
        return int.TryParse(numberString, out int result) ? result : 0;
    }


    [ContextMenu("Load Masks")]
    public void LoadMasks()
    {
        // Base folder inside Resources
        string baseFolderPath = "masks";

        // Get all top-level categories (e.g., "1.shirt", "2.pant")
        string[] categories = GetSubFolders(baseFolderPath);

        foreach (string category in categories)
        {
            // Create a new CategoryTextures instance for this category
            string categoryName = Path.GetFileName(category);
            CategoryTextures categoryTextures = new CategoryTextures(categoryName);

            // Get all subfolders for the current category (e.g., "mask0", "mask1")
            string[] subFolders = GetSubFolders(category);
            subFolders = subFolders.OrderBy(f => ExtractNumberFromFolderName(f)).ToArray();
            foreach (string subFolder in subFolders)
        {
            // Load all textures from the current subfolder
            Sprite[] allTextures = Resources.LoadAll<Sprite>(subFolder);

            for (int i = 0; i < allTextures.Length; i++)
            {
                Sprite texture = allTextures[i];
                if (i % 2 != 0) // Even index
                {
                    if (texture.name.Contains("first", System.StringComparison.OrdinalIgnoreCase))
                    {
                        categoryTextures.FirstTextures.Add(texture);
                        categoryTextures.BackMask.Add(texture);
                    }
                    else if (texture.name.Contains("second", System.StringComparison.OrdinalIgnoreCase))
                    {
                        categoryTextures.SecondTextures.Add(texture);
                    }
                    // else if (texture.name.Contains("mask", System.StringComparison.OrdinalIgnoreCase))
                    // {
                    //     categoryTextures.Mask.Add(texture);
                    // }
                }
            }
            Texture [] allTexture = Resources.LoadAll<Texture >(subFolder);

            for (int i = 0; i < allTexture.Length; i++)
            {
                Texture  texture = allTexture[i];
                // if (i % 2 == 0) // Even index
                // {

                    if (texture.name.Contains("mask", System.StringComparison.OrdinalIgnoreCase))
                    {
                        categoryTextures.Mask.Add(texture);
                    }
              //  }
            }
            Debug.Log($"Processed subfolder: {subFolder}. Found {allTextures.Length} textures.");
        }

        // Add the category textures to the list
        allCategoryTextures.Add(categoryTextures);

        Debug.Log($"Category '{categoryName}' - Total 'first' textures: {categoryTextures.FirstTextures.Count}");
        Debug.Log($"Category '{categoryName}' - Total 'second' textures: {categoryTextures.SecondTextures.Count}");
    }
}



    private string[] GetSubFolders(string baseFolderPath)
    {
        // Find all subfolders in the Resources directory
        string resourcesPath = Path.Combine(Application.dataPath, "Resources", baseFolderPath);
        if (!Directory.Exists(resourcesPath))
        {
            Debug.LogError($"Path not found: {resourcesPath}. Please ensure the folder structure is correct.");
            return new string[0];
        }

        // Get relative paths of all subfolders
        string[] directories = Directory.GetDirectories(resourcesPath);
        return directories.Select(dir => Path.Combine(baseFolderPath, Path.GetFileName(dir))).ToArray();
    }
}
// A class to store textures for a specific category
[Serializable]
public class CategoryTextures
{
    public string CategoryName;
    public List<Sprite> FirstTextures = new List<Sprite>();
    public List<Sprite> SecondTextures = new List<Sprite>();
    public List<Sprite > BackMask = new List<Sprite >();
    public List<Texture > Mask = new List<Texture >();


    public CategoryTextures(string categoryName)
    {
        CategoryName = categoryName;
    }
}