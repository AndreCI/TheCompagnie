using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardDIsplayWindow : MonoBehaviour
{

    public List<DuloGames.UI.UIItemSlot> slots;
    public GameObject tab;

    private void OnDestroy()
    {
        foreach (DuloGames.UI.UIItemSlot slot in slots)
        {
            slot.onUnassign.RemoveAllListeners();
            slot.onAssign.RemoveAllListeners();
        }
    }
    public void ProtectedStart()
    {
        slots = new List<DuloGames.UI.UIItemSlot>(tab.transform.GetComponentsInChildren<DuloGames.UI.UIItemSlot>(true));
        foreach (DuloGames.UI.UIItemSlot slot in slots)
        {
            slot.tooltipDelay = 0.2f;
            slot.tooltipEnabled = true;

        }
    }

    public virtual void Setup(List<Card> cards, int slotNumber = 0)
    {
        Clear();
        for (int i = 0; i < slots.Count; i++)
        {
            // slots[i].onUnassign.AddListener(CardUnAssigned);
            if (i < cards.Count)
            {
                slots[i].Assign(cards[i]);
            }
            else if (i < slotNumber && slotNumber > 0)
            {
                slots[i].Unassign();
                slots[i].dropEnabled = true;
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
    private void Clear()
    {
        if (slots == null || slots.Count == 0)
        {
            slots = new List<DuloGames.UI.UIItemSlot>(tab.transform.GetComponentsInChildren<DuloGames.UI.UIItemSlot>(true));

        }

        foreach (DuloGames.UI.UIItemSlot slot in slots)
        {
            slot.gameObject.SetActive(true);
            slot.Unassign();
            slot.gameObject.SetActive(true);
        }

    }

    protected abstract void CardAssigned(DuloGames.UI.UIItemSlot slot);
    protected abstract void CardUnAssigned(DuloGames.UI.UIItemSlot slot);
}