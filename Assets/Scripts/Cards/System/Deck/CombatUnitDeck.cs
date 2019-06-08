using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CombatUnitDeck : CombatDeck
{
    private List<Card> cards;

    public CombatUnitDeck(IEnumerable<Card> collection)
    {
        cards = new List<Card>(collection);
    }

    public override void AddCard(Card card, Unit owner = null)
    {
        cards.Add(card);
    }

    public override int Count(IEnumerable<Unit> owners = null)
    {
        return cards.Count;
    }

    public override Card Draw(Unit owner = null)
    {
        if (cards.Count > 0)
        {
            Card card = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return card;
        }
        else { return null; }
    }

    public override List<Card> DrawCards(List<int> number, IEnumerable<Unit> owners = null)
    {
        List<Card> drawn = new List<Card>();
        while(drawn.Count< number[0] && cards.Count > 0)
        {
            drawn.Add(Draw());
        }
        return drawn;
    }

    public override List<Card> GetCards(IEnumerable<Unit> owners = null)
    {
        return cards;
    }

    public override List<Card> Redraw(IEnumerable<Card> cards_)
    {
        Card card = new List<Card>(cards_)[0];
        cards.Add(card);
        Shuffle();
        return new List<Card> { Draw() };
    }

    public override void RemoveCard(Card card, Unit owner = null)
    {
        cards.Remove(card);
    }

    public override void Shuffle(IEnumerable<Unit> owners = null)
    {
        AudioManager.Instance?.PlayFromSet(AudioSound.AUDIO_SET.CARD_SHUFFLE);
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = Utils.rdx.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }
}