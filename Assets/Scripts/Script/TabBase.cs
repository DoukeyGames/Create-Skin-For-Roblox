using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TabBase : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public string eventName;

    public virtual void Subscribe(TabButton button){}

    public virtual void OnTabEnter(TabButton button){}

    public virtual void OnTabExit(TabButton button){}

    public virtual void OnTabSelected(TabButton button){}
}
