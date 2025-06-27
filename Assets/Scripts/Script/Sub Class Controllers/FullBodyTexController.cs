using System.Collections.Generic;
using UnityEngine;

public class FullBodyTexController : Singleton<FullBodyTexController>
{
    public List<TextureGroup> textureGroups = new List<TextureGroup>();
    
    public Sprite CurrentTexture;
    public Texture CurrentRawTexture;
    public int CurrentCatalog;
    public int CurrentPattern;

    
    public void ApplyTexture(int index)
    {
        CurrentPattern = index;
        CurrentTexture = textureGroups[CurrentCatalog].Texture[CurrentPattern];
        CurrentRawTexture = textureGroups[CurrentCatalog].RawTexture[CurrentPattern];
        RobloxBodyHandler.Instance.CurrentTexture = CurrentTexture;
        RobloxBodyHandler.Instance.CurrentRawTexture = CurrentRawTexture;
        RobloxBodyHandler.Instance.CurrentColor = Color.white;
    }
}





