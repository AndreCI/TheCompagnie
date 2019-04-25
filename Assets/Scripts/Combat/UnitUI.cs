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
    public void OnPointerClick(PointerEventData eventData)
    {
        UnitSelector.Instance.ToggleSelection(unit);
    }

    private void SelectedUnitsUpdate(List<Unit> selectedUnits)
    {
        if (!selectedUnits.Contains(unit))
        { 
            selectionAnimation.gameObject.SetActive(false);
        }
        else 
        {
            selectionAnimation.gameObject.SetActive(true);
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
    }
}