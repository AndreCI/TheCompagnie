using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class Leveling
{
    public int currentXP;
    public int currentLevel;
    public int nextLevelThreshold;
    public int talentPoints;

    public Leveling(int level = 0)
    {
        currentLevel = level;
        currentXP = 0;
        nextLevelThreshold = 10;
        talentPoints = 0;
    }

    public void GainXP(int xp)
    {
        currentXP += xp;
        while(currentXP >= nextLevelThreshold)
        {
            currentXP -= nextLevelThreshold;
            currentLevel += 1;
            talentPoints += 1;
        }
    }
}