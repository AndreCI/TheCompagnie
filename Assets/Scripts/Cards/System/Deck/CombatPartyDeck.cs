using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CombatPartyDeck : CombatDeck
{
    private List<Unit> owners_;
    private List<CombatUnitDeck> cards;

    public void RenewDeck(Unit owner, CombatUnitDeck deck)
    {

        cards[owners_.IndexOf(owner)] = deck;
    }
    public CombatPartyDeck(IEnumerable<Unit> owners, List<CombatUnitDeck> decks)
    {
        owners_ = new List<Unit>(owners);
        if(decks == null)
        {
            decks = new List<CombatUnitDeck>();
            foreach(Unit owner in owners)
            {
                decks.Add(new CombatUnitDeck(new List<Card>()));
            }
        }
        cards = decks;
    }

    public override void AddCard(Card card, Unit owner = null)
    {
        cards[owners_.IndexOf(owner)].AddCard(card);
    }

    public void AddCards(List<Card> cards, List<Unit> owners_ = null)
    {
        if(owners_ == null)
        {
            owners_ = new List<Unit>(cards.Select(x => x.owner));
        }
        int i = 0;
        foreach(Card c in cards)
        {
            AddCard(c, owners_[i]);
            i++;
        }
    }

    public override int Count(IEnumerable<Unit> owners = null)
    {
        int totalCount = 0;
        if (owners == null || (new List<Unit>(owners)).Count == 0)
        {
            owners = owners_;
        }
        foreach (Unit owner in owners)
        {
            totalCount += cards[owners_.IndexOf(owner)].Count();
        }
        return totalCount;
    }

    public override Card Draw(Unit owner = null)
    {
        return cards[owners_.IndexOf(owner)].Draw();
    }

    public override List<Card> DrawCards(List<int> number = null, IEnumerable<Unit> owners = null)
    {
        if(number == null)
        {
            number = Enumerable.Repeat(1, owners_.Count).ToList();
        }
                if (owners == null || (new List<Unit>(owners)).Count == 0)
        {
            owners = owners_;
        }
        List<Card> drawn = new List<Card>();
        int j = 0;
        foreach(Unit owner in owners)
        {
            int i = owners_.IndexOf(owner);
            drawn.AddRange(cards[i].DrawCards(new List<int> { number[j] }));
            j++;
        }
        return drawn;
    }

    public override List<Card> GetCards(IEnumerable<Unit> owners = null)
    {
        List<Card> newCards = new List<Card>();
                if (owners == null || (new List<Unit>(owners)).Count == 0)
        {
            owners = owners_;
        }
        foreach (Unit owner in owners)
        {
            newCards.AddRange(cards[owners_.IndexOf(owner)].GetCards());
        }
        return newCards;
    }

    public override List<Card> Redraw(IEnumerable<Card> cards_)
    {
        List<Card> redrawn = new List<Card>();
        foreach(Card card in cards_)
        {
            redrawn.AddRange(cards[owners_.IndexOf(card.owner)].Redraw(new List<Card> { card }));
        }
        return redrawn;
    }

    public override void RemoveCard(Card card, Unit owner = null)
    {
        cards[owners_.IndexOf(owner)].RemoveCard(card);
    }

    public override void Shuffle(IEnumerable<Unit> owners = null)
    {
        if (owners == null || (new List<Unit>(owners)).Count == 0)
        {
            owners = owners_;
        }
        foreach (Unit owner in owners)
        {
            cards[owners_.IndexOf(owner)].Shuffle();
        }
    }
}
