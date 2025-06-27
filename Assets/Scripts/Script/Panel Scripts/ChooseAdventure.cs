using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseAdventure : MonoBehaviour
{

    public Image[] Adventures;
    public GameObject[] Highlighted;
    public Color NormalColor, HighlightColor;
    public void Start()
    {
        ChangeAdventure(1);
    }

    // Update is called once per frame
    public void ChangeAdventure(int i)
    {
        for (int j = 0; j < Adventures.Length; j++)
        {
            Adventures[j].color = NormalColor;
        }
        for (int j = 0; j < Highlighted.Length; j++)
        {
            Highlighted[j].SetActive(false);
        }
        Adventures[i].color = HighlightColor;
        Highlighted[i].SetActive(true);
    }
}
