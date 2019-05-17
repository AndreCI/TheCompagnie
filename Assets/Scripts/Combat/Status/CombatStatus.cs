﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CombatStatus
{
    public enum STATUS {
        REGEN,
        BLOCK,
        PARRY,
        BURN,
        BLEED,
        POISON,
        REDUCE_STR,
        BUFF_STR,
        ACTION_GAIN,
        CARD_DRAW

};

    public STATUS status;
    public int value;
    public int duration;
    public bool permanent = false;
    public GeneralUtils.SUBJECT_TRIGGER trigger;
    public Unit target;
    public CombatStatusUI ui;
    public StatusAnimator animator;

    public CombatStatus(STATUS status_, int value_, int duration_, bool permanent_, GeneralUtils.SUBJECT_TRIGGER trigger_, Unit target_)
    {
        if (!CheckStatus(status_, target_, duration_, value_, permanent_, trigger_)) { return; }
        status = status_;
        value = value_;
        duration = duration_;
        trigger = trigger_;
        target = target_;
        permanent = permanent_;
        animator = CombatManager.Instance.GetUnitUI(target)?.statusAnimator;
        TurnManager.NotifyAll += Notified;
        target.AddStatus(this);
       
    }

    


    

    public void Notified(GeneralUtils.SUBJECT_TRIGGER currentTrigger)
    {
        if (currentTrigger == trigger)
        {
            Apply();
            if (!permanent) { duration -= 1; }
            CheckUpdate();
            if(ui != null)
            {
                ui.UpdateData();
                ui.Trigger(TurnManager.Instance.currentDuration);
            }
        }
    }
    private bool CheckStatus(STATUS status, Unit target, int duration, int value, bool permenent, GeneralUtils.SUBJECT_TRIGGER trigger)
    {
        switch (status)
        {
            case STATUS.BLOCK:
                foreach(CombatStatus s in target.CurrentStatus)
                {
                    if(s.status==status && s.trigger == trigger && s.duration == duration)
                    {
                        s.value += value;
                        s.ui?.UpdateData();
                        return false;
                    }
                }break;
            case STATUS.BLEED:
                foreach(CombatStatus s in target.CurrentStatus)
                {
                    if(s.status == status && s.trigger == trigger)
                    {
                        s.duration = Mathf.Max(s.duration, duration);
                        s.value += value;
                        return false;
                    }
                }
                break;
            case STATUS.POISON:
                foreach (CombatStatus s in target.CurrentStatus)
                {
                    if (s.status == status && s.trigger == trigger)
                    {
                        s.duration = Mathf.Max(s.duration, duration);
                        s.value = Mathf.Max(s.value, value);
                        s.ui.UpdateData();
                        return false;
                    }
                }
                break;
            case STATUS.BURN:
                foreach(CombatStatus s in target.CurrentStatus)
                {
                    if(s.status == status && s.trigger == trigger && s.duration == duration)
                    {
                        s.value += value;
                        s.ui.UpdateData();
                        return false;
                    }
                }
                break;
            case STATUS.BUFF_STR:
                foreach(CombatStatus s in target.CurrentStatus)
                {
                    if(s.status == status && (s.duration == duration || (s.permanent && permanent)))
                    {
                        s.value += value;
                        s.ui?.UpdateData();
                        return false;
                    }else if(s.status == STATUS.REDUCE_STR && (s.duration == duration || (s.permanent && permanent)))
                    {
                        if(s.value >= value)
                        {
                            s.value -= value;
                            s.ui?.UpdateData();
                            return false;
                        }
                        else
                        {
                            value -= s.value;
                            s.value = 0;
                            if(s.ui != null)
                                s.ui.setDestroy = true;
                            s.ui?.Trigger();
                            
                        }
                    }
                }
                break;
            case STATUS.REDUCE_STR:
                foreach (CombatStatus s in target.CurrentStatus)
                {
                    if (s.status == status && (s.duration == duration || (s.permanent && permanent)))
                    {
                        s.value += value;
                        s.ui?.UpdateData();
                        return false;
                    }
                    else if (s.status == STATUS.BUFF_STR && (s.duration == duration || (s.permanent && permanent)))
                    {
                        if (s.value >= value)
                        {
                            s.value -= value;
                            s.ui?.UpdateData();
                            return false;
                        }
                        else
                        {
                            value -= s.value;
                            s.value = 0;
                            if (s.ui != null)
                                s.ui.setDestroy = true;
                            s.ui?.Trigger();

                        }
                    }
                }
                break;

            case STATUS.REGEN:
                foreach(CombatStatus s in target.CurrentStatus)
                {
                    if(s.status == status && (
                        (s.duration == duration) ||
                        (s.permanent && permanent)
                        ))
                    {
                        s.value += value;
                        s.ui?.UpdateData();
                        return false;
                    }
                }break;
        }
       
        return true;
    }

    private void Apply()
    {
        switch (status)
        {
            case STATUS.REGEN:
                target.Heal(value);
                break;
            case STATUS.POISON:
                target.TakeDamage(value);
                break;
            case STATUS.BURN:
                target.TakeDamage(value);
                break;
            case STATUS.ACTION_GAIN:
                target.GainAction(value);
                break;
            case STATUS.CARD_DRAW:
                CombatManager.Instance.compagnionDeck.Draw(target);
                break;
            case STATUS.BLEED:
                target.TakeDamage(value);
                value -= 1;
                break;

        }
        CheckUpdate();
    }

    public void CheckUpdate(bool forceAnimation = false)
    {
        ui.UpdateData();
        if (animator != null && (trigger == GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN || forceAnimation)) {
            animator.PlayAnimation(status);
        }
        if(value <= 0 || (duration <= 0 && !permanent))
        {
            if(status == STATUS.POISON && animator != null) { animator.ResetSpecific(status); }
            target.RemoveStatus(this);
            TurnManager.NotifyAll -= Notified;
            ui.Trigger(1f);
            ui.setDestroy = true;
        }
    }

    public string GetDescription()
    {
        bool setFinalDamage = false;
        string description = "No tool tip written yet...\n";
        string timeMoment = "";
        switch (trigger)
        {
            case GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN:
                timeMoment= " turn";
                break;
            case GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK:
                timeMoment = " tick";
                break;
        }


        switch (status)
        {
            case STATUS.BLOCK:
                description = "Block: Prevent the next " + value.ToString() + " damages";
                break;
            case STATUS.BURN:
                description = "Burn: Take " + value.ToString() + " damages each" + timeMoment;
                setFinalDamage = true;
                break;
            case STATUS.POISON:
                description = "<i> Mixing poisons can bring death quickly... </i>\n";
                description += "Poison: Take " + value.ToString() + " damages each" + timeMoment;
                setFinalDamage = true;
                break;
            case STATUS.REGEN:
                description = "Regeneration: Gain " + value.ToString() + " health points each" + timeMoment;
                setFinalDamage = true;
                break;
            case STATUS.BUFF_STR:
                description = "Strength: Each instance of damage dealt by an attacks deals " + value.ToString() + " additional damages";
                break;
            case STATUS.REDUCE_STR:
                description = "Strength: Each instance of damage dealt by an attacks deals " + value.ToString() + " less damages";
                break;
            case STATUS.PARRY:
                description = "Parry: The next " + (value > 1 ? value.ToString() + " " : "") + "instance" + (value > 1 ? "s" : "") + "of damage from any attack going through block is prevented";
                break;
        }
        if (permanent)
        {
            description += " until end of combat";
        }
        else
        {
            description += " for " + duration.ToString() + timeMoment + (duration>1?"s":"");
            if (setFinalDamage)
            {
                description += " (" + (duration * value).ToString() + ")";
            }
        }
        
        return description;
    }

}
