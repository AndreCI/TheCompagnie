﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class PersistentDeck : IDeck
{
    public abstract void AddCard(Card card, Unit owner = null);
    public abstract List<Card> GetCards(IEnumerable<Unit> owners = null);
    public abstract void RemoveCard(Card card, Unit owner = null);
}