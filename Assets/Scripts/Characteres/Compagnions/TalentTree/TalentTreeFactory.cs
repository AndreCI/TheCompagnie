using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class TalenTreeFactory
{
    public List<Talent> branch1;
    public List<Talent> branch2;
    public CardDatabase.CARDCLASS classType;

    public int ID;

    public void Setup()
    {

    }
    public TalentTree Generate(Compagnion owner)
    {
        foreach(Talent t in branch1)
        {
            t.state = Talent.STATE.LOCKED;
            t.Setup(owner);
        }
        foreach (Talent t in branch2)
        {
            t.state = Talent.STATE.LOCKED;
            t.Setup(owner);
        }
        List<Talent> nb1 = new List<Talent>();// { b1};
        List<Talent> nb2 = new List<Talent>();// { b2};
        List<Talent> actionTalents = new List<Talent>(
            branch1.FindAll(x => x.onUnlockEffect.Count > 0 && x.onUnlockEffect.Any(y => y == Talent.UNIT_MODIFIER.ACTIONPOINT)).OrderBy(z => Utils.rdx.Next())
            );
        nb1.Add(actionTalents[0]);
        nb1.AddRange(branch1.Except(actionTalents).OrderBy(x => Utils.rdx.Next()).Take(3));
        actionTalents = new List<Talent>(
             branch2.FindAll(x => x.onUnlockEffect.Count > 0 && x.onUnlockEffect.Any(y => y == Talent.UNIT_MODIFIER.ACTIONPOINT)).OrderBy(z => Utils.rdx.Next())
             );
        nb2.Add(actionTalents[0]);
        nb2.AddRange(branch2.Except(actionTalents).OrderBy(x => Utils.rdx.Next()).Take(3));

        nb1 = nb1.OrderBy(x => Utils.rdx.Next()).ToList();
        nb2 = nb2.OrderBy(x => Utils.rdx.Next()).ToList();
        nb1[0].state = Talent.STATE.AVAILABLE;
        nb2[0].state = Talent.STATE.AVAILABLE;
        return new TalentTree(nb1, nb2);
    }
}
