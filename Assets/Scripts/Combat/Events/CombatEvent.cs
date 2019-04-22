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
    public List<Unit> targets;
    public List<CombatEffect> effects;


    public CombatEvent(Unit source_, List<Unit> targets_, int timeIndex_, List<CombatEffect> effects_, Card cardSource_)
    {
        timeIndex = timeIndex_;
        cardSource = cardSource_;
        source = source_;
        targets = targets_;
        effects = effects_;
        if(targets.Count != effects.Count)
        {
            Debug.Log("Target length and effect lengths are not equal!!");
        }
    }

    public void PerformEffect()
    {
        for(int i =0; i<targets.Count; i++)
        {
            effects[i].Perform();
        }
    }
}