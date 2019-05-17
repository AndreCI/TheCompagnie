using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class Leveling
{
    public int currentXP;
    public int nextLevelThreshold;
    public int currentLevel;
    private List<(CardDatabase.RARITY, float)> rarityDistribution;
    private Unit source;

    public Leveling(Unit source_, int level = 0)
    {
        source = source_;
        currentLevel = level;
        currentXP = 0;
        if (level == 0) { nextLevelThreshold = 5; }
        else
        {
            nextLevelThreshold = 10;
        }
        rarityDistribution = new List<(CardDatabase.RARITY, float)>();
        foreach(CardDatabase.RARITY r in Enum.GetValues(typeof(CardDatabase.RARITY)))
        {
            rarityDistribution.Add((r, 0f));
        }
    }

    public void GainXP(int xp)
    {
        currentXP += xp;
        while(currentXP >= nextLevelThreshold)
        {
            currentXP -= nextLevelThreshold;
            currentLevel += 1;
            //talentPoints += 1;
            nextLevelThreshold = 10;
            if(currentLevel == 1)
            {
                source.maxMana += 20;
                source.maxHealth += 20;
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
            }
            else if(currentLevel == 2)
            {
                TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.ACTIONPOINT);
                source.maxAction += 1;
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
            }else if(currentLevel == 3)
            {
                source.maxMana += 20;
                source.maxHealth += 20;
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
            }
            else if (currentLevel == 4)
            {
                source.maxAction += 1;
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
            }
            else
            {
                source.maxMana += 2;
                source.maxHealth += 5;
                nextLevelThreshold = 8;
            }
            //source.currentHealth = source.maxHealth;
           // source.CurrentMana = source.maxMana;
        }
    }

    
}