using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GettableCardsDisplay : CardDIsplayWindow
{
    private List<Card> cards;

    private void Awake()
    {
        base.ProtectedStart();

        foreach (DuloGames.UI.UIItemSlot slot in slots)
        {
            slot.onUnassign.AddListener(CardUnAssigned);
            slot.onAssign.AddListener(CardAssigned);
            slot.dropEnabled = false;
            slot.dragEnabled = true;
            slot.swapEnabled = false;
            slot.allowThrowAway = true;

        }
    }




    protected override void CardAssigned(DuloGames.UI.UIItemSlot slot)
    {
        if (slot.GetItemInfo() != null)
        {
        }
    }
    protected override void CardUnAssigned(DuloGames.UI.UIItemSlot slot)
    {
        if (slot.GetItemInfo() != null)
        {
            slot.gameObject.SetActive(false);
        }
    }
}