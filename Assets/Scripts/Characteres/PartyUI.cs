using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : UICardDropZone, IPointerClickHandler
{
    public List<UnitUI> units;

    void Start()
    {
        targeting = false;
        selectorNotified = true;
        units = new List<UnitUI>(GetComponentsInChildren<UnitUI>());
        partyDropZone = true;
    }


    void Update()
    {

        if (!selectorNotified)
        {
            /*  if(CardSelector.Instance.GetSelectedCard().All(x => x.multipleTarget) && CardSelector.Instance.GetSelectedCard().Count()>0)
              {
                  TargetNotification();
              }
              else
              {
                  UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.TCURRENT);
              }*/
              foreach(UnitUI u in units)
            {
                u.selectorNotified = false;
                u.targeting = targeting;
            }
            //UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.TCURRENT);
            selectorNotified = true;

        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (UnitUI ui in units)
        {
            ui.OnPointerClick(eventData);
            //UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.SELECT);
        }
    }

}