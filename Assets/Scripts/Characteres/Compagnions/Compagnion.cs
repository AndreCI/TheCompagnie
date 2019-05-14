using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class Compagnion : Unit
{
    public List<int> startingWeights;
    public override Unit Setup()
    {
        level = new Leveling(this);
        Unit copy = base.Setup();
        List<Card> cards = new List<Card>();
        int index = 0;
        foreach (Card card in PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, CardDatabase.RARITY.STARTER))
        {
            int i = startingWeights[index];
            index += 1;
            for (int j = 0; j < i; j++)
            {
                cards.Add(new Card(copy, card));
            }
        }
        copy.persistentDeck = new PersistentUnitDeck(cards);
        copy.persistentDeck.AddCardSlot();
        copy.persistentDeck.AddCardSlot();
        copy.persistentDeck.AddCardSlot();
        copy.id = Unit.currentId++;
        return copy;
    }

    public List<Card> DiscoverCard(int cardNumber = 3)
    {
        List<Card> cards = new List<Card>(PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, CardDatabase.RARITY.COMMON));
        cards = (PlayerInfos.Instance.cardDatabase.GetRandomCards(cardNumber,
            cards));

        return cards;
    }

    public override void TakeDamage(int amount)
    {
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.COMBATATTACKED);

        base.TakeDamage(amount);
        if(currentHealth <= 0)
        {
            PlayerInfos.Instance.gameOver.SetActive(true);
        }
    }
}
