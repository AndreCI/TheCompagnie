using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hand : UICardDropZone, CardHandler
{
    public CardUI cardUI;
    public List<CardUI> cards;
    private static Hand instance;
    public static Hand Instance { get => instance; }

    void Awake()
    {
        instance = this;
        cards = new List<CardUI>();
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
        card.handler = this;
        UI.transform.SetParent(transform);
        UI.parentToReturnTo = transform;
        UI.placeholderParent = transform;
        cards.Add(UI);
        UI.Playable = true;
    }

    public void SetLock(bool locked)
    {
        foreach(CardUI card in cards)
        {
            card.Playable = !locked;
        }
    }
}
