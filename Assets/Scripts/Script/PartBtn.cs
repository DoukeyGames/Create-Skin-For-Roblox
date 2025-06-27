using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartBtn : MonoBehaviour
{
    public BodyController bodyController;
     public RawImage m_partImg1;
     public RawImage[] m_subpartImg;
     public Material m_partMat;
     public Material m_partBoyMat,m_partGirlMat;
     public Material[] m_subpartMat;
     public Animator m_animator;
     public Animator[] m_subanimator;
     public bool m_subPart;
     public void Awake()
     {
         bodyController = BodyController.Instance;
     }

     public void ApplyTexture()
     {
        if(UIDragAndSnap.stickerApplyig)
            return;
        
         m_animator.gameObject.SetActive(true);
         m_animator.Play("open");
         m_subPart =true; // (MenuController.Instance.CurrentCatalog==0) ? false:
         if (m_subPart)
         {
             if (m_subanimator.Length > 0)
             {
                 for (int i = 0; i < m_subanimator.Length; i++)
                 {
                     m_subanimator[i].gameObject.SetActive(true);
                     m_subanimator[i].Play("open");
                 }

             }
         }
         bodyController = BodyController.Instance;

         if (bodyController.CurrentTexture)
         {
             
             if (m_partImg1) m_partImg1.texture = bodyController.CurrentRawTexture;
             if (m_partImg1) m_partImg1.color = bodyController.CurrentColor;
             
             // if (m_partMat)  m_partMat.SetTexture("_BaseTex",  bodyController.CurrentRawTexture);
             // if (m_partBoyMat)  m_partBoyMat.SetTexture("_BaseTex",  bodyController.CurrentRawTexture);
             // if (m_partGirlMat)  m_partGirlMat.SetTexture("_BaseTex",  bodyController.CurrentRawTexture);
             //
             // if (m_partMat)  m_partMat.SetColor("_BaseColor",  bodyController.CurrentColor);
             // if (m_partBoyMat)  m_partBoyMat.SetColor("_BaseColor",  bodyController.CurrentColor);
             // if (m_partGirlMat)  m_partGirlMat.SetColor("_BaseColor",  bodyController.CurrentColor);
             
             if (m_subPart)
             {
                 if (m_subpartImg.Length > 0)
                 {
                     for (int i = 0; i < m_subpartImg.Length; i++)
                     {
                         m_subpartImg[i].texture = bodyController.CurrentRawTexture;
                         m_subpartImg[i].color = bodyController.CurrentColor;
                     }

                 }
                 // if (m_subpartMat.Length > 0)
                 // {
                 //     for (int i = 0; i < m_subpartMat.Length; i++)
                 //     {
                 //         m_subpartMat[i].SetTexture("_BaseTex",  bodyController.CurrentRawTexture);
                 //         m_subpartMat[i].SetColor("_BaseColor",  bodyController.CurrentColor);
                 //     }
                 //
                 // }
             }
         }


     }
     
     


    
}
