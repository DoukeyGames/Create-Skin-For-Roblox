// using Unity.Jobs;
// using Unity.Burst;
// using UnityEngine;
// using UnityEngine.Rendering;
//
// [BurstCompile]
// public struct ProcessBodyPartJob : IJob
// {
//     public NativeArray<BodyPartData> bodyPartsData; // Data to pass to the job
//     public RenderTexture bakedTexture;  // Base texture to bake on
//     public Texture2D maskedTexture;    // Masked texture to apply
//
//     public void Execute()
//     {
//         for (int i = 0; i < bodyPartsData.Length; i++)
//         {
//             var bodyPart = bodyPartsData[i];
//             ProcessTexture(bodyPart, bakedTexture, maskedTexture);
//         }
//     }
//
//     private void ProcessTexture(BodyPartData bodyPart, RenderTexture bakedTexture, Texture2D maskedTexture)
//     {
//         // Add texture processing logic here, similar to the original `ProcessBodyPart` method.
//         // Use the data in bodyPart to apply transformations, stickers, etc., to the bakedTexture.
//     }
// }
//
// public struct BodyPartData
// {
//     public RenderTexture renderTexture;
//     public RenderTexture stickerTexture;
//     public RenderTexture backStickerTexture;
//     public Material material;
//     public Vector2 tiling;
//     public Vector2 offset;
//     public Vector2 textureTiling;
//     public float uvScale;
// }
//
//
// public class RenderTextureJob : MonoBehaviour
// {
//     public void LateBake()
// {
//     if (maskedTexture == null)
//     {
//         Debug.LogError("Masked texture is not assigned!");
//         return;
//     }
//
//     string fullPath = Path.Combine(saveFolder, fileName);
//     CreateSaveDirectory();
//
//     var bakedTexture = CreateBaseTexture(1024, 1024);
//
//     NativeArray<BodyPartData> bodyPartsData = new NativeArray<BodyPartData>(shirtBody.Count + pantBody.Count, Allocator.TempJob);
//
//     // Populate the data array for job execution
//     int index = 0;
//     if (MenuController.Instance.CurrentCatalog == 0 || MenuController.Instance.CurrentCatalog == 2)
//     {
//         foreach (var part in shirtBody)
//         {
//             bodyPartsData[index++] = new BodyPartData
//             {
//                 renderTexture = part.renderTexture,
//                 stickerTexture = part.stickerTexture,
//                 backStickerTexture = part.backStickerTexture,
//                 material = part.m_Material,
//                 tiling = part.tiling,
//                 offset = part.offset,
//                 textureTiling = part.texturetiling,
//                 uvScale = part.uvScale
//             };
//         }
//     }
//     else if (MenuController.Instance.CurrentCatalog == 1)
//     {
//         foreach (var part in pantBody)
//         {
//             bodyPartsData[index++] = new BodyPartData
//             {
//                 renderTexture = part.renderTexture,
//                 stickerTexture = part.stickerTexture,
//                 backStickerTexture = part.backStickerTexture,
//                 material = part.m_Material,
//                 tiling = part.tiling,
//                 offset = part.offset,
//                 textureTiling = part.texturetiling,
//                 uvScale = part.uvScale
//             };
//         }
//     }
//
//     // Schedule the job to run asynchronously
//     ProcessBodyPartJob processJob = new ProcessBodyPartJob
//     {
//         bodyPartsData = bodyPartsData,
//         bakedTexture = bakedTexture,
//         maskedTexture = maskedTexture
//     };
//
//     JobHandle jobHandle = processJob.Schedule();
//
//     // Complete the job and save the texture once it's finished
//     jobHandle.Complete();
//
//     SaveTexture(bakedTexture, fullPath);
//
//     // Dispose of the NativeArray after job completion
//     bodyPartsData.Dispose();
// }
//
// }


