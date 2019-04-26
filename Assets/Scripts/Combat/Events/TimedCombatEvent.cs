using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TimedCombatEvent : CombatEvent
{
    public int duration;
    public TimedCombatEvent(Unit source_, List<Unit> targets_, int timeIndex_, List<CombatEffect> effects_, Card cardSource_) : base(source_, targets_, timeIndex_, effects_, cardSource_)
    {
    }
}
