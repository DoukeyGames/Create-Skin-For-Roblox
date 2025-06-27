using System;
using System.Collections;
using System.Collections.Generic;
using CW.Common;
using PaintIn3D;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cw2DButton : MonoBehaviour, IPointerDownHandler
{
    public GameObject m_StickerParent;
    public Image m_StickerTex;

    public void Start()
    {
         m_StickerTex.sprite = GetComponent<CwDemoButton>().IsolateTarget.GetComponent<CwDemoButtonBuilder>().Icon;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
      m_StickerParent.SetActive(true);
     
    }

    public void OnMouseDrag()
    {
        m_StickerParent.transform.position = new Vector3(Input.mousePosition.x,Input.mousePosition.y,0f);
    }
}
