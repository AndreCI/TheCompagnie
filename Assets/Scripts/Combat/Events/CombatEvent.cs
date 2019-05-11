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

    private int currentCasterHealth;

    public CombatEvent(Unit source_, List<Unit> targets_, int timeIndex_, List<CombatEffect> effects_, Card cardSource_, bool channel_)
    {
        timeIndex = timeIndex_;
        cardSource = cardSource_;
        source = source_;
        targets = targets_;
        effects = effects_;
        channel = channel_;
        currentCasterHealth = source_.currentHealth;
        if (channel)
        {
            source_.NotifyUpdate += Remove;
        }
    }

    public void PerformEffect(float timePerEvent)
    {
        intent.Trigger(GetTime(timePerEvent));
        foreach (Unit target in targets)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Perform(target, source, forcedTime:GetTime(timePerEvent));
                if (channel)
                {
                    source.NotifyUpdate -= Remove;
                }
            }
        }
    }

    public void Remove()
    {
        if(intent == null)
        {
            source.NotifyUpdate -= Remove;
            return;
        }
        if(channel && currentCasterHealth > source.currentHealth && timeIndex >= TurnManager.Instance.currentIndex)
        {
            source.NotifyUpdate -= Remove;
            source.CurrentMana += cardSource.manaCost;

            TurnManager.Instance.RemoveCombatEvent(this);
        }
    }


    public float GetTime(float timePerEvent)
    {
        return channel ? 0.5f * timePerEvent : timePerEvent;
    }
}