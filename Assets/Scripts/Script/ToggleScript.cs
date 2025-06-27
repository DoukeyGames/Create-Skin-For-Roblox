using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScript : MonoBehaviour
{
    public GameObject icon;

   public void ChangeIcon()
    {
        if(icon.activeInHierarchy)
            icon.SetActive(false);
        else
            icon.SetActive(true);
        
    }
}
