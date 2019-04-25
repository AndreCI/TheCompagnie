using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PersistentUnitDeck : PersistentDeck
{

    private List<Card> cards;

    public PersistentUnitDeck(List<Card> cards_)
    {
        cards = cards_;
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
}