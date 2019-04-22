using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compagnion : Unit
{
    public Card strike;
    public Card heal;

    public override Deck GetDeck()
    {
        return new Deck(this, new List<Card>{ strike, strike, strike, heal, strike });
    }
}
