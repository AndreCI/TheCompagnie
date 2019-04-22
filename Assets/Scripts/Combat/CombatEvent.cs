using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CombatEvent
{
    public int timeIndex;
    public Card cardSource;
    public Unit source;
    public Unit target;
    public CombatEffect effect;

    public CombatEvent(Unit source_, Unit target_, int timeIndex_, CombatEffect effect_, Card cardSource_)
    {
        timeIndex = timeIndex_;
        cardSource = cardSource_;
        source = source_;
        target = target_;
        effect = effect_;
    }

    public void PerformEffect()
    {
        effect.Perform(target);
    }
}