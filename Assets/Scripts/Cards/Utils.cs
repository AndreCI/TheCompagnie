using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Utils
{
    private static Random rdx_;
    public static System.Random rdx
    {
        get
        {
            if (rdx_ == null)
            {
                rdx_ = new Random();
            }
            return rdx_;
        }
    }
   
}

[Serializable]
public class CardCollection
{
    public List<Card> cards;

    public List<Card> GetRandomCards(int number)
    {
        return new List<Card>(cards.OrderBy(x => Utils.rdx.Next()).Take(number));
    }

    public List<Card> GetCardByName(List<string> names)
    {
        List<Card> newCards = new List<Card>();
        foreach(string name in names)
        {
            foreach(Card c in cards)
            {
                if(c.Name == name)
                {
                    newCards.Add(new Card(null, c));
                }
            }
        }
        return newCards;
    }
}