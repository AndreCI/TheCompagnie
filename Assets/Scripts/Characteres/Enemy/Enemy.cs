using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class Enemy : Unit
{
    public enum ENEMY_TYPE { NONE, BEAST, UNDEAD};
    public ENEMY_TYPE type;
    public int xpValue;
    public List<int> cardsWeights;

    public List<CombatEffect> applyOnStart;
    public List<CombatEffect> applyOnLevel;
    public override Unit Setup()
    {
        int deckSizeFactor = 3;
        Unit copy = base.Setup();
        copy.level = GeneralUtils.Copy<Leveling>(level);
        List<Card> cards = new List<Card>();
        int index = 0;
        foreach(Card card in PlayerInfos.Instance.cardDatabase.GetCardsFromClass(availableCards))
        {
            int i = cardsWeights[index];
            for (int j = 0; j < i * deckSizeFactor; j++)
            {
                cards.Add(new Card(copy, card));
            }
            index++;
        }
        copy.persistentDeck = new PersistentUnitDeck(cards);
        foreach(CombatEffect e in applyOnStart)
        {
            e.Perform(copy, copy, 0f, 0f);
        }
        return copy;
    }
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if(currentHealth <= 0)
        {
            foreach(CombatStatus status in CurrentStatus)
            {
                TurnManager.NotifyAll -= status.Notified;
                GameObject.Destroy(status.ui.gameObject);
            }
            CombatManager.Instance.OnUnitDeath(this);
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

}
