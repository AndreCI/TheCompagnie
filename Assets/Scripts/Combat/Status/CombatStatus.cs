using System;
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
    public GeneralUtils.SUBJECT_TRIGGER trigger;
    public Unit target;
    public CombatStatusUI ui;
    public StatusAnimator animator;

    public CombatStatus(STATUS status_, int value_, int duration_, GeneralUtils.SUBJECT_TRIGGER trigger_, Unit target_)
    {
        if (!CheckStatus(status_, target_, duration_, value_, trigger_)) { return; }
        status = status_;
        value = value_;
        duration = duration_;
        trigger = trigger_;
        target = target_;
        animator = CombatManager.Instance.GetUnitUI(target).statusAnimator;
        TurnManager.NotifyAll += Notified;
        target.AddStatus(this);
       
    }

    


    

    public void Notified(GeneralUtils.SUBJECT_TRIGGER currentTrigger)
    {
        if (currentTrigger == trigger)
        {
            Apply();
            duration -= 1;
            CheckUpdate();
            if(ui != null)
            {
                ui.UpdateData();
                ui.Trigger(TurnManager.Instance.currentDuration);
            }
        }
    }
    private bool CheckStatus(STATUS status, Unit target, int duration, int value, GeneralUtils.SUBJECT_TRIGGER trigger)
    {
        switch (status)
        {
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
            case STATUS.BUFF_STR:
                foreach(CombatStatus s in target.CurrentStatus)
                {
                    if(s.status == status && s.trigger == GeneralUtils.SUBJECT_TRIGGER.PERMANENT && trigger == GeneralUtils.SUBJECT_TRIGGER.PERMANENT)
                    {
                        s.value += value;
                        s.ui.UpdateData();
                        return false;
                    }
                }
                break;
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
        if(value <= 0 || (duration <= 0 && trigger != GeneralUtils.SUBJECT_TRIGGER.PERMANENT))
        {
            if(status == STATUS.POISON && animator != null) { animator.ResetSpecific(status); }
            target.RemoveStatus(this);
            TurnManager.NotifyAll -= Notified;
            ui.Trigger(1f);
            ui.setDestroy = true;
        }
    }

}
