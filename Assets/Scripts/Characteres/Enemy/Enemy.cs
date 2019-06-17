using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

[Serializable]
public class Enemy : Unit
{
    public enum ENEMY_TYPE { NONE, BEAST, UNDEAD};
    public ENEMY_TYPE type;
   
    public int xpValue;
    public List<int> cardsWeights;
    public Color enemyColor;

    public List<CombatEffect> applyOnStart;
    public List<CombatEffect> applyOnLevel;

   

    public CardDatabase.SUBCARDCLASS subclass;
    public override Unit Setup()
    {
        int deckSizeFactor = 3;
        Unit copy = base.Setup();
        
        copy.level = GeneralUtils.Copy<Leveling>(level);
        List<Card> cards = GetCards(copy);// new List<Card>();
       /* int index = 0;
        foreach(Card card in PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards))
        {
            int i = cardsWeights[index];
            for (int j = 0; j < i * deckSizeFactor; j++)
            {
                cards.Add(new Card(copy, card));
            }
            index++;
        }*/

        copy.persistentDeck = new PersistentUnitDeck(cards);
        foreach(CombatEffect e in applyOnStart)
        {
            e.Perform(new List<Unit> { copy }, copy, null, 0f, 0f);
        }
        (copy as Enemy).enemyColor = enemyColor;
        return copy;
    }

    private List<Card> GetCards(Unit copy)
    {
        List<Card> cards = new List<Card>();
        List<Card> commons = new List<Card>();
        commons.AddRange(PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, CardDatabase.RARITY.COMMON, CardDatabase.BRANCH.NONE, CardDatabase.SUBCARDCLASS.GLOBAL));
        commons.AddRange(PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, CardDatabase.RARITY.COMMON, CardDatabase.BRANCH.NONE, subclass));
        List<Card> rares = new List<Card>();
        rares.AddRange(PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, CardDatabase.RARITY.RARE, CardDatabase.BRANCH.NONE, CardDatabase.SUBCARDCLASS.GLOBAL));
        rares.AddRange(PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards, CardDatabase.RARITY.RARE, CardDatabase.BRANCH.NONE, subclass));
        int weightsTotal = cardsWeights.Sum();
        rares = rares.OrderBy(x => Utils.rdx.Next()).ToList();
        commons = commons.OrderBy(x => Utils.rdx.Next()).ToList();
        for(int i = 0; i < cardsWeights.Count; i++)
        {
            float prob = Mathf.Max(0.35f - (float)cardsWeights[i] / weightsTotal, 0);
            Card current;
            if(((float)Utils.rdx.Next(100)/100f) < prob && rares.Count > 0 || commons.Count == 0)
            {
                current = rares[0];
                rares.RemoveAt(0);
            }
            else
            {
                current = commons[0];
                commons.RemoveAt(0);
            }
            for (int j = 0; j < cardsWeights[i]; j++)
            {
                cards.Add(new Card(copy, current));
            }
            if(current.actionCost == 0)
            {
                i--;
            }
        }
        return cards;
    }

    public bool IsCurrentCardStatus(Card c)
    {
        return CurrentStatus.Any(x => x.miscData.Name == c.Name);
    }

    public override void TakeDamage(int amount, Unit.DAMAGE_SOURCE_TYPE type, Unit source = null)
    {
        base.TakeDamage(amount, type, source);
        if(CurrentHealth <= 0)
        {
            CheckDeath();
            CombatManager.Instance.OnUnitDeath(this);
        }
    }

    public void CheckDeath()
    {
        if (CurrentHealth <= 0)
        {
            
            foreach (CombatStatus status in new List<UnitStatus>(CurrentStatus)) //Avoid collection was modified!
            {
                status.CheckUpdate(forceRemove:true);
                //   TurnManager.NotifyAll -= status.Notified;
                //   if (status.ui != null)
                //   {
                //      GameObject.Destroy(status.ui.gameObject);
                //  }
            }
        }
        }
    public void AddLevel()
    {
        level.currentLevel += 1;
        maxHealth += Mathf.FloorToInt(maxHealth / 15f);
        maxMana += Mathf.FloorToInt(maxMana / 15f);
        foreach(CombatEffect ce in applyOnLevel)
        {
            applyOnStart.Add(GeneralUtils.Copy<CombatEffect>(ce));
        }
    }

    public override Color GetCurrentColor()
    {
        return enemyColor;
    }
}
