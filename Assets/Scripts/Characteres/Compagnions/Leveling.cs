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
        if (level == 0) { nextLevelThreshold = 5;
        }
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
            nextLevelThreshold = 10 + currentLevel;
            if(currentLevel == 1)
            {
                (source as Compagnion).talentTree.talentPoint += 1;
                //source.maxMana += 10;
                //source.maxHealth += 10;
                //source.CurrentHealth += 10;
                //source.CurrentMana += 10;
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
            }
            else if(currentLevel == 2)
            {
                (source as Compagnion).talentTree.talentPoint += 1;
                source.CurrentVoidPoints += 2;
               // source.maxMana += 5;
               // source.CurrentMana += 5;
               // source.maxHealth += 5;
               // source.CurrentHealth += 5;
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
            }else if(currentLevel == 3)
            {
                (source as Compagnion).talentTree.talentPoint += 1;
             //   source.maxMana += 10;
             //   source.CurrentMana += 10;
              //  source.maxHealth += 10;
             //   source.CurrentHealth += 10;
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
            }
            else if (currentLevel == 4)
            {
                source.CurrentVoidPoints += 3;
                (source as Compagnion).talentTree.talentPoint += 1;
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
            }
            else
            {
             //   source.maxMana += 2;
             //   source.maxHealth += 5;
                nextLevelThreshold = currentLevel * 3 ;
              //  source.CurrentHealth += 10;
              //  source.CurrentMana += 10;
                (source as Compagnion).talentTree.talentPoint += 1;
                source.persistentDeck.AddCardSlot();
                source.persistentDeck.AddCardSlot();
                source.CurrentVoidPoints += 2;

            }
        }
    }

    
}