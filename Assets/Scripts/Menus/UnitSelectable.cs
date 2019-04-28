using System;
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

    void Start()
    {
        //Setup();
    }
    void Setup()
    {
        unit = PlayerInfos.Instance.compagnions[index];
        image.sprite = unit.portraitSprite;
        UnitSelector.Notify += SelectedUnitsUpdate;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(unit == null) { Setup(); }
        UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.SELECT);
    }

    private void SelectedUnitsUpdate(List<Unit> selectedUnits, UnitSelector.SELECTION_MODE mode)
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