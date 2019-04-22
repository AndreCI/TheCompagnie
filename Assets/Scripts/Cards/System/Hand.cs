using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hand : DropZone, CardHandler
{
    public List<Card> cards;
    private static Hand instance;
    public static Hand Instance { get => instance; }

    void Awake()
    {
        instance = this;
    }

    public void RemoveFromHand(Card card)
    {
        cards.Remove(card);
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
        card = Instantiate(card);
        card.owner = CombatManager.Instance.player;
        card.handler = this;
        card.transform.SetParent(transform);
        cards.Add(card);
    }
}
