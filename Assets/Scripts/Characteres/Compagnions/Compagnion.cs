using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class Compagnion : Unit
{
    public Card strike;
    public Card heal;

    public override Deck GetDeck()
    {
        List<Card> cards = new List<Card>();
        for(int i = 0; i <15; i++)
        {
            cards.Add(new Card(this, strike));
        }
        cards.Add(new Card(this, heal));
        return new Deck(this, cards);
    }

    public void GainMana(int amount)
    {
        currentMana += amount;
        if(currentMana > maxMana)
        {
            currentMana = maxMana;
        }
            UpdateInfo();
    }
    public void GainAction(int amount)
    {
        currentAction += amount;
        if(currentAction > maxAction)
        {
            currentAction = maxAction;
        }
        UpdateInfo();
    }
}
