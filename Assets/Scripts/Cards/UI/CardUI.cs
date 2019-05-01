using System;
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

    private bool playable = true;
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
        card = card_;
        image.sprite = card.sprite;
        manaCost.text = card.manaCost.ToString();
        delayCost.text = card.delay.ToString();
        description.text = card.Description;
        foreach(Text head in header.GetComponentsInChildren<Text>())
        {
            head.text = card.Name;
        }

        CardSelector.Notify += SelectedCardUpdate; 
    }
    public void Play(List<Unit> target)
    {
        card.Play(target);
        CombatManager.Instance.compagnionDiscard.AddCard(card, card.owner);
        Destroy(gameObject);
    }

    public void OnDisable()
    {
        CardSelector.Notify -= SelectedCardUpdate;
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
        List<UnitUI> target;
        if (!card.multipleTarget)
        {
            target = new List<UnitUI>(this.transform.GetComponentsInParent<UnitUI>());
        }
        else
        {
            PartyUI pui= this.transform.parent.GetComponent<PartyUI>();
            target = null;
            if (pui != null) {
                target = new List<UnitUI>(pui.units);
            }
        }
        if (target != null && target.Count > 0) 
        {
            Play(target.Select(x=>x.unit).ToList());
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UnitSelector.Instance.ForceSelection(new List<Unit> { card.owner }, UnitSelector.SELECTION_MODE.SELECT);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        UnitSelector.Instance.EndForceSelection(UnitSelector.SELECTION_MODE.SELECT);
    }
}

