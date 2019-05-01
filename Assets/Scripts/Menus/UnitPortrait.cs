﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Tweens;
using UnityEngine.EventSystems;

using static DuloGames.UI.Test_UIProgressBar;

public class UnitPortrait : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public DuloGames.UI.Test_UIProgressBar healthBar;
    public DuloGames.UI.Test_UIProgressBar manaBar;
    public DuloGames.UI.Test_UIProgressBar xpBar;
    public List<ActionPoint> actionsPoints;
    public GameObject actionPointsHolder;
    public Image portraitImage;
    public Text levelText;
    public Image levelImage;
    public Text unitName;
    public Unit linkedUnit;
    public GridLayoutGroup status;
    public Image selectionImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectionImage != null)
        {
            UnitSelector.Instance.ToggleSelection(linkedUnit, UnitSelector.SELECTION_MODE.SELECT);
            selectionImage.gameObject.SetActive(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT).Contains(linkedUnit));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(xpBar == null) {
            unitName.text = linkedUnit.unitName;
            levelImage.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (xpBar == null)
        {
            unitName.text = "";//linkedUnit.unitName;
            levelImage.gameObject.SetActive(false);
        }
    }

    public void Setup(Unit unit, bool setBarTo0=true)
    {
        linkedUnit = unit;
        levelText.text = unit.level.currentLevel.ToString();
        unitName.text = unit.unitName;
        healthBar.m_TextValue = unit.maxHealth;
        manaBar.m_TextValue = unit.maxMana;
        if(xpBar != null) { xpBar.m_TextValue = unit.level.nextLevelThreshold; }
        portraitImage.sprite = unit.portraitSprite;

        if (setBarTo0)
        {
            healthBar.SetFillAmount(0);
            manaBar.SetFillAmount(0);
            if (xpBar != null) { xpBar.SetFillAmount(0); }
        }
        if(levelImage!=null) { unitName.text = ""; levelImage.gameObject.SetActive(false); }
        actionsPoints = new List<ActionPoint>();
        if(actionPointsHolder != null) {
            actionsPoints = new List<ActionPoint>();
            foreach(ActionPoint ap in actionPointsHolder.GetComponentsInChildren<ActionPoint>())
            {
                ActionPoint Nap = ap.Setup(unit);
                if(Nap != null) { actionsPoints.Add(Nap); }
            }
        }

        if (selectionImage != null)
        {
            UnitSelector.Notify += UpdateSelection;
        }
        UpdatePortrait(1f);
    }

    public void UpdatePortrait(float duration=0.3f)
    {
        healthBar.StartTween(linkedUnit.CurrentHealth, duration, linkedUnit.maxHealth);
        manaBar.StartTween(linkedUnit.CurrentMana, duration, linkedUnit.maxMana);
        if (xpBar != null) { xpBar.StartTween(linkedUnit.level.currentXP, duration, linkedUnit.level.nextLevelThreshold); }
        if(status != null)
        {
            foreach(CombatStatus cs in linkedUnit.CurrentStatus)
            {
                if(cs.ui == null)
                {
                    AddNewCombatStatusUI(cs);
                }
            }
        }
        UpdateSelection(new List<Unit>(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT)), 
            UnitSelector.SELECTION_MODE.SELECT);
    }

    private void UpdateSelection(List<Unit> selected, UnitSelector.SELECTION_MODE mode)
    {
        if (selectionImage != null && mode == UnitSelector.SELECTION_MODE.SELECT)
        {

            selectionImage.gameObject.SetActive(selected.Contains(linkedUnit));
        }
    }

    private void AddNewCombatStatusUI(CombatStatus cs)
    {
        CombatStatusUI ui = Instantiate(CombatManager.Instance.combaStatusUI);
        ui.transform.SetParent(status.transform);
        ui.transform.localScale = new Vector3(1, 1, 1);
        ui.Setup(cs);
        cs.ui = ui;
    }
}
