using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DeckDisplayUI : MonoBehaviour
{

    public List<DuloGames.UI.UIItemSlot> slots;
    public GameObject tab1;

    private void Start()
    {
       
        slots = new List<DuloGames.UI.UIItemSlot>(tab1.transform.GetComponentsInChildren<DuloGames.UI.UIItemSlot>(true));
       
    }
    public void Setup(PersistentDeck deck)
    {
        List<Card> cards = deck.GetCards();
        Setup(cards, deck.GetCardSlots());
    }

    public void Setup(CombatDeck deck)
    {
        List<Card> cards = deck.GetCards();
        Setup(cards, 0);
    }

    public void Setup(List<Card> cards, int slotNumber=0)
    {
        Clear();
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < cards.Count)
            {
                slots[i].Assign(cards[i]);
            }
            else if (i < slotNumber && slotNumber >0)
            {
                slots[i].Unassign();
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
    public void Clear()
    {
        if (slots == null || slots.Count == 0)
        {
            slots = new List<DuloGames.UI.UIItemSlot>(tab1.transform.GetComponentsInChildren<DuloGames.UI.UIItemSlot>(true));

        }
        {
            foreach (DuloGames.UI.UIItemSlot slot in slots)
            {
                slot.gameObject.SetActive(true);
                slot.Unassign();
            }
        }
    }
}