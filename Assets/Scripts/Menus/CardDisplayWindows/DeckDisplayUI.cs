using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DeckDisplayUI : CardDIsplayWindow
{

    public Text deckType;
    private IDeck deck;
    private bool updateDeck;

    private void Awake()
    {
        base.ProtectedStart();
        updateDeck = false;
        
        foreach (DuloGames.UI.UIItemSlot slot in slots)
        {
            slot.onUnassign.AddListener(CardUnAssigned);
            slot.onAssign.AddListener(CardAssigned);
            slot.dragAndDropEnabled = false;
            slot.swapEnabled = false;
            slot.allowThrowAway = false;

        }
    }
    public void Setup(IDeck deck_)
    {
        deck = deck_;
        Setup(deck.GetCards(), (deck is PersistentDeck)? (deck as PersistentDeck).GetCardSlots() : 0);
    }

    public void Setup(List<Card> cards)
    {
        Setup(cards, 0);
    }

    protected override void Setup(List<Card> cards, int slotNumber=0)
    {
        updateDeck = false;
        base.Setup(cards, slotNumber);
        updateDeck = true;
    }


    protected override void CardAssigned(DuloGames.UI.UIItemSlot slot)
    {
        if(slot.GetItemInfo() != null && updateDeck)
        {
            Card copy = GeneralUtils.Copy<Card>(slot.GetItemInfo());
            copy.owner = PlayerInfos.Instance.unitsWindow.unit;
            deck.AddCard(copy, PlayerInfos.Instance.unitsWindow.unit);
            slot.dragAndDropEnabled = false;
        }
    }
    protected override void CardUnAssigned(DuloGames.UI.UIItemSlot slot)
    {
        if (slot.GetItemInfo() != null && updateDeck)
        {
            Card copy = GeneralUtils.Copy<Card>(slot.GetItemInfo());
            copy.owner = PlayerInfos.Instance.unitsWindow.unit;
            deck.RemoveCard(copy, PlayerInfos.Instance.unitsWindow.unit);
            slot.dropEnabled = true;
        }
    }
}