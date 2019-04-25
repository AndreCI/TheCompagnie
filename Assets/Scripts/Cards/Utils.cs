using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Utils
{
    public static void Shuffle<T>(Stack<T> stack)
    {
        var values = stack.ToArray();
        var random = new System.Random();
        stack.Clear();
        foreach (var value in values.OrderBy(x => random.Next()))
            stack.Push(value);
    }
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