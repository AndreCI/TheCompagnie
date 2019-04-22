using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDisplay : MonoBehaviour, CardHandler
{
    private List<Card> cards;
    private List<CardUI> cardUIs;
    public GridLayoutGroup group;
    public CardUI cardUI;

    private void Start()
    {
        cards = new List<Card>();        
    }

    public void Clear()
    {
        foreach(CardUI ui in cardUIs)
        {
            Destroy(ui.gameObject);
        }
    }

    public void DisplayDeck()
    {
        cards = CombatManager.Instance.compagnionDeck.GetCards();
        Display();
    }

    public void DisplayDiscard()
    {
        cards = CombatManager.Instance.compagnionDiscard.GetCards();
        Display();
    }

    private void Display()
    {
        cardUIs = new List<CardUI>();
        foreach(Card card in cards)
        {
            cardUIs.Add(CreateUI(card));
        }
    }

    private CardUI CreateUI(Card card)
    {
        CardUI UI = Instantiate(cardUI);
        UI.Setup(card);
        card.handler = this;
        UI.transform.SetParent(transform);
        UI.parentToReturnTo = transform;
        UI.placeholderParent = transform;
        UI.Playable = false;
        return UI;
    }
}
