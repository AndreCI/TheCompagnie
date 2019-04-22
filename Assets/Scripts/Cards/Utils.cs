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