using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class Compagnion : Unit
{
    public List<int> startingWeights;
    [HideInInspector] public TalentTree talentTree;
    public Color branch1Color;
    public Color branch2Color;
    [HideInInspector] CardDatabase.SUBCARDCLASS starterType;

    public Unit Setup(CardDatabase.SUBCARDCLASS type)
    {
        starterType = type;
        return Setup();
    }

    public override Unit Setup()
    {
        if(starterType == CardDatabase.SUBCARDCLASS.GLOBAL) { starterType = CardDatabase.SUBCARDCLASS.NONE; }
        level = new Leveling(this);
        Unit copy = base.Setup();
        List<Card> cards = new List<Card>();
        int index = 0;
        foreach (Card card in PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, CardDatabase.RARITY.STARTER,subclass:starterType))
        {
            int i = startingWeights[index];
            index += 1;
            for (int j = 0; j < i; j++)
            {
                cards.Add(new Card(copy, card));
            }
        }
        copy.persistentDeck = new PersistentUnitDeck(cards);
        copy.persistentDeck.AddCardSlot();
        copy.persistentDeck.AddCardSlot();
        copy.persistentDeck.AddCardSlot();
        copy.id = id;// Unit.currentId++;
        copy.CurrentVoidPoints = 0;
        (copy as Compagnion).talentTree = PlayerInfos.Instance.talentTreeDatabase.Get(availableCards).Generate(copy as Compagnion);
        return copy;
    }

    public static Color GetBranch1Color(CardDatabase.CARDCLASS compClass)
    {
        switch (compClass)
        {
            case CardDatabase.CARDCLASS.PALADIN:
                
                return new Color(1f, 1f, 0f, 1f);
            case CardDatabase.CARDCLASS.ELEM:
                return new Color(1f, 0f, 0.1f, 1f);
            case CardDatabase.CARDCLASS.HUNTER:
                return new Color(0f, 1f, 0.1f, 1f);
        }
        return Color.white;
    }
    public static Color GetBranch2Color(CardDatabase.CARDCLASS compClass)
    {
        switch (compClass)
        {
            case CardDatabase.CARDCLASS.PALADIN:

                return new Color(1f, 0.1f, 0f, 1f);
            case CardDatabase.CARDCLASS.ELEM:
                return new Color(0.1f, 0f, 1f, 1f);
            case CardDatabase.CARDCLASS.HUNTER:
                return new Color(0f, 0.1f, 0.8f, 1f);
        }
        return Color.white;
    }


    public static Color GetCurrentColor(int v1, int v2, Color c1, Color c2)
    {
        float b1 = Mathf.Exp(v1);
        float b2 = Mathf.Exp(v2);
        float div = b1 + b2;
        b1 /= div;
        b2 /= div;
        return MixColor(c1, c2, b1, b2);
    }

    public override Color GetCurrentColor()
    {
        float b1 = Mathf.Exp(talentTree.branch1Value);
        float b2 = Mathf.Exp(talentTree.branch2Value);
        float div = b1 + b2;
        b1 /= div;
        b2 /= div;
        return MixColor(branch1Color, branch2Color, b1, b2);
    }
    public static Color MixColor(Color a, Color b, float weightA = 0.5f, float weightB = 0.5f)
    {
        Vector3 value = GetColorVector(a) * weightA + GetColorVector(b) * weightB; //GetColorVector(baseColor) * b0 +
        Color current = Color.HSVToRGB(value.x, value.y, value.z); // new Color(value.x, value.y, value.z);
        return current;
    }

    public static Vector3 GetColorVector(Color c)
    {
        float H, S, V;
        Color.RGBToHSV(c, out H, out S, out V);
        return new Vector3(H, S, V);
    }

    public List<Card> DiscoverCard2(int cardNumber = 3)
    {
        List<(CardDatabase.BRANCH, CardDatabase.RARITY)> types = new List<(CardDatabase.BRANCH, CardDatabase.RARITY)>();
        List<Card> cards = new List<Card>();
        for(int i = 0; i < cardNumber; i++)
        {
            CardDatabase.RARITY oldRarity = GetRarity();
            List<Card> currents = PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, oldRarity, GetBranch()).
                 Except(cards).
                 OrderBy(x => Utils.rdx.Next()).ToList();
            if (currents.Count > 0)
            {
                cards.Add(currents.First());
            }
            else if(cardNumber < 100)
            {

                cardNumber += 1;
            }
        }
        return cards;
    }

    public List<Card> DiscoverCard(int cardNumber = 3)
    {
        if(availableCards == CardDatabase.CARDCLASS.ELEM || availableCards == CardDatabase.CARDCLASS.PALADIN || availableCards == CardDatabase.CARDCLASS.HUNTER)
        { return DiscoverCard2(cardNumber); }
        List<Card> cards = new List<Card>(PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, CardDatabase.RARITY.NONE));
        cards = (PlayerInfos.Instance.cardDatabase.GetRandomCards(cardNumber,
            cards));

        return cards;
    }

    public CardDatabase.RARITY GetRarity()
    {

        int v = Utils.rdx.Next(0, 100);
        if (v > 35)
        {
            return CardDatabase.RARITY.COMMON;
        }else if (v > 9)
        {
            return CardDatabase.RARITY.RARE;
        }
        else
        {
            return CardDatabase.RARITY.EPIC;
        }
    }

    public CardDatabase.BRANCH GetBranch()
    {
        List<int> perc = new List<int> { 33, 33, 33 };

        float b0 = Mathf.Exp(1);
        float b1 = Mathf.Exp(talentTree.branch1Value);
        float b2 = Mathf.Exp(talentTree.branch2Value);
        float div = b0 + b1 + b2;
        float v = ((float)Utils.rdx.Next(0, 100)/100f);
        if (v < b0 / div)
        {
            return CardDatabase.BRANCH.BASIC;
        }else if(v < b0/div + b1 / div)
        {
            return CardDatabase.BRANCH.B1;
        }else if(v < b0/div + b1/div + b2 / div)
        {
            return CardDatabase.BRANCH.B2;
        }
        return CardDatabase.BRANCH.NONE;
    }

    public override void TakeDamage(int amount, Unit.DAMAGE_SOURCE_TYPE type, Unit source=null)
    {
        if (type == DAMAGE_SOURCE_TYPE.ATTACK)
        {
            if (!TurnManager.Instance.paused && !TutorialManager.Instance.status[TutorialManager.TUTOTRIGGER.COMBATATTACKED]&&
               !PlayerSettings.Instance.disableTutorial ) { TurnManager.Instance.TogglePosed(); }
            TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.COMBATATTACKED);
        }

        base.TakeDamage(amount, type, source);
        if(CurrentHealth <= 0)
        {
            PlayerSettings.Instance.Unlock(
                CardDatabase.CARDCLASS.PALADIN, Utils.rdx.Next() > 0.5f ? CardDatabase.SUBCARDCLASS.TYPE2 : CardDatabase.SUBCARDCLASS.TYPE3);
            PlayerInfos.Instance.gameOver.SetActive(true);
        }
    }
}
