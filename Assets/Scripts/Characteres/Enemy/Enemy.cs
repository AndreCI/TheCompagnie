using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class Enemy : Unit
{
    public int xpValue;

    public override Unit Setup()
    {
        int deckSizeFactor = 3;
        Unit copy = base.Setup();
        List<Card> cards = new List<Card>();
        foreach (Card card in PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards))
        {
            int i = 0;
            if (card.Name == "Bite")
            {
                i = 7;
            }
            else if (card.Name == "Defend")
            {
                i = 3;
            }
            else if (card.Name == "Hide")
            {
                i = 4;
            }else if(card.Name== "Brutal Mutation")
            {
                i = 3;
            }else if(card.Name == "Toxic Cloud")
            {
                i = 4;
            }else if(card.Name == "Slow Mutation")
            {
                i = 3;
            }else if(card.Name == "Poison Burst")
            {
                i = 1;
            }else if(card.Name == "Crippling Poison")
            {
                i = 2;
            }

            for (int j = 0; j < i * deckSizeFactor; j++)
            {
                cards.Add(new Card(copy, card));
            }
        }
        copy.persistentDeck = new PersistentUnitDeck(cards);
        return copy;
    }
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if(currentHealth <= 0)
        {
            foreach(CombatStatus status in CurrentStatus)
            {
                TurnManager.NotifyAll -= status.Notified;
                GameObject.Destroy(status.ui.gameObject);
            }
            CombatManager.Instance.OnUnitDeath(this);
        }
    }

}
