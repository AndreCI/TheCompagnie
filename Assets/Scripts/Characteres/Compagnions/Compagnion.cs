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
    public override Unit Setup()
    {
        level = new Leveling(this);
        Unit copy = base.Setup();
        List<Card> cards = new List<Card>();
        int index = 0;
        foreach (Card card in PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, CardDatabase.RARITY.STARTER))
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
        copy.id = Unit.currentId++;
        copy.CurrentVoidPoints = 0;
        (copy as Compagnion).talentTree = PlayerInfos.Instance.talentTreeDatabase.Get(availableCards).Generate(copy as Compagnion);
        return copy;
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
    public static Color MixColor(Color a, Color b, float weightA, float weightB)
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
            else
            {
                cardNumber += 1;
            }
        }
        return cards;
    }

    public List<Card> DiscoverCard(int cardNumber = 3)
    {
        if(availableCards == CardDatabase.CARDCLASS.ELEM || availableCards == CardDatabase.CARDCLASS.PALADIN) { return DiscoverCard2(cardNumber); }
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
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.COMBATATTACKED);

        base.TakeDamage(amount, type, source);
        if(CurrentHealth <= 0)
        {
            PlayerInfos.Instance.gameOver.SetActive(true);
        }
    }
}
