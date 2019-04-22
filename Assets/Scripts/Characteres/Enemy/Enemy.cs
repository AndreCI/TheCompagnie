using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Unit
{
    public override Deck GetDeck()
    {
        return new Deck(this, new System.Collections.Generic.List<Card>());
    }
}
