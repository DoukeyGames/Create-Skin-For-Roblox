using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.UI;
using AppsFlyerSDK;

public class RenderTextureScaler : MonoBehaviour
{
    public GameObject Loading;
    public GameObject Loading3D;
    public RawImage outputTexture;
    public RawImage outputTexture3D;
    public string fileName = "TextureBacker.png";
    public string saveFolder = "Assets/";
    public List<BodyPart> shirtBody;
    public List<BodyPart> pantBody;
    public int checkerboardSize = 32;
    public Color uvLineColor = Color.white;
    public Color greenMaskColor = Color.white;
    public bool showCheckerboard;
    public bool showUVMapping;
    public bool showTextureOnUVMap = true;
    public bool applyStickerToUV1 = true;
    public bool applyBackStickerToUV3 = true;

    public Texture2D maskedTexture;
    public int Width, Height;
    public static RenderTextureScaler instance;
    private bool bakeTriggered = false;

    public void Awake()
    {
        instance = this;
    }

    public void BakeTexture()
    {
        Loading.SetActive(true);
        Loading3D.SetActive(true);

#if UNITY_IOS && !UNITY_EDITOR
        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventJoinGroup,
            Firebase.Analytics.FirebaseAnalytics.ParameterGroupID, "skin_download");

        LogSkinDownload("skin_download");
#endif

        bakeTriggered = true;

        Invoke(nameof(LateBake), 0.1f);
    }

    public void BakeTextureModel()
    {
        Loading.SetActive(false);
        Loading3D.SetActive(false);

#if UNITY_IOS && !UNITY_EDITOR
        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventJoinGroup,
            Firebase.Analytics.FirebaseAnalytics.ParameterGroupID, "skin_download");

        LogSkinDownload("skin_download");
#endif

        bakeTriggered = true;

        Invoke(nameof(LateBake), 0.1f);
    }

    public void LogSkinDownload(string skinName)
    {
        try
        {
            // Determine skin type based on current catalog
            string skinType = "unknown";
            string contentId = "unknown";
            if (MainMenu.Instance != null)
            {
                switch (MainMenu.Instance.CurrentCatalog)
                {
                    case 0:
                        skinType = "shirt";
                        break;
                    case 1:
                        skinType = "pants";
                        break;
                    case 2:
                        skinType = "tshirt";
                        break;
                    case 3:
                        skinType = "fullbody";
                        break;
                }

                // Create content ID with pattern info
                contentId = skinType + "_" + MainMenu.Instance.CurrentPattern;
            }

            // Track with AnalyticsManager for comprehensive logging
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.TrackSkinDownload(skinName, skinType);
            }

            // Use AnalyticsManager for consistent tracking
            AnalyticsManager.Instance.TrackSkinDownload(skinName, skinType);

            // Also keep the direct AppsFlyer call for backward compatibility
            Dictionary<string, string> eventValues = new Dictionary<string, string>();
            eventValues.Add("skin_name", skinName);
            eventValues.Add("skin_type", skinType);

            // Log for debugging
            Debug.Log($"Sending AppsFlyer event: skin_download with value: {skinName}, type: {skinType}");

            AppsFlyer.sendEvent("skin_download", eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending AppsFlyer event: {ex.Message}");
            AnalyticsManager.Instance.TrackError("skin_download", ex.Message);
        }
    }

    public void LateBake()
    {
        if (maskedTexture == null)
        {
            Debug.LogError("Masked texture is not assigned!");
            return;
        }

        string fullPath = Path.Combine(saveFolder, fileName);
        CreateSaveDirectory();

        var bakedTexture = CreateBaseTexture(Width, Height);

        if (MainMenu.Instance.CurrentCatalog == 0 || MainMenu.Instance.CurrentCatalog == 2)
        {
            foreach (var part in shirtBody)
            {
                ProcessBodyPart(part, bakedTexture);
            }
        }
        else if (MainMenu.Instance.CurrentCatalog == 1)
        {
            foreach (var part in pantBody)
            {
                ProcessBodyPart(part, bakedTexture);
            }
        }

        // Check if bakeTriggered is true, then call SaveTexture
        if (bakeTriggered)
        {
            SaveTexture(bakedTexture, fullPath);
            bakeTriggered = false; // Reset the flag after saving
        }
    }

    private void SaveTexture(Texture2D bakedTexture, string fullPath)
    {
        bakedTexture.Apply();
        outputTexture.texture = bakedTexture;
        outputTexture3D.texture = bakedTexture;

        // Only assign the texture based on catalog choice
        if (MainMenu.Instance.CurrentCatalog == 0 || MainMenu.Instance.CurrentCatalog == 2)
        {
            foreach (var part in shirtBody)
            {
                part.m_Material.mainTexture = bakedTexture;
            }
        }
        else if (MainMenu.Instance.CurrentCatalog == 1)
        {
            foreach (var part in pantBody)
            {
                part.m_Material.mainTexture = bakedTexture;
            }
        }

        // Save the texture to gallery
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(bakedTexture, "Roblox", "RobloxSkin.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));
        Debug.Log("Permission result: " + permission);

#if UNITY_EDITOR
        File.WriteAllBytes(fullPath, bakedTexture.EncodeToPNG());
        Debug.Log($"Baked texture saved to {fullPath}");
#endif
    }

    private void CreateSaveDirectory()
    {
#if UNITY_EDITOR
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
#endif
    }

    private Texture2D CreateBaseTexture(int width, int height, int checkerboardSize = 10, bool showCheckerboard = false)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Bilinear;

        Color baseColor = Color.white;
        Color[] pixels = new Color[width * height];

        if (showCheckerboard)
        {
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int index = x + y * width;
                    bool isCheckerSquare = (x / checkerboardSize + y / checkerboardSize) % 2 == 0;
                    pixels[index] = isCheckerSquare ? Color.white : baseColor;
                }
            });
        }
        else
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = baseColor;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private void ProcessBodyPart(BodyPart part, Texture2D bakedTexture)
    {
        if (part.bodyPart == null)
        {
            Debug.LogWarning("Body part is null.");
            return;
        }

        Mesh mesh =  GetMeshFromBodyPart(part.bodyPart);
        if (mesh == null || mesh.uv == null || mesh.uv.Length == 0)
        {
            Debug.LogWarning($"Invalid or missing mesh/UVs for {part.bodyPart.name}");
            return;
        }

        if (showUVMapping) DrawUVLines(mesh, bakedTexture, mesh.uv);

        if (showTextureOnUVMap)
        {
            ApplyTexture(mesh, part.renderTexture, bakedTexture, part);

           // if(part.additionalrenderTexture)
           // ApplyTexture(mesh, part.additionalrenderTexture, bakedTexture, part);

        }

        if (applyStickerToUV1 && part.stickerTexture != null)
        {
            ApplyTexture(mesh, part.stickerTexture, bakedTexture, part, true);
        }
        if (applyBackStickerToUV3 && part.backStickerTexture != null)
        {
            ApplyTexture(mesh, part.backStickerTexture, bakedTexture, part, true,true);
        }
    }
    private void ApplyTexture(Mesh mesh, RenderTexture sourceTexture, Texture2D targetTexture, BodyPart part, bool isSticker = false, bool isBackSticker = false)
    {
       // var uvData = GetUVData(mesh); // Retrieve all UV sets
        var texture2D = ConvertRenderTextureToTexture2D(sourceTexture);
        if (texture2D == null) return;

        // Select the appropriate UV set based on isBackSticker and isSticker
        if (isBackSticker)
        {
            var uvs = mesh.uv3; // Use uv3 for back sticker
            ApplyTextureToTriangles(mesh, texture2D, targetTexture, uvs, part, isSticker);
        }
        else
        {
            var uvs = isSticker ?  mesh.uv2 :  mesh.uv; // Use uv2 for sticker, otherwise uv

            ApplyTextureToTriangles(mesh, texture2D, targetTexture, uvs, part, isSticker);
        }
    }

    private void ApplyTextureToTriangles(Mesh mesh, Texture2D sourceTexture, Texture2D targetTexture, Vector2[] uvs, BodyPart part, bool isSticker)
    {
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector2[] triangleUVs = TransformUVs(new Vector2[]
            {
                uvs[mesh.triangles[i]],
                uvs[mesh.triangles[i + 1]],
                uvs[mesh.triangles[i + 2]]
            }, part, isSticker);

            ApplyMaskedTextureToTriangle(triangleUVs[0], triangleUVs[1], triangleUVs[2], sourceTexture, targetTexture);
        }
    }

void ApplyMaskedTextureToTriangle(Vector2 uvA, Vector2 uvB, Vector2 uvC, Texture2D sourceTexture, Texture2D targetTexture)
{
    // Calculate bounds for the texture region once
    int minX = Mathf.FloorToInt(Mathf.Min(uvA.x, Mathf.Min(uvB.x, uvC.x)) * targetTexture.width);
    int minY = Mathf.FloorToInt(Mathf.Min(uvA.y, Mathf.Min(uvB.y, uvC.y)) * targetTexture.height);
    int maxX = Mathf.FloorToInt(Mathf.Max(uvA.x, Mathf.Max(uvB.x, uvC.x)) * targetTexture.width);
    int maxY = Mathf.FloorToInt(Mathf.Max(uvA.y, Mathf.Max(uvB.y, uvC.y)) * targetTexture.height);

    // Clamp bounds to texture size
    minX = Mathf.Clamp(minX, 0, targetTexture.width - 1);
    minY = Mathf.Clamp(minY, 0, targetTexture.height - 1);
    maxX = Mathf.Clamp(maxX, 0, targetTexture.width - 1);
    maxY = Mathf.Clamp(maxY, 0, targetTexture.height - 1);

    // Precompute source texture UV-to-pixel mapping (use only once)
    float srcWidth = sourceTexture.width;
    float srcHeight = sourceTexture.height;

    // Cache the masked texture pixel data
    Color[] maskColors = maskedTexture.GetPixels();

    // Loop through pixels in the bounding box
    for (int x = minX; x <= maxX; x++)
    {
        for (int y = minY; y <= maxY; y++)
        {
            // Calculate the UV of the current pixel
            Vector2 uv = new Vector2(x / (float)targetTexture.width, y / (float)targetTexture.height);

            // Only check if the point is inside the triangle if it's within the bounds
            if (IsPointInTriangle(uv, uvA, uvB, uvC))
            {
                // Ensure UVs are clamped to [0, 1] range before using them for texture sampling
                uv.x = Mathf.Clamp01(uv.x);
                uv.y = Mathf.Clamp01(uv.y);

                // Sample mask color (ensure indices are within bounds)
                int maskIndex = Mathf.FloorToInt(uv.y * maskedTexture.height) * maskedTexture.width + Mathf.FloorToInt(uv.x * maskedTexture.width);
                if (maskIndex >= 0 && maskIndex < maskColors.Length)
                {
                    Color maskColor = maskColors[maskIndex];

                    if (maskColor == Color.black) // Only apply texture where maskedTexture is black
                    {
                        // Calculate source texture UV mapping
                        float srcX = Mathf.InverseLerp(minX, maxX, x) * srcWidth;
                        float srcY = Mathf.InverseLerp(minY, maxY, y) * srcHeight;

                        // Sample the source texture
                        Color sourceColor = sourceTexture.GetPixelBilinear(srcX / srcWidth, srcY / srcHeight);

                        // Blend source color onto base texture
                        Color baseColor = targetTexture.GetPixel(x, y);
                        float alpha = sourceColor.a;
                        Color blendedColor = Color.Lerp(baseColor, sourceColor, alpha);

                        targetTexture.SetPixel(x, y, blendedColor);
                    }
                    // else if (maskColor == Color.green) // Green region: Replace with uvLineColor
                    // {
                    //     targetTexture.SetPixel(x, y, greenMaskColor);
                    // }
                }
            }
        }
    }
}

    bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 v0 = c - a;
        Vector2 v1 = b - a;
        Vector2 v2 = p - a;

        float dot00 = Vector2.Dot(v0, v0);
        float dot01 = Vector2.Dot(v0, v1);
        float dot02 = Vector2.Dot(v0, v2);
        float dot11 = Vector2.Dot(v1, v1);
        float dot12 = Vector2.Dot(v1, v2);

        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0) && (v >= 0) && (u + v <= 1);
    }

    private Vector2[] TransformUVs(Vector2[] uvs, BodyPart part, bool isSticker)
    {
        var tiling = isSticker ? part.tiling : part.texturetiling;
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(
                uvs[i].x * tiling.x + part.offset.x,
                uvs[i].y * tiling.y + part.offset.y) * part.uvScale;
        }
        return uvs;
    }

    private void DrawUVLines(Mesh mesh, Texture2D texture, Vector2[] uvs)
    {
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            DrawLine(uvs[mesh.triangles[i]], uvs[mesh.triangles[i + 1]], texture);
            DrawLine(uvs[mesh.triangles[i + 1]], uvs[mesh.triangles[i + 2]], texture);
            DrawLine(uvs[mesh.triangles[i + 2]], uvs[mesh.triangles[i]], texture);
        }
    }

    void DrawLine(Vector2 p1, Vector2 p2, Texture2D texture)
    {
        int x1 = Mathf.FloorToInt(p1.x * texture.width);
        int y1 = Mathf.FloorToInt(p1.y * texture.height);
        int x2 = Mathf.FloorToInt(p2.x * texture.width);
        int y2 = Mathf.FloorToInt(p2.y * texture.height);

        int dx = Mathf.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
        int dy = Mathf.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Draw the current pixel
            texture.SetPixel(x1, y1, uvLineColor);

            if (x1 == x2 && y1 == y2) break;

            int e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x1 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y1 += sy;
            }
        }
    }

    private Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        if (renderTexture == null) return null;
        RenderTexture.active = renderTexture;

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        return texture;
    }

    private Mesh GetMeshFromBodyPart(GameObject bodyPart)
    {
        var meshFilter = bodyPart.GetComponent<MeshFilter>();
        if (meshFilter?.sharedMesh != null) return meshFilter.sharedMesh;

        var skinnedMeshRenderer = bodyPart.GetComponent<SkinnedMeshRenderer>();
        return skinnedMeshRenderer?.sharedMesh;
    }

    [System.Serializable]
    public struct BodyPart
    {
        public GameObject bodyPart;
        public RenderTexture renderTexture;
        public RenderTexture stickerTexture;
        public RenderTexture backStickerTexture;
        public Material m_Material;
       public Vector2 tiling ;
       public Vector2 offset ;
       public Vector2 texturetiling ;
       public float uvScale ;
    }

}
