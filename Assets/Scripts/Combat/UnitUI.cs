using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : UICardDropZone, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Unit unit;
    public SpriteRenderer Image;
    public ParticleSystem selectionAnimation;
    public ParticleSystem targetAnimation;
    public UnitPortrait portraitInfos;

    public StatusAnimator statusAnimator;
    public SpriteRenderer effectSpriteRenderer;
    public EffectAnimation currentAnimation;

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!locked)
        {
            if (eventData.pointerDrag == null)
                return;

            CardUI d = eventData.pointerDrag.GetComponent<CardUI>();
            if (d != null && d.Playable && IsAcceptableTarget(d) && d.placeholderParent == this.transform)
            {
                d.placeholderParent = d.parentToReturnTo;
                targeting = false;
                selectorNotified = false;
                CursorManager.Instance.type = CursorManager.CURSOR_TYPE.GRAB;

                if (d.card.multipleTarget)
                {
                    foreach (UnitUI friendUI in CombatManager.Instance.GetFriendsUnitUI(unit))
                    {
                        friendUI.targeting = false;
                        friendUI.selectorNotified = false;
                    }
                }
            }
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!locked)
        {
            if (eventData.pointerDrag == null)
                return;

            CardUI d = eventData.pointerDrag.GetComponent<CardUI>();
            if (d != null && d.Playable && IsAcceptableTarget(d))
            {
                d.placeholderParent = this.transform;
                targeting = true;
                selectorNotified = false;
                CursorManager.Instance.type = CursorManager.CURSOR_TYPE.ATTACK;

                if (d.card.multipleTarget)
                {
                    foreach(UnitUI friendUI in CombatManager.Instance.GetFriendsUnitUI(unit))
                    {
                        friendUI.targeting = true;
                        friendUI.selectorNotified = false;

                    }
                }
            }
        }
    }
    public override void OnDrop(PointerEventData eventData)
    {
        if (!locked)
        {
            //            Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

            CardUI d = eventData.pointerDrag.GetComponent<CardUI>();
            if (d != null && d.Playable && IsAcceptableTarget(d))
            {
                d.parentToReturnTo = this.transform;
                targeting = false;
                selectorNotified = false;
                if (d.card.multipleTarget)
                {
                    foreach (UnitUI friendUI in CombatManager.Instance.GetFriendsUnitUI(unit))
                    {
                        friendUI.targeting = false;
                        friendUI.selectorNotified = false;
                    }
                }
            }
        }

    }
    void Start()
    {
       // currentAnimation = new List<EffectAnimation>();
        targeting = false;
        selectorNotified = true;
        CardSelector.Notify += SelectedCardUpdate;
   

    }

    void Update()
    {

        if (!selectorNotified)
        {
            UnitSelector.Instance.ToggleSelection(unit, UnitSelector.SELECTION_MODE.TCURRENT);
            selectorNotified = true;
        }
        
    }
    private void SelectedCardUpdate(List<Card> selectedCard)
    {
        if(selectedCard.FindAll(x=> !IsAcceptableTarget(x)).Count == 0 && selectedCard.Count>0)
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
                    break;
                case UnitSelector.SELECTION_MODE.SHOWSOURCE:
                    selectionAnimation.startColor = Color.cyan;
                    this.SelectedUnitsUpdate(new List<Unit>(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT)),
                        UnitSelector.SELECTION_MODE.SELECT);
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

                case UnitSelector.SELECTION_MODE.SHOWSOURCE:
                    selectionAnimation.startColor = Color.blue;
                    selectionAnimation.gameObject.SetActive(true);
                    break;

            }
        }
    }
    public void SetInfos(Unit unit_)
    {
        portraitInfos.gameObject.SetActive(true);
        unit = unit_;
        Image.sprite = unit_.combatSprite;
        //Image.preserveAspect = true;
        unit.NotifyUpdate += UpdateInfo;
        UnitSelector.Notify += SelectedUnitsUpdate;
        portraitInfos.Setup(unit);
        partyDropZone = false;
        UpdateInfo();
    }

    public void Disable()
    {

            portraitInfos.gameObject.SetActive(false);
            gameObject.SetActive(false);
    }
    
    public void UpdateInfo()
    {
        portraitInfos.UpdatePortrait();
    }
    private void OnDestroy()
    {
        if (unit != null)
        {
            unit.NotifyUpdate -= UpdateInfo;
            UnitSelector.Notify -= this.SelectedUnitsUpdate;
            CardSelector.Notify -= this.SelectedCardUpdate;
        }
    }
    private void FixedUpdate()
    {
        currentAnimation?.FixedUpdate(Time.fixedDeltaTime);
    }

    private bool IsAcceptableTarget(Card card)
    {
        return (card.potential_target == Card.POTENTIAL_TARGET.SELF && card.owner == unit) ||
            (card.potential_target == target_type);
    }

    public override bool IsAcceptableTarget(CardUI card)
    {
        return base.IsAcceptableTarget(card) || (card.card.potential_target == Card.POTENTIAL_TARGET.SELF && card.card.owner == unit);

    }
}