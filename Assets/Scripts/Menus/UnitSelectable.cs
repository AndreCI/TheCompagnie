﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
public class UnitSelectable : MonoBehaviour, IPointerDownHandler
{

    public int index;
    public Image image;
    private Unit unit;



    void Setup()
    {
        unit = PlayerInfos.Instance.compagnions[index];
        UnitSelector.Notify += SelectedUnitsUpdate;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(unit == null) { Setup(); }
        UnitSelector.Instance.ToggleSelection(unit);
    }

    private void SelectedUnitsUpdate(List<Unit> selectedUnits)
    {
        if (!selectedUnits.Contains(unit))
        { //CALLED TWICE!
            image.color = Color.white;//.gameObject.SetActive(false);
        }
        else
        {
            image.color = Color.red;//selectionAnimation.gameObject.SetActive(true);
        }
    }
}