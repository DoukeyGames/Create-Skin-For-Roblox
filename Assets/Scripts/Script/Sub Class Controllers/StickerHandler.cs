using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerHandler : MonoBehaviour
{
 public GameObject stickerPrefab;
 public Transform stickerParent;
 public GameObject currentSticker;
 public List<GameObject> allSticker;
 public static StickerHandler Instance;

 public void Awake()
 {
     Instance = this;
 }

 public void CreateSticker()
 {
     if (currentSticker)
     {
         Destroy(currentSticker);
     }

     currentSticker = Instantiate(stickerPrefab, stickerParent); allSticker.Add(currentSticker);
     currentSticker.GetComponent<StickerDragger>().originalParent = stickerParent;
 }

 public void ClearList()
 {
     foreach (var t in allSticker)
     { 
         if(t == null) continue;
         var uiDragAndSnap = t.GetComponent<StickerDragger>(); 
         if (uiDragAndSnap != null) 
         {
            uiDragAndSnap.DeleteSticker(); 
         }
     }

     allSticker.Clear();
 }
}
