using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabBase tabGroup;
    [SerializeField] private GameObject defaultBtn, choosenBtn;

    private void Start() => tabGroup.Subscribe(this);

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    public void SetImage(bool isSelected)
    {
        if (defaultBtn != null) defaultBtn.SetActive(!isSelected);
        if (choosenBtn != null) choosenBtn.SetActive(isSelected);
    }
}
