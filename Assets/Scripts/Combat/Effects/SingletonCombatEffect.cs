using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SingletonCombatEffect : AbstractCombatEffect
{
    public enum SINGLETONCOMBATEFFECT { dealdamage, heal};
    public SINGLETONCOMBATEFFECT effect;
    public override void Perform()
    {
        throw new NotImplementedException();
    }
}