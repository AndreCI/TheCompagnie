using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : UICardDropZone, IPointerClickHandler
{
    public Unit unit;
    public SpriteRenderer Image;
    public ParticleSystem selectionAnimation;
    public ParticleSystem targetAnimation;
    public UnitPortrait portraitInfos;

    public Animator test;

    void Start()
    {
        targeting = false;
        selectorNotified = true;
        CardSelector.Notify += SelectedCardUpdate;
        if (test != null)
        {
        //    test.gameObject.SetActive(false);
        }

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
            UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.TCURRENT);
            selectorNotified = true;

        }
        
    }
    private void SelectedCardUpdate(List<Card> selectedCard)
    {
        if(selectedCard.FindAll(x=> x.potential_target != target_type).Count == 0 && selectedCard.Count>0)
        {
            UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.TPOTENTIAL, forceAdd:true);
        }
        else
        {
            UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.TPOTENTIAL, forceRemove: true);

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.SELECT);
        if (test != null)
        {
            test.Play("recover");
           // test.gameObject.SetActive(true);
        }
    }

    private void SelectedUnitsUpdate(List<Unit> selectedUnits, UnitSelector.SELECTION_MODE mode)
    {
        if (!selectedUnits.Contains(unit))
        {
            switch (mode)
            {
                case UnitSelector.SELECTION_MODE.SELECT:
                    selectionAnimation.gameObject.SetActive(false);
                    break;
                case UnitSelector.SELECTION_MODE.TPOTENTIAL:
                    targetAnimation.gameObject.SetActive(false);
                    break;
                case UnitSelector.SELECTION_MODE.TCURRENT:
                    
                    this.SelectedUnitsUpdate(new List<Unit>(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.TPOTENTIAL)),
                        UnitSelector.SELECTION_MODE.TPOTENTIAL);
                 //   targetAnimation.gameObject.SetActive(false);
                    break;

            }
            
        }
        else
        {
            switch (mode)
            {
                case UnitSelector.SELECTION_MODE.SELECT:
                    selectionAnimation.gameObject.SetActive(true);
                    break;
                case UnitSelector.SELECTION_MODE.TPOTENTIAL:
                    targetAnimation.startColor = Color.green;
                    targetAnimation.gameObject.SetActive(true);
                    break;
                case UnitSelector.SELECTION_MODE.TCURRENT:
                    targetAnimation.startColor = Color.red;

                    targetAnimation.gameObject.SetActive(true);
                    break;

            }
        }
    }
    public void SetInfos(Unit unit_)
    {
        unit = unit_;
        Image.sprite = unit_.combatSprite;
        //Image.preserveAspect = true;
        unit.NotifyUpdate += UpdateInfo;
        UnitSelector.Notify += SelectedUnitsUpdate;
        portraitInfos.Setup(unit);
        partyDropZone = false;
        UpdateInfo();
    }
    
    public void UpdateInfo()
    {
        portraitInfos.UpdatePortrait();
    }
    private void OnDestroy()
    {
        unit.NotifyUpdate -= UpdateInfo;
        UnitSelector.Notify -= this.SelectedUnitsUpdate;
        CardSelector.Notify -= this.SelectedCardUpdate;
    }
}