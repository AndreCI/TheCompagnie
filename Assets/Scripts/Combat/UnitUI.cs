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
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider actionSlider;
    public SpriteRenderer Image;
    public ParticleSystem selectionAnimation;
    public ParticleSystem targetAnimation;


    void Start()
    {
        targeting = false;
        selectorNotified = true;
        CardSelector.Notify += SelectedCardUpdate;
    }

    void Update()
    {

        if (!selectorNotified)
        {
            if(CardSelector.Instance.GetSelectedCard().All(x => x.multipleTarget) && CardSelector.Instance.GetSelectedCard().Count()>0)
            {
                TargetNotification();
            }
            else
            {
                UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.TCURRENT);
            }
            selectorNotified = true;

        }
        
    }

    private void TargetNotification()
    {
        List<UnitUI> uis = new List<UnitUI>(transform.parent.GetComponentsInChildren<UnitUI>());
        foreach(UnitUI ui in uis)
        {
            UnitSelector.Instance.ToggleSelection(ui.unit, UnitSelector.SELECTION_MODE.TCURRENT);
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

        UpdateInfo();
    }
    
    public void UpdateInfo()
    {
        healthSlider.value = 1 - (float)unit.CurrentHealth / (float)unit.maxHealth;
        manaSlider.value = 1 - (float)unit.CurrentMana / (float)unit.maxMana;
        actionSlider.value = 1 - (float)unit.CurrentAction / (float)unit.maxAction;
        if (unit.CurrentHealth <= 0)
        {
            CombatManager.Instance.OnUnitDeath(unit);
        }
    }
    private void OnDestroy()
    {
        unit.NotifyUpdate -= UpdateInfo;
        UnitSelector.Notify -= this.SelectedUnitsUpdate;
        CardSelector.Notify -= this.SelectedCardUpdate;
    }
}