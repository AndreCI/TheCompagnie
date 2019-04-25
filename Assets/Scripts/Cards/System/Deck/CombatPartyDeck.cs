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
        Debug.Log(owners_.Count.ToString());
        Debug.Log(cards.Count.ToString());
    }

    public override void AddCard(Card card, Unit owner = null)
    {
        cards[owners_.IndexOf(owner)].AddCard(card);
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
        }if(owners == null)
        {
            owners = owners_;
        }
        List<Card> drawn = new List<Card>();
        foreach(Unit owner in owners)
        {
            int i = owners_.IndexOf(owner);
            drawn.AddRange(cards[i].DrawCards(new List<int> { number[i] }));
        }
        return drawn;
    }

    public override List<Card> GetCards(IEnumerable<Unit> owners = null)
    {
        List<Card> newCards = new List<Card>();
        if(owners == null)
        {
            owners = owners_;
        }
        foreach (Unit owner in owners)
        {
            newCards.AddRange(cards[owners_.IndexOf(owner)].GetCards());
        }
        return newCards;
    }

    public override void RemoveCard(Card card, Unit owner = null)
    {
        cards[owners_.IndexOf(owner)].RemoveCard(card);
    }

    public override void Shuffle(IEnumerable<Unit> owners = null)
    {
        if(owners == null)
        {
            owners = owners_;
        }
        foreach(Unit owner in owners)
        {
            Debug.Log(owners_.Count.ToString() + " " + owner.ToString());
            cards[owners_.IndexOf(owner)].Shuffle();
        }
    }
}
