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
    public bool channel;

    public Intent intent;

    private bool cancelChannel = false;
    public CombatEvent nextChannelEvent;

    public CombatEvent(Unit source_, List<Unit> targets_, int timeIndex_, List<CombatEffect> effects_, Card cardSource_, bool channel_, CombatEvent nextChannelEvent_ = null)
    {
        timeIndex = timeIndex_;
        cardSource = cardSource_;
        source = source_;
        targets = targets_;
        effects = effects_;
        channel = channel_;
        if (channel)
        {
            nextChannelEvent = nextChannelEvent_;
            source_.SpecificUpdate += SourceUnit_Update;
        }
        foreach(CombatEffect e in effects)
        {
            if (e.OnPlay)
            {
                PerformEffect(e, TurnManager.Instance.timePerEvent);
            }
        }
    }


    private void PerformEffect(CombatEffect effect, float timePerEvent)
    {
        foreach (Unit target in targets)
        {

            effect.Perform(target, source, forcedTime: GetTime(timePerEvent));
            if (channel)
            {
                source.SpecificUpdate -= SourceUnit_Update;
            }
        }
    }

    public void PerformEffect(float timePerEvent)
    {
        intent.Trigger(GetTime(timePerEvent));
        foreach (Unit target in targets)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (!effects[i].OnPlay)
                {
                    effects[i].Perform(target, source, forcedTime: GetTime(timePerEvent));
                    if (channel)
                    {
                        source.SpecificUpdate -= SourceUnit_Update;
                    }
                }
            }
        }
    }

    private void SourceUnit_Update(Unit.UNIT_SPECIFIC_TRIGGER trigger)
    {
        if (channel)
        {
            if (trigger == Unit.UNIT_SPECIFIC_TRIGGER.ATTACKED)
            {
                cancelChannel = true;
            }
            else if (trigger == Unit.UNIT_SPECIFIC_TRIGGER.DAMAGE_INSTANCE_END)
            {
                cancelChannel = false;
            }
            else if (trigger == Unit.UNIT_SPECIFIC_TRIGGER.DAMAGE_DEALT && cancelChannel)
            {
                Remove();
            }
        }
        
    }
    private void Remove()
    {
        if (intent == null)
        {
            source.SpecificUpdate -= SourceUnit_Update;
            return;
        }
        if (channel)
        {
            source.SpecificUpdate -= SourceUnit_Update;
            source.CurrentMana += cardSource.manaCost;

            TurnManager.Instance.RemoveCombatEvent(this);
        }
    }


    public float GetTime(float timePerEvent)
    {
        return channel ? 0.5f * timePerEvent : timePerEvent;
    }
}