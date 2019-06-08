using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuloGames.UI;
using Unity;
using UnityEngine;

public class RemovableCardDisplay : CardDIsplayWindow
{
    public DeckDisplayUI deckDisplay;
    public Sprite destroyedIcon;
    public Sprite pitIcon;

    private UIItemSlot currentSlot;

    private void Awake()
    {
        base.ProtectedStart();
    }

    public void Setup(int slotNumber)
    {
        base.Setup(new List<Card>(), slotNumber);

        foreach (UIItemSlot slot in slots)
        {
            slot.onAssign.AddListener(CardAssigned);
            slot.dragAndDropEnabled = false;
            slot.dropEnabled = true;
            slot.swapEnabled = false;
            slot.allowThrowAway = false;
            slot.SetIcon(pitIcon);
        }
        foreach (UIItemSlot s in deckDisplay.slots)
        {
            s.dragEnabled = true;
            s.dropEnabled = false;
            s.onUnassign.AddListener(CardUnAssigned);
        }
    }
    private void OnDisable()
    {
        foreach (UIItemSlot s in deckDisplay.slots)
        {
            s.dragEnabled = false;
            s.dropEnabled = true;
            s.onUnassign.RemoveListener(CardUnAssigned);
        }
    }
    protected override void CardAssigned(UIItemSlot slot)
    {
        if (slot.GetItemInfo() != null)
        {
            PlayerInfos.Instance.unitsWindow.unit.persistentDeck.RemoveCard(slot.GetItemInfo());
            PlayerInfos.Instance.unitsWindow.unit.persistentDeck.RemoveCardSlot();
            slot.Unassign();
            slot.SetIcon(destroyedIcon);
            slot.dragAndDropEnabled = false;
        }
    }

    protected override void CardUnAssigned(UIItemSlot slot)
    {
        slot.SetIcon(destroyedIcon);
        slot.dragAndDropEnabled = false;
    }
}