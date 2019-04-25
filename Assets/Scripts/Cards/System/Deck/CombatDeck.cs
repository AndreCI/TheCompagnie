using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class CombatDeck : IDeck
{
    public abstract void AddCard(Card card, Unit owner=null);
    public abstract Card Draw(Unit owner=null);
    public abstract List<Card> DrawCards(List<int> number = null, IEnumerable<Unit> owners = null);
    public abstract void Shuffle(IEnumerable<Unit> owners = null);
    public abstract void RemoveCard(Card card, Unit owner=null);
    public abstract List<Card> GetCards(IEnumerable<Unit> owners = null);
    
}
