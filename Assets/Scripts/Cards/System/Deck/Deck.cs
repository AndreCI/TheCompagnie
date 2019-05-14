using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDeck
{
    void AddCard(Card card, Unit owner=null);
    void RemoveCard(Card card, Unit owner=null);

    int Count(IEnumerable<Unit> owners= null);
    List<Card> GetCards(IEnumerable<Unit> owners=null);
}
