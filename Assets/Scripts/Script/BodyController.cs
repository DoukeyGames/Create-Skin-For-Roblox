using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BodyController : MonoBehaviour
{
    
    public GameObject CharFront, CharBack;
    public GameObject CharRotateAnim;
    public float m_RotateWait;
    
    
    public Image m_bodyMask;
    public Image m_rightHandMask;
    public Image m_leftHandMask;
    public Image m_rightLegMask;
    public Image m_leftLegMask;
    
    public Image m_backbodyMask;
    public Image m_backrightHandMask;
    public Image m_backleftHandMask;
    public Image m_backrightLegMask;
    public Image m_backleftLegMask;
    
    public GameObject m_CentreBodyBtn;
    public GameObject m_TopBodyBtn,m_BottomBodyBtn;
    public GameObject m_BCentreBodyBtn;
    public GameObject m_BTopBodyBtn,m_BBottomBodyBtn;
    public GameObject m_LHandBtn,m_RHandBtn;
    public GameObject m_BLHandBtn,m_BRHandBtn;
    public GameObject[] m_LLegBtn,m_RLegBtn;
    public GameObject[] m_LegRawImage,m_HandRawImage;
    
    public GameObject m_CompleteBody,m_AdditionalTopBody,m_AdditionalBottomBody;
    public GameObject m_BCompleteBody,m_BAdditionalTopBody,m_BAdditionalBottomBody;

    public GameObject m_BodyArea,m_AdditionalBodyArea;
    public GameObject m_LHandArea,m_RHandArea;
    public GameObject[] m_LLegArea,m_RLegArea;

    public Texture m_BaseTex,m_OutlineTex;
    public Material[] m_MatBody, m_MatHand, m_MatLeg;
    public GameObject[] m_WholeBody;
    public GameObject[] m_WholeBackBody;


    public GameObject m_subMenu, m_fullMenu;
    
    public Sprite CurrentTexture;
    public Texture CurrentRawTexture;
    public Color CurrentColor;

    public static BodyController Instance;
    public void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        ClearTexture();
    }
    public void ChangeRotation()
    {
        CharRotateAnim.SetActive(true);
        StartCoroutine(ChangeCharRotation());
    }

    public IEnumerator ChangeCharRotation()
    {
        GameObject g;
        if (CharFront.activeInHierarchy)
         {
            g = CharBack;
        }
        else
        {
            g = CharFront; 
        }
        CharFront.SetActive(false);
        CharBack.SetActive(false);
        yield return new WaitForSeconds(m_RotateWait);
        g.SetActive(true);
        CharRotateAnim.SetActive(false);
    }

    public void setMenu(GameObject g)
    {
        m_subMenu.SetActive(false);
        m_fullMenu.SetActive(false);
        g.SetActive(true);
        CharFront.SetActive(true);
        CharBack.SetActive(false);
        CharRotateAnim.SetActive(false);
    }

    public void UnActiveBody()
    {
        for (int i = 0; i < m_WholeBody.Length; i++)
        {
            m_WholeBody[i].SetActive(false);
        }
        for (int i = 0; i < m_WholeBody.Length; i++)
        {
            m_WholeBackBody[i].SetActive(false);
        }
    }

    public void SetShirtMask(Sprite shirt,Sprite bshirt, Sprite part, Texture mask)
    {
      UnActiveBody();
      m_WholeBody[0].SetActive(true);
      m_WholeBody[1].SetActive(true);
      m_WholeBody[2].SetActive(true);
      m_WholeBackBody[0].SetActive(true);
      m_WholeBackBody[1].SetActive(true);
      m_WholeBackBody[2].SetActive(true);
        
        
        
        m_CompleteBody.SetActive(true);
        m_AdditionalTopBody.SetActive(false);
        m_AdditionalBottomBody.SetActive(false);
        m_BCompleteBody.SetActive(true);
        m_BAdditionalTopBody.SetActive(false);
        m_BAdditionalBottomBody.SetActive(false);
        m_CentreBodyBtn.SetActive(true);
        m_TopBodyBtn.SetActive(false);
        m_BottomBodyBtn.SetActive(false);
        m_BCentreBodyBtn.SetActive(true);
        m_BTopBodyBtn.SetActive(false);
        m_BBottomBodyBtn.SetActive(false);
        m_LHandBtn.SetActive(true);
        m_RHandBtn.SetActive(true);
        m_BLHandBtn.SetActive(true);
        m_BRHandBtn.SetActive(true);
        m_LHandArea.SetActive(false);
        m_RHandArea.SetActive(false);
        m_LHandArea.transform.GetChild(0).gameObject.SetActive(true);
        m_RHandArea.transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i < m_LegRawImage.Length; i++)
        {
            m_LegRawImage[i].SetActive(false);
        }
        for (int i = 0; i < m_HandRawImage.Length; i++)
        {
            m_HandRawImage[i].SetActive(true);
        }
        for (int i = 0; i < m_LLegBtn.Length; i++)
        {
            m_LLegBtn[i].SetActive(false);
        }
        for (int i = 0; i < m_RLegBtn.Length; i++)
        {
            m_RLegBtn[i].SetActive(false);
        }
        for (int i = 0; i < m_LLegArea.Length; i++)
        {
            m_LLegArea[i].SetActive(true);
            m_LLegArea[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < m_RLegArea.Length; i++)
        {
            m_RLegArea[i].SetActive(true);
            m_RLegArea[i].transform.GetChild(0).gameObject.SetActive(false);

        }
        
        m_bodyMask.gameObject.SetActive(true);
        m_backbodyMask.gameObject.SetActive(true);
        m_rightHandMask.gameObject.SetActive(true);
        m_backrightHandMask.gameObject.SetActive(true);
        m_leftHandMask.gameObject.SetActive(true);
        m_backleftHandMask.gameObject.SetActive(true);
        m_rightLegMask.gameObject.SetActive(false);
        m_backrightLegMask.gameObject.SetActive(false);
        m_leftLegMask.gameObject.SetActive(false);
        m_backleftLegMask.gameObject.SetActive(false);
        m_bodyMask.sprite = shirt; 
        m_backbodyMask.sprite = bshirt; 
        m_rightHandMask.sprite =part ;
        m_backrightHandMask.sprite =part ;
        m_leftHandMask.sprite = part;
        m_backleftHandMask.sprite = part;
      
        ChangeTexture(m_MatBody,"_MainTex",m_OutlineTex);
        ChangeTexture(m_MatHand,"_MainTex",m_OutlineTex);
        //ChangeTexture(m_MatBody,"_MaskTex",mask);
     //   ChangeTexture(m_MatHand,"_MaskTex",mask);
        
        ChangeColor(m_MatBody,"_Color",Color.white);
        ChangeColor(m_MatHand,"_Color",Color.white);
        
        setMenu(m_subMenu);
        if (StickerHandler.Instance)
        {
            StickerHandler.Instance.ClearList();
        }
    }
      public void SettShirtMask(Sprite shirt,Sprite bshirt, Sprite part, Texture mask)
      {
//          print("her");
          UnActiveBody();
          m_WholeBody[0].SetActive(true);
          m_WholeBackBody[0].SetActive(true);
          
        m_CompleteBody.SetActive(true);
        m_AdditionalTopBody.SetActive(false);
        m_AdditionalBottomBody.SetActive(false);
        m_BCompleteBody.SetActive(true);
        m_BAdditionalTopBody.SetActive(false);
        m_BAdditionalBottomBody.SetActive(false);
        m_CentreBodyBtn.SetActive(true);
        m_TopBodyBtn.SetActive(false);
        m_BottomBodyBtn.SetActive(false);
        m_BCentreBodyBtn.SetActive(true);
        m_BTopBodyBtn.SetActive(false);
        m_BBottomBodyBtn.SetActive(false);
        m_LHandBtn.SetActive(false);
        m_RHandBtn.SetActive(false);
        m_BLHandBtn.SetActive(false);
        m_BRHandBtn.SetActive(false);
        m_LHandArea.SetActive(false);
        m_RHandArea.SetActive(false);
        m_LHandArea.transform.GetChild(0).gameObject.SetActive(false);
        m_RHandArea.transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 0; i < m_LegRawImage.Length; i++)
        {
            m_LegRawImage[i].SetActive(false);
        }
        for (int i = 0; i < m_HandRawImage.Length; i++)
        {
            m_HandRawImage[i].SetActive(false);
        }
        for (int i = 0; i < m_LLegBtn.Length; i++)
        {
            m_LLegBtn[i].SetActive(false);
        }
        for (int i = 0; i < m_RLegBtn.Length; i++)
        {
            m_RLegBtn[i].SetActive(false);
        }
        for (int i = 0; i < m_LLegArea.Length; i++)
        {
            m_LLegArea[i].SetActive(true);
            m_LLegArea[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < m_RLegArea.Length; i++)
        {
            m_RLegArea[i].SetActive(true);
            m_RLegArea[i].transform.GetChild(0).gameObject.SetActive(false);

        }
        
        m_bodyMask.gameObject.SetActive(true);
        m_backbodyMask.gameObject.SetActive(true);
        m_rightHandMask.gameObject.SetActive(false);
        m_backrightHandMask.gameObject.SetActive(false);
        m_leftHandMask.gameObject.SetActive(false);
        m_backleftHandMask.gameObject.SetActive(false);
        m_rightLegMask.gameObject.SetActive(false);
        m_backrightLegMask.gameObject.SetActive(false);
        m_leftLegMask.gameObject.SetActive(false);
        m_backleftLegMask.gameObject.SetActive(false);
        m_bodyMask.sprite = shirt; 
        m_backbodyMask.sprite = bshirt; 
      
      
        ChangeTexture(m_MatBody,"_MainTex",m_OutlineTex);
        //ChangeTexture(m_MatHand,"_BaseTex",m_OutlineTex);
        //ChangeTexture(m_MatBody,"_MaskTex",mask);
       // ChangeTexture(m_MatHand,"_MaskTex",mask);
       
       ChangeColor(m_MatBody,"_Color",Color.white);
       
       setMenu(m_subMenu);
       if (StickerHandler.Instance)
       {
           StickerHandler.Instance.ClearList();
       }
        
    }
    public void SetPantMask(Sprite shirt,Sprite bshirt, Sprite part, Texture mask)
    {  
        UnActiveBody();
        m_WholeBody[0].SetActive(true);
        m_WholeBody[3].SetActive(true);
        m_WholeBody[4].SetActive(true);
        m_WholeBackBody[0].SetActive(true);
        m_WholeBackBody[3].SetActive(true);
        m_WholeBackBody[4].SetActive(true);
        
        m_CompleteBody.SetActive(false);
        m_AdditionalTopBody.SetActive(true);
        m_AdditionalBottomBody.SetActive(true);
        m_BCompleteBody.SetActive(false);
        m_BAdditionalTopBody.SetActive(true);
        m_BAdditionalBottomBody.SetActive(true);
        m_CentreBodyBtn.SetActive(false);
        m_TopBodyBtn.SetActive(true);
        m_BottomBodyBtn.SetActive(true);
        m_BCentreBodyBtn.SetActive(false);
        m_BTopBodyBtn.SetActive(true);
        m_BBottomBodyBtn.SetActive(true);
        m_LHandBtn.SetActive(false);
        m_RHandBtn.SetActive(false);
        m_BLHandBtn.SetActive(false);
        m_BRHandBtn.SetActive(false);
        m_LHandArea.SetActive(true);
        m_RHandArea.SetActive(true);
        m_LHandArea.transform.GetChild(0).gameObject.SetActive(false);
        m_RHandArea.transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 0; i < m_LegRawImage.Length; i++)
        {
            m_LegRawImage[i].SetActive(true);
        }
        for (int i = 0; i < m_HandRawImage.Length; i++)
        {
            m_HandRawImage[i].SetActive(false);
        }
        for (int i = 0; i < m_LLegBtn.Length; i++)
        {
            m_LLegBtn[i].SetActive(true);
        }
        for (int i = 0; i < m_RLegBtn.Length; i++)
        {
            m_RLegBtn[i].SetActive(true);
        }
        for (int i = 0; i < m_LLegArea.Length; i++)
        {
            m_LLegArea[i].SetActive(false);
            m_LLegArea[i].transform.GetChild(0).gameObject.SetActive(true);
        }
        for (int i = 0; i < m_RLegArea.Length; i++)
        {
            m_RLegArea[i].SetActive(false);
            m_RLegArea[i].transform.GetChild(0).gameObject.SetActive(true);
        }
        m_bodyMask.gameObject.SetActive(true);
        m_backbodyMask.gameObject.SetActive(true);
        m_rightHandMask.gameObject.SetActive(false);
        m_backrightHandMask.gameObject.SetActive(false);
        m_leftHandMask.gameObject.SetActive(false);
        m_backleftHandMask.gameObject.SetActive(false);
        m_rightLegMask.gameObject.SetActive(true);
        m_backrightLegMask.gameObject.SetActive(true);
        m_leftLegMask.gameObject.SetActive(true);
        m_backleftLegMask.gameObject.SetActive(true);
         m_bodyMask.sprite =shirt ;
         m_backbodyMask.sprite =bshirt ;
         m_rightLegMask.sprite = part;
         m_backrightLegMask.sprite = part;
         m_leftLegMask.sprite = part;
         m_backleftLegMask.sprite = part;
         
         ChangeTexture(m_MatBody,"_MainTex",m_OutlineTex);
         ChangeTexture(m_MatLeg,"_MainTex",m_OutlineTex);
         // ChangeTexture(m_MatBody,"_MaskTex",mask);
         // ChangeTexture(m_MatLeg,"_MaskTex",mask);
         
         ChangeColor(m_MatBody,"_Color",Color.white);
         ChangeColor(m_MatLeg,"_Color",Color.white);
         
         setMenu(m_subMenu);
         if (StickerHandler.Instance)
         {
             StickerHandler.Instance.ClearList();
         }
    }
    
     public void SetFullBodyTexture(Texture texture)
    {
        
        
        ChangeTexture(m_MatBody,"_MainTex",texture);
        ChangeTexture(m_MatLeg,"_MainTex",texture);
        ChangeTexture(m_MatHand,"_MainTex",texture);
        
        ChangeColor(m_MatBody,"_Color",Color.white);
        ChangeColor(m_MatLeg,"_Color",Color.white);
        ChangeColor(m_MatHand,"_Color",Color.white);
    }
    public void ChangeTexture( Material[] m_Mat,string TextureType, Texture m_Tex)
    {
        for (int i = 0; i < m_Mat.Length; i++)
        {
            m_Mat[i].SetTexture(TextureType, m_Tex);
        }
    }
    public void ChangeColor( Material[] m_Mat,string TextureType, Color m_Color)
    {
        for (int i = 0; i < m_Mat.Length; i++)
        {
            m_Mat[i].SetColor(TextureType, m_Color);
        }
    }
    public void ClearTexture()
    {
        m_AdditionalBodyArea.transform.GetChild(0).GetComponent<RawImage>().texture = null ;
        m_BodyArea.transform.GetChild(0).GetComponent<RawImage>().texture = null ;
        m_LHandArea.transform.GetChild(0).GetComponent<RawImage>().texture = null ;
        m_RHandArea.transform.GetChild(0).GetComponent<RawImage>().texture = null ;
        for (int i = 0; i < m_LLegArea.Length; i++)
        {
            m_LLegArea[i].transform.GetChild(0).GetComponent<RawImage>().texture = null ;
        }
        for (int i = 0; i < m_RLegArea.Length; i++)
        {
            m_RLegArea[i].transform.GetChild(0).GetComponent<RawImage>().texture = null ;
        }  
        
        
        ChangeTexture(m_MatBody,"_MainTex",m_BaseTex);
        ChangeTexture(m_MatHand,"_MainTex",m_BaseTex);
        ChangeTexture(m_MatLeg,"_MainTex",m_BaseTex);
       // ChangeTexture(m_MatBody,"_MaskTex",null);
       // ChangeTexture(m_MatHand,"_MaskTex",null);
       // ChangeTexture(m_MatLeg,"_MaskTex",null);
        ChangeColor(m_MatBody,"_Color",Color.white);
        ChangeColor(m_MatLeg,"_Color",Color.white);
        ChangeColor(m_MatHand,"_Color",Color.white);
        
        m_AdditionalBodyArea.SetActive(false);
        m_BodyArea.SetActive(false);
        m_LHandArea.SetActive(false);
        m_RHandArea.SetActive(false);
        for (int i = 0; i < m_LLegArea.Length; i++)
        {
            m_LLegArea[i].SetActive(false);
        }
        for (int i = 0; i < m_RLegArea.Length; i++)
        {
            m_RLegArea[i].SetActive(false);

        }
        if (StickerHandler.Instance)
        {
            StickerHandler.Instance.ClearList();
        }

        if (SubMenuController.Instance)
        {
            SubMenuController.Instance.m_colorPlate.gameObject.SetActive(false);
        }

        foreach (var part in RenderTextureScaler.instance.shirtBody)
            {
                part.m_Material.mainTexture = null;
            }
       
            foreach (var part in RenderTextureScaler.instance.pantBody)
            {
                part.m_Material.mainTexture = null;
            }
        
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }


}
