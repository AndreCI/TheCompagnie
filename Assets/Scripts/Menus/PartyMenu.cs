using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class PartyMenu : MonoBehaviour
{
    public Unit unit;
    public UnitPortrait portrait;
    public GameObject tab1;
    public GameObject tab2;
    public GameObject tab3;

    public List<List<DuloGames.UI.UIItemSlot>> slots;
    public List<Card> cards;
    public CardUI cardHolder;

    public void Start()
    {
    }

    public void SetInfos(IEnumerable<Unit> units = null, List<Card> cards = null)
    {
        Clear();
        if(units == null || units.Count() == 0) { units = PlayerInfos.Instance.compagnions; }
        unit = (new List<Unit>(units))[0];
        if (portrait != null)
        {
            portrait.Setup(unit);
        }
        if (cards == null) { cards = PlayerInfos.Instance.persistentPartyDeck.GetCards(units); }
        for (int i = 0; i < cards.Count; i++)
        {
            slots[0][i].Assign(cards[i]);
        }
     

        
    }
    private void OnDestroy()
    {
        //unit.NotifyUpdate -= UpdateInfo;
    }

    public void Clear()
    {
        slots = new List<List<DuloGames.UI.UIItemSlot>>();
        slots.Add(new List<DuloGames.UI.UIItemSlot>(tab1.GetComponentsInChildren<DuloGames.UI.UIItemSlot>()));
        slots.Add(new List<DuloGames.UI.UIItemSlot>(tab2.GetComponentsInChildren<DuloGames.UI.UIItemSlot>()));
        slots.Add(new List<DuloGames.UI.UIItemSlot>(tab3.GetComponentsInChildren<DuloGames.UI.UIItemSlot>()));
        foreach (List<DuloGames.UI.UIItemSlot> slotlist in slots)
        {
            foreach (DuloGames.UI.UIItemSlot slot in slotlist)
            {
                slot.Unassign();
            }
        }
    }

    private void OnEnable()
    {
        SetInfos(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT));
    }
    public void ShowCardHolder(Card card)
    {
        cardHolder.gameObject.SetActive(true);
        cardHolder.Setup(card);
    }
    public void HideCardHolder()
    {
        cardHolder.gameObject.SetActive(false);
    }
}