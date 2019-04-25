using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PersistentPartyDeck : PersistentDeck
{
    private List<Unit> owners_;
    private List<PersistentUnitDeck> cards;

    public PersistentPartyDeck(IEnumerable<Unit> owners, List<PersistentUnitDeck> decks)
    {
        owners_ = new List<Unit>(owners);
        cards = decks;
    }
    public override void AddCard(Card card, Unit owner = null)
    {
        cards[owners_.IndexOf(owner)].AddCard(card);
    }

    public override List<Card> GetCards(IEnumerable<Unit> owners = null)
    {
        if(owners == null || (new List<Unit>(owners)).Count == 0)
        {
            owners = owners_;
        }
        List<Card> newCards = new List<Card>();
        foreach(Unit owner in owners)
        {
            newCards.AddRange(cards[owners_.IndexOf(owner)].GetCards());
        }
        return newCards;
    }

    public CombatPartyDeck GenerateCombatDeck(IEnumerable<Unit> owners)
    {
                if (owners == null || (new List<Unit>(owners)).Count == 0)
        {
            owners = owners_;
        }
        List<CombatUnitDeck> combatDecks = new List<CombatUnitDeck>();
        foreach (Unit owner in owners)
        {
            combatDecks.Add(cards[owners_.IndexOf(owner)].GenerateCombatDeck());
        }
        return new CombatPartyDeck(owners, combatDecks);
    }

    public override void RemoveCard(Card card, Unit owner = null)
    {
        cards[owners_.IndexOf(owner)].RemoveCard(card);
    }
}
