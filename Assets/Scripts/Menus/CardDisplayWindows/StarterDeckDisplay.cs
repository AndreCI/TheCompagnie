using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StarterDeckDisplay : CardDIsplayWindow
{
    public CardUI cardDisplay;
    public CardDatabase compagnionCards;
    public CustomToggleGroup starterChoice;
    public CustomToggleGroup deckChoice;
    public List<Text> currentName;
    public List<Sprite> deckImages;
    private Dictionary<CardDatabase.CARDCLASS, List<bool>> unlocked;

    [HideInInspector] public CardDatabase.CARDCLASS currentClass;
    [HideInInspector] public CardDatabase.SUBCARDCLASS currentDeck;

    private static StarterDeckDisplay instance;
    public static StarterDeckDisplay Instance { get => instance; }

    private void Start()
    {
        instance = this;
        base.ProtectedStart();

        foreach (DuloGames.UI.UIItemSlot slot in slots)
        {
            slot.dragAndDropEnabled = false;
            slot.swapEnabled = false;
            slot.allowThrowAway = false;
            slot.glowingImage.CrossFadeAlpha(0, 0, true);

        }
        foreach (Toggle t in starterChoice.toggles.Keys)
        {
            t.onValueChanged.AddListener(delegate { OnStarterChange(t); }) ;
        }
        foreach (Toggle t in deckChoice.toggles.Keys)
        {
            t.onValueChanged.AddListener(delegate { OnDeckChange(t); });
        }
        compagnionCards.Setup();
        unlocked = new Dictionary<CardDatabase.CARDCLASS, List<bool>>();
       
        unlocked.Add(CardDatabase.CARDCLASS.PALADIN, new List<bool>( PlayerSettings.Instance.unlockedClasses[0]));

        unlocked.Add(CardDatabase.CARDCLASS.ELEM, new List<bool>(PlayerSettings.Instance.unlockedClasses[1]));
        unlocked.Add(CardDatabase.CARDCLASS.HUNTER, new List<bool>(PlayerSettings.Instance.unlockedClasses[2]));
        
        

        currentClass = CardDatabase.CARDCLASS.PALADIN;
        currentDeck = CardDatabase.SUBCARDCLASS.TYPE1;
        SetStatus();

    }

    public void ShowCardHolder(Card card)
    {
        cardDisplay.gameObject.SetActive(true);
        Color cardColor = Color.red;
        if(card.subClass == CardDatabase.SUBCARDCLASS.TYPE1)
        {
            cardColor = Compagnion.GetCurrentColor(0,0, Compagnion.GetBranch1Color(card.cardClass), Compagnion.GetBranch2Color(card.cardClass));
        }else if(card.subClass == CardDatabase.SUBCARDCLASS.TYPE2)
        {
            cardColor = Compagnion.GetBranch1Color(card.cardClass);
        }
        else if (card.subClass == CardDatabase.SUBCARDCLASS.TYPE3)
        {
            cardColor = Compagnion.GetBranch2Color(card.cardClass);
        }
        cardDisplay.Setup(card, cardColor, Color.white);
    }

    public void HideCardHolder()
    {
        cardDisplay.gameObject.SetActive(false);
    }

    private void SetStatus()
    {
        List<Card> s = compagnionCards.GetCardsFromClass(currentClass, CardDatabase.RARITY.STARTER, subclass: currentDeck).ToList();
        Setup(s);
        int index = 0;
        switch (currentClass)
        {
            case CardDatabase.CARDCLASS.PALADIN:
                index = 1;
                break;
            case CardDatabase.CARDCLASS.ELEM:
                index = 4;
                break;
            case CardDatabase.CARDCLASS.HUNTER:
                index = 7;
                break;
        }
        foreach (Toggle t in deckChoice.toggles.Keys)
        {
            t.GetComponentsInChildren<Image>().First(x => x.name == "Avatar").sprite = deckImages[index];
            index++;
        }
        int i = 0;
        foreach (Toggle t in starterChoice.toggles.Keys)
        {

            CardDatabase.CARDCLASS temp;
            if (i == 0)
            {
                temp = CardDatabase.CARDCLASS.PALADIN;
            }
            else if (i == 1)
            {
                temp = CardDatabase.CARDCLASS.ELEM;
            }
            else { temp = CardDatabase.CARDCLASS.HUNTER; }
            t.interactable = unlocked[temp][0];
            if (!unlocked[temp][0])
            {
                t.GetComponentsInChildren<Image>().First(x => x.name == "Avatar").sprite = deckImages[0];

            }
            i++;
        }
        i = 0;
        foreach (Toggle t in new List<Toggle>(deckChoice.toggles.Keys))
        {
            t.interactable = unlocked[currentClass][i];
            if (!unlocked[currentClass][i])
            {
                t.GetComponentsInChildren<Image>().First(x => x.name == "Avatar").sprite = deckImages[0];
                if (deckChoice.toggles[t])
                {
                    deckChoice.SetBaseToggle();
                }
            }
            i++;
        }
       


    }

    void OnDeckChange(Toggle current)
    {
        switch (deckChoice.GetCurrentIndex())
        {
            case 0:
                currentDeck = CardDatabase.SUBCARDCLASS.TYPE1;
                break;
            case 1:
                currentDeck = CardDatabase.SUBCARDCLASS.TYPE2;
                break;
            case 2:
                currentDeck = CardDatabase.SUBCARDCLASS.TYPE3;
                break;
        }
        SetStatus();
    }
    void OnStarterChange(Toggle current)
    {
        switch (starterChoice.GetCurrentIndex())
        {
            case 0:
                currentClass = CardDatabase.CARDCLASS.PALADIN;
                break;
            case 1:
                currentClass = CardDatabase.CARDCLASS.ELEM;
                break;
            case 2:
                currentClass = CardDatabase.CARDCLASS.HUNTER;
                break;
        }
        SetStatus();
    }



    private void Setup(List<Card> cards)
    {
        base.Clear();
        for (int i = 0; i < slots.Count; i++)
        {
            // slots[i].onUnassign.AddListener(CardUnAssigned);
            if (cards.Count == 4)
            {
                if (i < 4)
                {
                    slots[i].Assign(cards[0]);
                }
                else if (i < 7)
                {
                    slots[i].Assign(cards[1]);
                }
                else if (i < 9)
                {
                    slots[i].Assign(cards[2]);
                }
                else
                {
                    slots[i].Assign(cards[3]);

                }
            }
        }
    }



    protected override void CardAssigned(DuloGames.UI.UIItemSlot slot)
    {
        
    }
    protected override void CardUnAssigned(DuloGames.UI.UIItemSlot slot)
    {
        
    }

}