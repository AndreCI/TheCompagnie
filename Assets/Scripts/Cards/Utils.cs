using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Utils
{
   
}

[Serializable]
public class CardCollection
{
    public List<Card> cards;

    public List<Card> GetRandomCards(int number)
    {
        Random rnd = new Random();
        return new List<Card>(cards.OrderBy(x => rnd.Next()).Take(number));
    }
}