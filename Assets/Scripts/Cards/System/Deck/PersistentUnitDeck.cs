using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PersistentUnitDeck : PersistentDeck
{

    private List<Card> cards;
    private int cardSlots;


    public PersistentUnitDeck(List<Card> cards_)
    {
        cards = cards_;
        cardSlots = cards.Count;
    }
    public override void AddCard(Card card, Unit owner = null)
    {
        cards.Add(card);
    }

    public override List<Card> GetCards(IEnumerable<Unit> owners = null)
    {
        return cards;
    }


    public CombatUnitDeck GenerateCombatDeck()
    {
        List<Card> newCard = new List<Card>();
        foreach (Card card in cards)
        {
            newCard.Add(new Card(card.owner, card));
        }
        return new CombatUnitDeck(newCard);
    }

    public override void RemoveCard(Card card, Unit owner = null)
    {
       cards.Remove(card);
    }

    public override void AddCardSlot(Unit owner = null)
    {
        cardSlots += 1;
    }

    public override int GetCardSlots(IEnumerable<Unit> owners = null)
    {
        return cardSlots;
    }

    public override int Count(IEnumerable<Unit> owners = null)
    {
        throw new NotImplementedException();
    }
}