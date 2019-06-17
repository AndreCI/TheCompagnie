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
    public bool cancellable;

    public Intent intent;

    private bool cancelChannel = false;
    public CombatEvent nextChannelEvent;

    private AudioSound.AUDIO_SET audioSet;
    private string audioSound;
    public CombatEvent(Unit source_, List<Unit> targets_, int timeIndex_, List<CombatEffect> effects_, Card cardSource_, AudioSound.AUDIO_SET audioSet_, bool channel_, bool cancellable_, CombatEvent nextChannelEvent_ = null)
    {
        timeIndex = timeIndex_;
        cardSource = cardSource_;
        source = source_;
        targets = targets_;
        effects = effects_;
        channel = channel_;
        audioSet = audioSet_;
        cancellable = cancellable_;
        if (channel)
        {
            nextChannelEvent = nextChannelEvent_;
            
        }
        if (cancellable)
        {
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
        if(audioSound != null && audioSound != "") { AudioManager.Instance?.Play(audioSound); }
        else if(audioSet != AudioSound.AUDIO_SET.NONE) { AudioManager.Instance?.PlayFromSet(audioSet); }

            effect.Perform(targets, source, cardSource, forcedTime: GetTime(timePerEvent));
            if (cancellable)
            {
                source.SpecificUpdate -= SourceUnit_Update;
            }
        
    }

    public void PerformEffect(float timePerEvent)
    {
        if (audioSound != null && audioSound != "") { AudioManager.Instance?.Play(audioSound); }
        else if (audioSet != AudioSound.AUDIO_SET.NONE) { AudioManager.Instance?.PlayFromSet(audioSet); }
        intent.Trigger(GetTime(timePerEvent));
        for (int i = 0; i < effects.Count; i++)
        {
            if (!effects[i].OnPlay)
            {
                effects[i].Perform(targets, source, cardSource, forcedTime: GetTime(timePerEvent));
                if (channel)
                {
                    source.SpecificUpdate -= SourceUnit_Update;
                }
            }
        }
        source.TriggerSpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.ACT, source);

    }

    private void SourceUnit_Update(Unit.UNIT_SPECIFIC_TRIGGER trigger, Unit source)
    {
        if (cancellable)
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
                if (!TurnManager.Instance.paused && !TutorialManager.Instance.status[TutorialManager.TUTOTRIGGER.CANCEL_CHANNEL] && !PlayerSettings.Instance.disableTutorial) { TurnManager.Instance.TogglePosed(); }
                TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.CANCEL_CHANNEL);
                Remove();
            }
        }
        
    }
    public void Remove()
    {
        if (intent == null)
        {
            source.SpecificUpdate -= SourceUnit_Update;
            return;
        }
        if (cancellable)
        {
            source.SpecificUpdate -= SourceUnit_Update;
            source.CurrentMana += cardSource.manaCost;

        }
        TurnManager.Instance.RemoveCombatEvent(this);
    }


    public float GetTime(float timePerEvent)
    {
        return channel ? 0.5f * timePerEvent : timePerEvent;
    }
}