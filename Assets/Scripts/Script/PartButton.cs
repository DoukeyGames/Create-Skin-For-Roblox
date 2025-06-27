using UnityEngine;
using UnityEngine.UI;

public class PartButton : MonoBehaviour
{
    public RobloxBodyHandler robloxBodyHandler;
     public RawImage m_partImg1;
     public RawImage[] m_subpartImg;
     public Animator m_animator;
     public Animator[] m_subanimator;
     public bool m_subPart;
     public void Awake()
     {
         robloxBodyHandler = RobloxBodyHandler.Instance;
     }

     public void ApplyTexture()
     {
        if(StickerDragger.stickerApplyig)
            return;
        
        m_animator.gameObject.SetActive(true);
        m_animator.Play("open");
        m_subPart =true;
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
        robloxBodyHandler = RobloxBodyHandler.Instance;

        if (robloxBodyHandler.CurrentTexture)
        {
             
            if (m_partImg1) m_partImg1.texture = robloxBodyHandler.CurrentRawTexture;
            if (m_partImg1) m_partImg1.color = robloxBodyHandler.CurrentColor;
             
            if (m_subPart)
            {
                if (m_subpartImg.Length > 0)
                {
                    for (int i = 0; i < m_subpartImg.Length; i++)
                    {
                        m_subpartImg[i].texture = robloxBodyHandler.CurrentRawTexture;
                        m_subpartImg[i].color = robloxBodyHandler.CurrentColor;
                    }

                }
            }
        }
     }
}
