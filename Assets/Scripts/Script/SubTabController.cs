using System;
using System.Collections.Generic;
using UnityEngine;

public class SubTabController : MonoBehaviour
{
    public List<SubClassCategory> m_classCategory;
    public ColorPlate m_colorPlate;

    public static SubTabController Instance;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        OpenTab(0);
    }

    public void OpenTab(int i)
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
    public void CloseTab()
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