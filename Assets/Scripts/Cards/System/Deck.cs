using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;


public class Deck : CardHandler
{
    private List<Unit> owners;
    private List<Stack<Card>> cards;

    public List<Stack<Card>> Cards { get => cards; set => cards = value; }
    public List<Unit> Owners { get => owners; set => owners = value; }

    public Deck(Unit owner, List<Card> ownerCards)
    {
        owners = new List<Unit>();
        cards = new List<Stack<Card>>();
        cards.Add(new Stack<Card>(ownerCards));
        owners.Add(owner);
    }

    public Deck(List<Deck> decks)
    {
        owners = new List<Unit>();
        cards = new List<Stack<Card>>();
        foreach (Deck deck in decks)
        {
            owners.AddRange(deck.Owners);
            cards.AddRange(deck.cards);
        }
    }

    public List<Card> Draw(List<Unit> users = null)
    {
        if(users == null) { users = owners; }
        List<Card> drawnCards = new List<Card>();
        foreach(Unit u in users)
        {
            drawnCards.Add(Draw(u));
        }
        return drawnCards;
    }

    public Card Draw(Unit user)
    {
        if (!owners.Contains(user))
        {
            Debug.Log("User" + user.ToString() + " not found. skip draw");
            return null;
        }
        return cards[owners.IndexOf(user)].Pop();
    }

    public void Shuffle(List<Unit> users = null)
    {
        if (users == null) { users = owners; }
        foreach (Unit user in users)
        {
            if (!owners.Contains(user))
            {
                Debug.Log("User" + user.ToString() + " not found. skip draw");
                return;
            }
            Utils.Shuffle(cards[owners.IndexOf(user)]);
        }
    }
}
