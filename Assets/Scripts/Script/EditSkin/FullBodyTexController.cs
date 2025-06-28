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
    
    public Texture2D GetToTexture2D()
    {
        var sourceTexture = CurrentRawTexture as Texture2D;
        // Ensure the source texture is not null
        if (sourceTexture == null)
        {
            Debug.LogError("Source texture is null!");
            return null;
        }

        // Create a temporary RenderTexture with the same dimensions as the source texture
        RenderTexture renderTex = RenderTexture.GetTemporary(
            sourceTexture.width,
            sourceTexture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        // Copy the source texture to the temporary RenderTexture
        Graphics.Blit(sourceTexture, renderTex);

        // Store the currently active RenderTexture
        RenderTexture previous = RenderTexture.active;

        // Set the temporary RenderTexture as the active one
        RenderTexture.active = renderTex;

        // Create a new Texture2D to read the pixels into
        Texture2D readableText = new Texture2D(sourceTexture.width, sourceTexture.height);

        // Read the pixels from the active RenderTexture
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();

        // Restore the previously active RenderTexture
        RenderTexture.active = previous;

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(renderTex);

        return readableText;
    }
}





