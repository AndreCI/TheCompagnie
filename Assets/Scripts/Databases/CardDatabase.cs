using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class CardDatabase : ScriptableObject
{
   public enum RARITY {NONE, STARTER, COMMON };

    public enum CARDCLASS { CRUSADER, ALCHEMIST, WOLF, ABOMINATION, GLOBAL, WARRIOR};


    public string Name;
    public CARDCLASS cardClass;
    public RARITY rarityLevel;
    public List<CardDatabase> childrenDatabase;
        public Card[] items;

 

    public void Setup() { 
        if (childrenDatabase != null && childrenDatabase.Count > 0)
        {
            List<Card> litems = new List<Card>();
            int idOffset = 0;
            foreach (CardDatabase database in childrenDatabase)
            {
                database.Setup();
                foreach (Card card in database.GetAllCards())
                {
                    card.databasePath += "/" + Name;
                    card.ID += idOffset;
                    litems.Add(card);
                }
                idOffset += database.items.Length;
            }
            items = litems.ToArray();
        }
        else
        {
            foreach (Card card in items)
            {
                card.databasePath = Name;
                card.cardClass = cardClass;
                card.rarity = rarityLevel;
            }
        }
    }

    public IEnumerable<Card> GetCardsFromClass(CARDCLASS cClass, RARITY rarity = RARITY.NONE)
    {
        if(childrenDatabase.Count == 0 && cardClass == cClass && (rarity == rarityLevel || rarity == RARITY.NONE))
        {
            return items;
        }
        else if (childrenDatabase.Count == 0)
        {
            return new Card[0];
        }else
        {
            List<Card> cards = new List<Card>();
            foreach(CardDatabase database in childrenDatabase)
            {
                cards.AddRange(database.GetCardsFromClass(cClass, rarity));
            }
            return cards; 
        }
    }

    public IEnumerable<Card> GetAllCards()
    {
        return items;
    }
        /// <summary>
        /// Get the specified ItemInfo by index.
        /// </summary>
        /// <param name="index">Index.</param>
        public Card Get(int index)
        {
            return (this.items[index]);
        }

        /// <summary>
        /// Gets the specified ItemInfo by ID.
        /// </summary>
        /// <returns>The ItemInfo or NULL if not found.</returns>
        /// <param name="ID">The item ID.</param>
        public Card GetByID(int ID)
        {
            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i].ID == ID)
                    return this.items[i];
            }

            return null;
        }

    /// <summary>
    /// Get the specified by name
    /// </summary>
    /// <param name="name">Name.</param>
    public Card GetByName(string name)
    {
        for (int i = 0; i < this.items.Length; i++)
        {
            if (this.items[i].Name == name)
                return this.items[i];
        }

        return null;
    }

    public List<Card> GetRandomCards(int number, IEnumerable<Card> forcedOptions = null)
    {
        List<Card> cards = new List<Card>(forcedOptions ?? items);
        System.Random rnd = new System.Random();
        return new List<Card>(cards.OrderBy(x => rnd.Next()).Take(number));
    }

}