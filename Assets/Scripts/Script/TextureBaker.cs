using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TextureBaker : MonoBehaviour
{
    public string fileName = "BakedTexture.png";   // Output file name
    public string saveFolder = "Assets/";          // Save folder for the baked texture
    public List<BodyPartTexture> bodyParts;        // Body parts with textures, materials, and stickers
    public int checkerboardSize = 32;              // Size of checkerboard squares
    public Color uvLineColor = Color.white;        // Color for UV mapping lines
    public bool showCheckerboard = true;           // Toggle checkerboard background
    public bool showUVMapping = true;              // Toggle UV mapping lines
    public bool showTextureOnUVMap = true;         // Toggle applying textures to UV map
    public bool applyStickerToUV2 = true;          // Toggle sticker application to UV2
    public int selectedUVSet = 0;                  // Selected UV set (0: UV1, 1: UV2, etc.)

    [ContextMenu("Bake Texture")]
    private void BakeTexture()
    {
        SaveUVSnapshot(fileName);
    }

    void SaveUVSnapshot(string fileName)
    {
        if (bodyParts == null || bodyParts.Count == 0)
        {
            Debug.LogError("No body parts or textures assigned!");
            return;
        }

        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string fullPath = Path.Combine(saveFolder, fileName);

        int textureWidth = 2048;
        int textureHeight = 2048;
        Texture2D bakedTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);

        // Create a checkerboard background
        if (showCheckerboard)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                for (int y = 0; y < textureHeight; y++)
                {
                    bool isLightSquare = ((x / checkerboardSize + y / checkerboardSize) % 2 == 0);
                    bakedTexture.SetPixel(x, y, isLightSquare ? Color.white : new Color(0.2f, 0.2f, 0.2f));
                }
            }
        }
        else
        {
            Color fillColor = Color.black;
            for (int x = 0; x < textureWidth; x++)
            {
                for (int y = 0; y < textureHeight; y++)
                {
                    bakedTexture.SetPixel(x, y, fillColor);
                }
            }
        }

        foreach (var part in bodyParts)
        {
            if (part.bodyPart == null)
            {
                Debug.LogWarning("Body part is null.");
                continue;
            }

            Mesh mesh = GetMeshFromBodyPart(part.bodyPart);
            if (mesh == null)
            {
                Debug.LogWarning($"No valid mesh found on {part.bodyPart.name}.");
                continue;
            }

            Vector2[] uvs = GetUVsBySetIndex(mesh, selectedUVSet);
            if (uvs == null || uvs.Length == 0)
            {
                Debug.LogError($"UV set {selectedUVSet + 1} is missing for {part.bodyPart.name}.");
                continue;
            }

            Debug.Log($"UV set {selectedUVSet + 1} loaded with {uvs.Length} coordinates for {part.bodyPart.name}.");

            if (showUVMapping)
            {
                DrawUVLines(mesh, bakedTexture, uvs);
            }

            if (showTextureOnUVMap)
            {
                foreach (var renderTexture in part.renderTextures)
                {
                    Texture2D sourceTexture = ConvertRenderTextureToTexture2D(renderTexture);
                    if (sourceTexture != null)
                    {
                        ApplyTextureToUVs(mesh, sourceTexture, bakedTexture, uvs);
                    }
                }
            }
            if (applyStickerToUV2 && part.stickerTexture != null)
            {
                Vector2[] stickerUVs = mesh.uv2;

                if (stickerUVs == null || stickerUVs.Length == 0)
                {
                    Debug.LogError($"UV2 set is missing for {part.bodyPart.name}. Cannot apply sticker.");
                    continue;
                }

                Texture2D sticker = ConvertRenderTextureToTexture2D(part.stickerTexture);
                if (sticker != null)
                {
                    ApplyStickerToUV2(mesh, sticker, bakedTexture);
                    Debug.Log($"Sticker applied strictly to UV2 for {part.bodyPart.name}.");
                }
            }

            // Apply the sticker to UV2
            // if (applyStickerToUV2 && part.stickerTexture != null)
            // {
            //     Vector2[] stickerUVs = mesh.uv2;
            //     if (stickerUVs == null || stickerUVs.Length == 0)
            //     {
            //         Debug.LogError($"UV2 set is missing for {part.bodyPart.name}. Cannot apply sticker.");
            //         continue;
            //     }
            //
            //     Texture2D sticker = ConvertRenderTextureToTexture2D(part.stickerTexture);
            //     if (sticker != null)
            //     {
            //         ApplyTextureToUVs(mesh, sticker, bakedTexture, stickerUVs);
            //         Debug.Log($"Sticker applied to {part.bodyPart.name} using UV2.");
            //     }
            // }
        }

        bakedTexture.Apply();
        File.WriteAllBytes(fullPath, bakedTexture.EncodeToPNG());
        DestroyImmediate(bakedTexture);

        Debug.Log($"Baked texture saved to {fullPath}");
    }

    public Rect tileArea;
void ApplyStickerToUV2(Mesh mesh, Texture2D stickerTexture, Texture2D targetTexture)
{
    // Ensure we are working with UV2 coordinates
    Vector2[] uv2 = mesh.uv2;

    if (uv2 == null || uv2.Length == 0)
    {
        Debug.LogError($"UV2 coordinates are missing for {mesh.name}. Cannot apply sticker.");
        return;
    }

    int[] triangles = mesh.triangles;

    // Assuming tileArea is the region of your atlas where the sticker is located
 //   Rect tileArea = new Rect(0.25f, 0.25f, 0.5f, 0.5f);  // Example: middle region of the texture atlas

    for (int i = 0; i < triangles.Length; i += 3)
    {
        Vector2 uvA = uv2[triangles[i]];
        Vector2 uvB = uv2[triangles[i + 1]];
        Vector2 uvC = uv2[triangles[i + 2]];

        // Remap the UV coordinates for the sticker, taking into account the scale of the tileArea
        Vector2 newUVA = RemapUV(uvA, tileArea);
        Vector2 newUVB = RemapUV(uvB, tileArea);
        Vector2 newUVC = RemapUV(uvC, tileArea);

        // Apply the sticker texture to the triangle
        ApplyStickerToTriangle(newUVA, newUVB, newUVC, stickerTexture, targetTexture, tileArea);
    }
}

void ApplyStickerToTriangle(Vector2 uvA, Vector2 uvB, Vector2 uvC, Texture2D stickerTexture, Texture2D targetTexture, Rect tileArea)
{
    // Calculate the bounding box of the triangle in UV space
    int minX = Mathf.FloorToInt(Mathf.Min(uvA.x, Mathf.Min(uvB.x, uvC.x)) * targetTexture.width);
    int minY = Mathf.FloorToInt(Mathf.Min(uvA.y, Mathf.Min(uvB.y, uvC.y)) * targetTexture.height);
    int maxX = Mathf.FloorToInt(Mathf.Max(uvA.x, Mathf.Max(uvB.x, uvC.x)) * targetTexture.width);
    int maxY = Mathf.FloorToInt(Mathf.Max(uvA.y, Mathf.Max(uvB.y, uvC.y)) * targetTexture.height);

    // Clamp the coordinates within the texture boundaries
    minX = Mathf.Clamp(minX, 0, targetTexture.width - 1);
    minY = Mathf.Clamp(minY, 0, targetTexture.height - 1);
    maxX = Mathf.Clamp(maxX, 0, targetTexture.width - 1);
    maxY = Mathf.Clamp(maxY, 0, targetTexture.height - 1);

    for (int x = minX; x <= maxX; x++)
    {
        for (int y = minY; y <= maxY; y++)
        {
            // Map the pixel to UV space
            Vector2 uv = new Vector2(x / (float)targetTexture.width, y / (float)targetTexture.height);

            // Check if the pixel is inside the triangle using barycentric coordinates
            if (IsPointInTriangle(uv, uvA, uvB, uvC))
            {
                // Map UV to sticker texture space with tileArea scaling
                float srcX = Mathf.Lerp(tileArea.xMin, tileArea.xMax, uv.x);
                float srcY = Mathf.Lerp(tileArea.yMin, tileArea.yMax, uv.y);

                // Ensure the UV coordinates are within bounds of the sticker texture
                srcX = Mathf.Clamp(srcX, 0, stickerTexture.width - 1);
                srcY = Mathf.Clamp(srcY, 0, stickerTexture.height - 1);

                Color sourceColor = stickerTexture.GetPixel((int)srcX, (int)srcY);
                Color targetColor = targetTexture.GetPixel(x, y);

                // Check if the sticker has any alpha transparency
                if (sourceColor.a > 0)
                {
                    // Apply the sticker (if it has alpha transparency)
                    Color blendedColor = Color.Lerp(targetColor, sourceColor, sourceColor.a);
                    targetTexture.SetPixel(x, y, blendedColor);
                }
            }
        }
    }
}

Vector2 RemapUV(Vector2 uv, Rect tileArea)
{
    // Calculate the scaled UV coordinates based on the tileArea.
    float scaledX = Mathf.Lerp(tileArea.xMin, tileArea.xMax, uv.x);
    float scaledY = Mathf.Lerp(tileArea.yMin, tileArea.yMax, uv.y);
    
    // Scale the UV coordinates according to the size of the tileArea
    return new Vector2(scaledX, scaledY);
}


    Mesh GetMeshFromBodyPart(GameObject bodyPart)
    {
        // Attempt to retrieve the mesh from either MeshFilter or SkinnedMeshRenderer
        MeshFilter meshFilter = bodyPart.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            return meshFilter.sharedMesh;
        }

        SkinnedMeshRenderer skinnedMeshRenderer = bodyPart.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
        {
            return skinnedMeshRenderer.sharedMesh;
        }

        return null; // No valid mesh found
    }

    Vector2[] GetUVsBySetIndex(Mesh mesh, int uvSet)
    {
        switch (uvSet)
        {
            case 0: return mesh.uv;
            case 1: return mesh.uv2;
            case 2: return mesh.uv3;
            case 3: return mesh.uv4;
            default:
                Debug.LogError($"Invalid UV set index: {uvSet}");
                return null;
        }
    }

    Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        if (renderTexture == null) return null;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = currentRT;
        return texture;
    }

    // void ApplyTextureToUVs(Mesh mesh, Texture2D sourceTexture, Texture2D targetTexture, Vector2[] uvs)
    // {
    //     int[] triangles = mesh.triangles;
    //
    //     for (int i = 0; i < triangles.Length; i += 3)
    //     {
    //         Vector2 uvA = uvs[triangles[i]];
    //         Vector2 uvB = uvs[triangles[i + 1]];
    //         Vector2 uvC = uvs[triangles[i + 2]];
    //
    //         ApplyTextureToTriangle(uvA, uvB, uvC, sourceTexture, targetTexture);
    //     }
    // }

    void ApplyTextureToUVs(Mesh mesh, Texture2D sourceTexture, Texture2D targetTexture, Vector2[] uvs)
{
    int[] triangles = mesh.triangles;

    for (int i = 0; i < triangles.Length; i += 3)
    {
        Vector2 uvA = uvs[triangles[i]];
        Vector2 uvB = uvs[triangles[i + 1]];
        Vector2 uvC = uvs[triangles[i + 2]];

        ApplyTextureToTriangle(uvA, uvB, uvC, sourceTexture, targetTexture);
    }
}

void ApplyTextureToTriangle(Vector2 uvA, Vector2 uvB, Vector2 uvC, Texture2D sourceTexture, Texture2D targetTexture)
{
    // Calculate the bounding box of the triangle in UV space
    int minX = Mathf.FloorToInt(Mathf.Min(uvA.x, Mathf.Min(uvB.x, uvC.x)) * targetTexture.width);
    int minY = Mathf.FloorToInt(Mathf.Min(uvA.y, Mathf.Min(uvB.y, uvC.y)) * targetTexture.height);
    int maxX = Mathf.FloorToInt(Mathf.Max(uvA.x, Mathf.Max(uvB.x, uvC.x)) * targetTexture.width);
    int maxY = Mathf.FloorToInt(Mathf.Max(uvA.y, Mathf.Max(uvB.y, uvC.y)) * targetTexture.height);

    // Clamp the coordinates within the texture boundaries
    minX = Mathf.Clamp(minX, 0, targetTexture.width - 1);
    minY = Mathf.Clamp(minY, 0, targetTexture.height - 1);
    maxX = Mathf.Clamp(maxX, 0, targetTexture.width - 1);
    maxY = Mathf.Clamp(maxY, 0, targetTexture.height - 1);

    for (int x = minX; x <= maxX; x++)
    {
        for (int y = minY; y <= maxY; y++)
        {
            // Map the pixel to UV space
            Vector2 uv = new Vector2(x / (float)targetTexture.width, y / (float)targetTexture.height);

            // Check if the pixel is inside the triangle using barycentric coordinates
            if (IsPointInTriangle(uv, uvA, uvB, uvC))
            {
                // Map UV to source texture space
                float srcX = uv.x * sourceTexture.width;
                float srcY = uv.y * sourceTexture.height;

                Color sourceColor = sourceTexture.GetPixelBilinear(srcX / sourceTexture.width, srcY / sourceTexture.height);
                Color baseColor = targetTexture.GetPixel(x, y);

                // Apply alpha blending
                float alpha = sourceColor.a;
                Color blendedColor = Color.Lerp(baseColor, sourceColor, alpha);

                targetTexture.SetPixel(x, y, blendedColor);
            }
        }
    }
}

bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
{
    // Compute vectors
    Vector2 v0 = c - a;
    Vector2 v1 = b - a;
    Vector2 v2 = p - a;

    // Compute dot products
    float dot00 = Vector2.Dot(v0, v0);
    float dot01 = Vector2.Dot(v0, v1);
    float dot02 = Vector2.Dot(v0, v2);
    float dot11 = Vector2.Dot(v1, v1);
    float dot12 = Vector2.Dot(v1, v2);

    // Compute barycentric coordinates
    float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
    float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
    float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

    // Check if point is in triangle
    return (u >= 0) && (v >= 0) && (u + v < 1);
}


    void DrawUVLines(Mesh mesh, Texture2D texture, Vector2[] uvs)
    {
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            DrawLine(uvs[triangles[i]], uvs[triangles[i + 1]], texture);
            DrawLine(uvs[triangles[i + 1]], uvs[triangles[i + 2]], texture);
            DrawLine(uvs[triangles[i + 2]], uvs[triangles[i]], texture);
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
        int err = (dx > dy ? dx : -dy) / 2, e2;

        while (true)
        {
            texture.SetPixel(x1, y1, uvLineColor);
            if (x1 == x2 && y1 == y2) break;
            e2 = err;
            if (e2 > -dx) { err -= dy; x1 += sx; }
            if (e2 < dy) { err += dx; y1 += sy; }
        }
    }

    [System.Serializable]
    public class BodyPartTexture
    {
        public GameObject bodyPart;                 // Body part object
        public List<RenderTexture> renderTextures;  // Render textures for materials
        public RenderTexture stickerTexture;        // Sticker RenderTexture
    }
}
