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
    public bool isAbstract;


    public override string ToString()
    {
        string returnval = "Deck(s) of : ";
        foreach(Unit owner in owners)
        {
            returnval += owner.ToString() + " ";
        }
        returnval += "\n";
        List<Stack<Card>> newList = new List<Stack<Card>>();
        foreach(Stack<Card> sc in cards)
        {
            returnval += ("("+sc.Count.ToString()+" cards)\n");
            Stack<Card> newStack = new Stack<Card>();
            while(sc.Count>0)
            {
                Card c = sc.Pop();
                returnval += c.ToString() + "\n";
                newStack.Push(c);
            }
            newList.Add(newStack);
            returnval += "------" + "\n";
        }
        cards = newList;

        return returnval;

    }
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

    public void AddCard(Unit owner, Card card)
    {
        cards[owners.IndexOf(owner)].Push(card);
    }

    public List<Card> Draw(List<Unit> users = null)
    {
        if(users == null) { users = owners; }
        List<Card> drawnCards = new List<Card>();
        foreach(Unit u in users)
        {
            Card drawn = Draw(u);
            if (drawn != null)
            {
                drawnCards.Add(drawn);
            }
        }
        return drawnCards;
    }

    public Card Draw(Unit user)
    {
        if (!owners.Contains(user))
        {
            Debug.Log("User" + user.ToString() + " not found. skip draw");
            return null;
        }if(cards[owners.IndexOf(user)].Count <= 0)
        {
            return null;
        }
        return cards[owners.IndexOf(user)].Pop();
    }

    public List<Card> GetCards()
    {
        List<Card> allCards = new List<Card>();
        foreach(Stack<Card> cs in cards)
        {
            foreach(Card c in cs)
            {
                allCards.Add(c);
            }
        }
        return allCards;
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
