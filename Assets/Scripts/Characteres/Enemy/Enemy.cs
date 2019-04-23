using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Enemy : Unit
{
    public Card strike;
    public override Deck GetDeck()
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < 6; i++)
        {
            cards.Add(new Card(this, strike));
        }
        return new Deck(this, cards);
    }
}
