using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hand : UICardDropZone
{
    public CardUI cardUI;
    public List<CardUI> cards;
    private bool currentTurn;
    private static Hand instance;
    public static Hand Instance { get => instance; }

    public List<Card> inactiveCards;
    public List<Card> activeCards;
    public List<Unit> activatedUnits;

    public Text additionalCards;

    void Awake()
    {
        instance = this;
        cards = new List<CardUI>();
        inactiveCards = new List<Card>();
        activeCards = new List<Card>();
        activatedUnits = new List<Unit>();
        currentTurn = false;
        UnitSelector.Notify += UnitSelector_Notify;
    }
    private void OnDestroy()
    {
        UnitSelector.Notify -= UnitSelector_Notify;
    }

    private void SetCurrentHand()
    {
        foreach(CardUI c in cards)
        {
            c.gameObject.SetActive(activatedUnits.Contains(c.card.owner));
        }
    }
    private void UnitSelector_Notify(List<Unit> selected, UnitSelector.SELECTION_MODE mode)
    {

        if (selected.Count == 0 )//|| !selected.All(x => x.GetType() == typeof(Compagnion)))
        {
            selected = new List<Unit>(CombatManager.Instance.compagnions);
        }
        if (mode == UnitSelector.SELECTION_MODE.SELECT &&
            selected.Find(x=>x.GetType() == typeof(Enemy)) == null &&
         (selected.Except(activatedUnits).Any() || activatedUnits.Except(selected).Any()))
        {
            activatedUnits = new List<Unit>(selected);
            SetCurrentHand();
            SetCardUIs();
        }
        /*

        if(selected.Count == 0 || !selected.All(x => x.GetType() == typeof(Compagnion))) {  
        selected = new List<Unit>(CombatManager.Instance.compagnions);
        }
        if (mode == UnitSelector.SELECTION_MODE.SELECT && 
            (selected.Except(activatedUnits).Count() >0 ||
            activatedUnits.Except(selected).Count() > 0))
        {
            activatedUnits = selected;
            foreach(CardUI c in cards)
            {
                inactiveCards.Add(c.card);
            }
            cards = new List<CardUI>();
            activeCards = inactiveCards.FindAll(x => selected.Contains(x.owner));
            foreach(CardUI c in cards)
            {
                if (activeCards.Contains(c.card))
                {
                    activeCards.Remove(c.card);
                }
                else
                {
                    Destroy(c.gameObject);
                }
            }
            inactiveCards.RemoveAll(x=>selected.Contains(x.owner));
            AddToHand(activeCards);
        }*/
    }

    public List<Card> AddToHand(List<Card> cards_)
    {
        if(activatedUnits.Count == 0)
        {
            activatedUnits = new List<Unit>(CombatManager.Instance.compagnions);
        }
        List<Card> returnValue = new List<Card>();
        foreach (Card c in cards_)
        {
            if (inactiveCards.Count + cards.Count < 10)
            {
                AddToHand(c);
            }
        /*
                if (activatedUnits.Contains(c.owner) ||
                    activatedUnits.Count() == 0)
                {
                    AddToHand(c);
                }
                else
                {
                    inactiveCards.Add(c);
                }
            }*/
            else
            {
                returnValue.Add(c);
            }
        }
        return returnValue;
    }

    public void AddToHand(Card card)
    {
        CardUI UI = Instantiate(cardUI);
        UI.Setup(card);
        UI.transform.SetParent(transform);
        UI.parentToReturnTo = transform;
        UI.placeholderParent = transform;
        UI.transform.localScale = new Vector3(1f, 1f, 1f);
        cards.Add(UI);
        activeCards.Add(card);
        UI.Playable = !currentTurn;
        UI.gameObject.SetActive(activatedUnits.Contains(card.owner));
        SetCardUIs();
        AudioManager.Instance?.PlayFromSet(AudioSound.AUDIO_SET.CARD_DRAW);
    }

    public void SetLock(bool locked_)
    {
        currentTurn = locked_;
        foreach(CardUI card in cards)
        {
            card.Playable = !currentTurn;
        }
    }

    public void SetCardUIs()
    {
        List<CardUI> cardsToSet = new List<CardUI>(cards.FindAll(c => c.isActiveAndEnabled));
        additionalCards.text = "+" + (cards.Count - cardsToSet.Count).ToString() + "\n\n" +
            (cards.Count).ToString() + "/10";
        CombatManager.Instance.deckButton.GetComponentInChildren<Text>().text = "Deck (" + CombatManager.Instance.compagnionDeck.Count(activatedUnits).ToString() + ")";
        CombatManager.Instance.discardButton.GetComponentInChildren<Text>().text = "Discard (" + CombatManager.Instance.compagnionDiscard.Count(activatedUnits).ToString() + ")";
        if (cardsToSet.Count == 0) { return; }
        cardsToSet.Sort((x,y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));
        GetComponent<HorizontalLayoutGroup>().spacing = - (cardsToSet.Count - 1.5f) * cardsToSet[0].GetComponent<RectTransform>().sizeDelta.x * 0.07f;
        float maximum = GetHeight(((float)cardsToSet.Count - 1) / 2f, cardsToSet.Count);
        float mid = (cardsToSet.Count - 1) / 2f;
        for(int i =0; i < cardsToSet.Count; i++)
        {
            RectTransform current = cardsToSet[i].GetComponentsInChildren<RectTransform>()[1];
            float posy = (cardsToSet[i].GetComponent<RectTransform>().sizeDelta.y * 0.5f * GetHeight(i, cardsToSet.Count) / maximum) - cardsToSet[i].GetComponent<RectTransform>().sizeDelta.y * 0.6f;
            current.localPosition = new Vector2(current.localPosition.x,
                posy );
            current.eulerAngles = (new Vector3(0f, 0f, (mid - i) * GetRotateFactor(cardsToSet.Count)));
        }
    }
    private float GetRotateFactor(int number)
    {
        return 15f - number;
    }
    private float GetHeight(float index, int number)
    {
        float mid = (float)(number - 1) / 2f;
        float sigmaS = (float)number;// Mathf.Sqrt((float)(number - 1));
        float p = Mathf.Exp(-Mathf.Pow(((float)(index) - mid), 2) / (2 * sigmaS)) / Mathf.Sqrt(2 * Mathf.PI * sigmaS);
        return p;
        //p(x) = exp(-(x - mu) ^ 2 / (2 * sigma ^ 2)) / sqrt(2 * pi * sigma ^ 2)

    }
}
