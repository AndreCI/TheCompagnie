﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Card card;
    public Image image;

    private bool playable = true;
    public bool Playable
    {
        get => playable &&
            card.manaCost <= card.owner.CurrentMana &&
            card.actionCost <= card.owner.CurrentAction 
            ; set
        {
            playable = value;
        }
    }

    public void Setup(Card card_)
    {
        card = card_;
        image.sprite = card.sprite;
        CardSelector.Notify += SelectedCardUpdate; 
    }
    public void Play(Unit target)
    {
        card.Play(target);
        CombatManager.Instance.compagnionDiscard.AddCard(card, card.owner);
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        CardSelector.Notify -= SelectedCardUpdate;

    }

    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;

    GameObject placeholder = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        CardSelector.Instance.ToggleSelection(card);
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

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;

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

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        CardSelector.Instance.UnselectOne();
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        Destroy(placeholder);
        UnitUI target = this.transform.parent.GetComponent<UnitUI>();
        if (target != null)
        {
            Play(target.unit);
        }

    }
}

