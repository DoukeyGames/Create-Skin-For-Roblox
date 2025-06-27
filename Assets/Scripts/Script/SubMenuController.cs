using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuController : MonoBehaviour
{
    public List<SubClassCategory> m_classCategory;
    public ColorPlate m_colorPlate;

    public static SubMenuController Instance;
    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        OpenCategory(0);
    }

    public void OpenCategory(int i )
    {
        if (StickerHandler.Instance)
        {
            if(StickerHandler.Instance.currentSticker)
            Destroy(StickerHandler.Instance.currentSticker);
        }
        for (int j = 0; j < m_classCategory.Count; j++)
        {
            m_classCategory[j].m_Panel.SetActive(false);
        }
        m_classCategory[i].m_Panel.SetActive(true);

        if (m_colorPlate)
        {
            m_colorPlate.FinishApplyingTexture();
            m_colorPlate.gameObject.SetActive(false);
        }
    }
    public void closeCategory(  )
    {
        for (int j = 0; j < m_classCategory.Count; j++)
        {
            m_classCategory[j].m_Panel.SetActive(false);
        }

    }
}

[Serializable]
public class SubClassCategory
{
    public string m_Name;
    public GameObject m_Panel;
}