using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TalentTree
{
    public List<Talent> branch1;
    public List<Talent> branch2;
    public int talentPoint;
    public int branch1Value;
    public int branch2Value;

    public TalentTree(List<Talent> branch1_, List<Talent> branch2_)
    {
        branch1 = new List<Talent>(branch1_);
        branch2 = new List<Talent>(branch2_);
        talentPoint = 10;
        branch1Value = 0;
        branch2Value = 0;
    }
    public List<Talent> GetActivatedTalents()
    {
        List<Talent> activated = new List<Talent>();
        activated.AddRange(branch1.FindAll(x => x.state == Talent.STATE.UNLOCKED));
        activated.AddRange(branch2.FindAll(x => x.state == Talent.STATE.UNLOCKED));
        return activated; 
    }

    public Talent Get(int index, int branch)
    {
        if (branch == 1) { return branch1[index]; }
        return branch2[index];
    }

    public void StartCombat() {
        foreach(Talent t in branch2)
        {
            t.StartCombat();
        }
        foreach (Talent t in branch1)
        {
            t.StartCombat();
        }
    }

    public void EndCombat()
    {
        foreach(Talent t in branch1)
        {
            t.EndCombat();
        }
        foreach (Talent t in branch2)
        {
            t.EndCombat();
        }
    }
}