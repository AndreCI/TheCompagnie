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

    public void OnPointerClick(PointerEventData eventData)
    {
        UnitSelector.Instance.AddCompagnionToSelection(unit);
        UnitSelector.Notify += S;
    }

    public void S(List<Unit> jack)
    {
        foreach(Unit u in jack)
        {
            Debug.Log("HERE" + u.ToString());
        }
    }
    public void SetInfos(Unit unit_)
    {
        unit = unit_;
        Image.sprite = unit_.combatSprite;
        //Image.preserveAspect = true;
        unit.NotifyUpdate += UpdateInfo;

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
    }
}