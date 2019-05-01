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
    public List<CombatEffect> effects;

    public Intent intent;


    public CombatEvent(Unit source_, Unit target_, int timeIndex_, List<CombatEffect> effects_, Card cardSource_)
    {
        timeIndex = timeIndex_;
        cardSource = cardSource_;
        source = source_;
        target = target_;
        effects = effects_;
    }

    public void PerformEffect()
    {
        for(int i =0; i<effects.Count; i++)
        {
            effects[i].Perform(target, source);
        }
    }
}