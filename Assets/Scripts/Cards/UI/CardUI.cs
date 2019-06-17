﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Card card;

    public GameObject header;
    public Image image;
    public Text manaCost;
    public Text delayCost;
    public Text description;
    public Text cardType;
    public Text cardTarget;
    public Image cardRarity;
    public List<Image> colorGlows;

    private bool playable = true;
    private bool setuped = false;
    public bool Playable
    {
        get => playable &&
            card.manaCost <= card.owner.CurrentMana &&
            card.actionCost <= card.owner.CurrentAction &&
            ((new List<Unit>(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.TPOTENTIAL))).Count > 0 ||
            (new List<Unit>(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.TCURRENT))).Count > 0)
            ; set
        {
            playable = value;
        }
    }

    public void Setup(Card card_)
    {
        if(card_.owner != null)
        {
            Setup(card_, card_.owner.GetCurrentColor(), card_.manaCost > card_.owner.CurrentMana ? Color.red : Color.white);
        }
        else
        {
            Setup(card_, new Color(1f, 1f, 1f, 1f), Color.white);
        }
    }

    public void Setup(Card card_, Color fixedColor, Color manaColor)
    {
        if (!setuped && card_.owner != null)
        {
        //    CardSelector.Notify += SelectedCardUpdate;
            card_.owner.NotifyUpdate += UnitUpdate_Notify;
            setuped = true;
        }
        else if(card.owner != null && card_.owner != null)
        {
            card.owner.NotifyUpdate -= UnitUpdate_Notify;
            card_.owner.NotifyUpdate += UnitUpdate_Notify;
        }
        card = card_;
        image.sprite = card.sprite;
        manaCost.text = card.manaCost.ToString();
        manaCost.color = manaColor;
        manaCost.transform.parent.transform.parent.gameObject.SetActive(card.manaCost > 0);
        delayCost.transform.parent.transform.parent.gameObject.SetActive(card.effects.Count(x=>!x.OnPlay) > 0);

        int delay = Mathf.Max((card.delay + (card.owner == null ? 0 : card.owner.currentSpeed)), 0);
        delayCost.text = delay.ToString();
        if (delay - card.delay == 0)
        {
            delayCost.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            delayCost.color =delay - card.delay > 0 ? new Color(1f, 0f, 0f, 1f) : new Color(0f, 1f, 0f, 1f);
        }
        description.text = card.GetDescription();
        if(cardType != null) { cardType.text = card.GetCardClassName() + " "+ card.GetCardType().ToString().ToLower(); }
        if(cardTarget != null) { cardTarget.text = card.GetTargetTypeDescription(); }
        if(cardRarity != null)
        {
            switch (card.rarity)
            {
                case CardDatabase.RARITY.COMMON:
                    cardRarity.color = new Color(0.8f, 0.8f, 0.8f, 0.75f);
                    break;
                case CardDatabase.RARITY.RARE:
                    cardRarity.color = new Color(0f, 0.8f, 1f, 0.65f);
                    break;
                case CardDatabase.RARITY.EPIC:
                    cardRarity.color = new Color(0.8f, 0f, 1f, 0.65f);
                    break;
                case CardDatabase.RARITY.STARTER:
                    cardRarity.color = new Color(1f, 0.8f, 0.0f, 0.65f);
                    break;
                case CardDatabase.RARITY.NONE:
                    cardRarity.color = new Color(0, 0, 0, 0);
                    break;
            }
        }


        foreach(Text head in header.GetComponentsInChildren<Text>())
        {
            head.text = card.Name;
        }
        foreach(Image i in colorGlows)
        {
            i.color = fixedColor;// card_.owner != null ? card_.owner.CurrentColor : new Color(1f, 1f, 1f, 0.5f);
           
        }
    }

    private void UnitUpdate_Notify()
    {
        if(card.manaCost > card.owner.CurrentMana)
        {
            manaCost.color = Color.red;
        }
        else
        {
            manaCost.color = Color.white;
        }description.text = card.GetDescription(source: card.owner);
        int delay = Mathf.Max((card.delay + (card.owner == null ? 0 : card.owner.currentSpeed)),0) ;
        delayCost.text = delay.ToString();
        if (delay - card.delay == 0)
        {
            delayCost.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            delayCost.color = delay - card.delay > 0 ? new Color(1f, 0f, 0f, 1f) : new Color(0f, 1f, 0f, 1f);
        }
    }

    public void Play(List<Unit> target)
    {
        card.Play(target);
        Hand.Instance.cards.Remove(this);
        Hand.Instance.SetCardUIs();
        CombatManager.Instance.compagnionDiscard.AddCard(card, card.owner);
        Destroy(gameObject);
    }

    public void OnDisable()
    {
    //    CardSelector.Notify -= SelectedCardUpdate;
        if (card.owner != null)
        {
            card.owner.NotifyUpdate -= UnitUpdate_Notify;
        }
    }

    public void OnDestroy()
    {
   //     CardSelector.Notify -= SelectedCardUpdate;
        if (card.owner != null)
        {
            card.owner.NotifyUpdate -= UnitUpdate_Notify;
        }

    }

    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;

    GameObject placeholder = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.COMBATPLAY);

       // CardSelector.Instance.ToggleSelection(card);
    }

    private void SelectedCardUpdate(List<Card> selectedCard)
    {
        if (!selectedCard.Contains(card))
        {
            image.color = Color.white;
        }
        else
        {
            image.color = Color.red;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        CardSelector.Instance.SelectOne(card);
        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

        parentToReturnTo = this.transform.parent;
        placeholderParent = parentToReturnTo;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
        CursorManager.Instance.type = CursorManager.CURSOR_TYPE.GRAB;
        ResetTransform();
        if(CombatManager.Instance.GetUnitUI(card.owner).portraitInfos != null)
        {
            int channelValue = card.channel? Mathf.Max(0, card.channelLenght + (card.owner == null ? 0 : card.owner.CurrentChannelValue)) : 0;
            CombatManager.Instance.GetUnitUI(card.owner).portraitInfos.phantomManaCost = card.channel ? channelValue * card.manaCost : card.manaCost;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
        if(CursorManager.Instance.type != CursorManager.CURSOR_TYPE.GRAB && CursorManager.Instance.type != CursorManager.CURSOR_TYPE.ATTACK)
            CursorManager.Instance.type = CursorManager.CURSOR_TYPE.GRAB;

        if (placeholder.transform.parent != placeholderParent)
            placeholder.transform.SetParent(placeholderParent);

        int newSiblingIndex = placeholderParent.childCount;

        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            if (this.transform.position.x < placeholderParent.GetChild(i).position.x)
            {

                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;

                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex);

    }
    private void ResetTransform()
    {
        RectTransform current = GetComponentsInChildren<RectTransform>()[1];
        current.localPosition = new Vector2(0f,
            0f);
        current.eulerAngles = (new Vector3(0f, 0f, 0f));
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        TurnManager.Instance.RemovePhantomEvents();

        CardSelector.Instance.UnselectOne();
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        Destroy(placeholder);
        List<UnitUI> target;
        if (CombatManager.Instance.GetUnitUI(card.owner).portraitInfos != null)
        {
            CombatManager.Instance.GetUnitUI(card.owner).portraitInfos.phantomManaCost = 0f;
        }
        if (!card.multipleTarget)
        {
            target = new List<UnitUI>(this.transform.GetComponentsInParent<UnitUI>());
        }
        else
        {
            target = new List<UnitUI>(this.transform.GetComponentsInParent<UnitUI>());
        }
        if (target != null && target.Count > 0) 
        {

            if (card.multipleTarget)
            {
                target.AddRange(CombatManager.Instance.GetFriendsUnitUI(target[0].unit));
                
            }
            if (card.actionCost > 0) { TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.COMBATENDTURN); }
            if (card.manaCost > 0)
            {
                if (TutorialManager.Instance.status[TutorialManager.TUTOTRIGGER.COMBATENDTURN])
                {
                    TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.COMBATMANA);
                }
            }
            if (card.channel)
            {
                TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.CHANNEL);
            }
            AudioManager.Instance?.PlayFromSet(AudioSound.AUDIO_SET.CARD_PLAY);
            Play(target.Select(x=>x.unit).ToList());
        }
        else
        {
            Hand.Instance.SetCardUIs();
        }
        CursorManager.Instance.type = CursorManager.CURSOR_TYPE.DEFAULT;


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       // UnitSelector.Instance.ForceSelection(new List<Unit> { card.owner }, UnitSelector.SELECTION_MODE.SELECT);
        if (Hand.Instance.cards.Contains(this))
        {
            CursorManager.Instance.type = CursorManager.CURSOR_TYPE.PREGRAB;

            ResetTransform();
            RectTransform current = GetComponentsInChildren<RectTransform>()[1];
            current.localPosition = new Vector2(current.localPosition.x,
                GetComponent<RectTransform>().sizeDelta.y * 0.1f + current.localPosition.y);
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            CardUI UI = TurnManager.Instance.cardPlaceHolder;
            UI.gameObject.SetActive(true);
            UI.Setup(card);
            UI.description.text = card.GetDescription(source: card.owner);
            UI.Playable = false;
            TurnManager.Instance.AddPhantomCombatEvent(card);

        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.type = CursorManager.CURSOR_TYPE.DEFAULT;
        //    UnitSelector.Instance.EndForceSelection(UnitSelector.SELECTION_MODE.SELECT);
        if (Hand.Instance.cards.Contains(this))
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Hand.Instance.SetCardUIs();
            TurnManager.Instance.cardPlaceHolder.gameObject.SetActive(false);
            if (GetComponent<CanvasGroup>().blocksRaycasts)
            {
                TurnManager.Instance.RemovePhantomEvents();
            }
        }
    }
}

