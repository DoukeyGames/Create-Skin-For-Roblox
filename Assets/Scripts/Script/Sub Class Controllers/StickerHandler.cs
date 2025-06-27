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
     currentSticker.GetComponent<UIDragAndSnap>().originalParent = stickerParent;
 }

 public void ClearList()
 {
     for (int i = 0; i < allSticker.Count; i++)
     {
         allSticker[i].GetComponent<UIDragAndSnap>().DeleteSticker();
     }
     allSticker.Clear();
 }
}
