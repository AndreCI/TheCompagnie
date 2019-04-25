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

    public Scrollbar cardSelection;

    void Start()
    {
        
    }

    public void Clear()
    {
        if (cardUIs != null)
        {
            foreach (CardUI ui in cardUIs)
            {
                Destroy(ui.gameObject);
            }
        }
        cardUIs = new List<CardUI>();
        cards = new List<Card>();
    }

    public void DisplayAbstractDeck()
    {
        Clear();
        cards = PlayerInfos.Instance.compagnions[0].GetNewDeck().GetCards();
        Display();
    }

    public void DisplayDeck(CombatDeck deck)
    {
        Clear();
        cards = deck.GetCards();
        //cards = CombatManager.Instance.compagnionDeck.GetCards();
        Display();
    }

    public void DisplayLevelUpSelection()
    {
        Clear();
        group.childAlignment = TextAnchor.MiddleCenter;
        cards = PlayerInfos.Instance.collection.GetRandomCards(3);
        cardSelection.gameObject.SetActive(true);
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

    public void SelectionValidate()
    {
        PlayerInfos.Instance.compagnions[0].persistentDeck.AddCard( 
            new Card(PlayerInfos.Instance.compagnions[0], cards[(int)cardSelection.value * 2]),
            PlayerInfos.Instance.compagnions[0]);
        cardSelection.gameObject.SetActive(false);
    }
}
