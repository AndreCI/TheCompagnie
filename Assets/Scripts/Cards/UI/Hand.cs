using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hand : UICardDropZone
{
    public CardUI cardUI;
    public List<CardUI> cards;
    private bool currentTurn;
    private static Hand instance;
    public static Hand Instance { get => instance; }

    void Awake()
    {
        instance = this;
        cards = new List<CardUI>();
        currentTurn = false;
    }


    public void AddToHand(List<Card> cards)
    {
        foreach(Card c in cards)
        {
            AddToHand(c);
        }
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
        UI.Playable = !currentTurn;
    }

    public void SetLock(bool locked_)
    {
        currentTurn = locked_;
        foreach(CardUI card in cards)
        {
            card.Playable = !currentTurn;
        }
    }
}
