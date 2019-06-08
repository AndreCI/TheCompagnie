using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CombatStatus : UnitStatus
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
        CARD_DRAW,
        FROST,
        CHANNEL,
        STATUS_APPLY,
        BUFF_SPEED,
        RECUDE_SPEED

};

    public STATUS status;
    public GeneralUtils.SUBJECT_TRIGGER trigger;
    public Unit.UNIT_SPECIFIC_TRIGGER unitTrigger;
    public CombatStatusUI ui;
    [NonSerialized]  public List<CombatStatusFactory> factory;
    [NonSerialized] public List<CombatEffectFactory> effectFactory;
   [NonSerialized] public CombatStatusData miscData;

    public CombatStatus(STATUS status_, int value_, int duration_, bool permanent_, GeneralUtils.SUBJECT_TRIGGER trigger_, 
        Unit target_, Unit.UNIT_SPECIFIC_TRIGGER unitT,
        List<CombatStatusFactory> factory_, List<CombatEffectFactory> effectFactory_, CombatStatusData misc)
    {
        if (permanent_) { duration_ = -1; }
        value = value_;

        if (!CheckStatus(status_, target_, duration_, permanent_, trigger_)) { return; }
        status = status_;
        noValue = status_ == STATUS.FROST || status == STATUS.STATUS_APPLY;
        duration = duration_;
        trigger = trigger_;
        target = target_;
        unitTrigger = unitT;
        permanent = permanent_;
        effectFactory = effectFactory_;
        factory = factory_;
        showUi = true;
        miscData = misc;
        TurnManager.NotifyAll += Notified;
        target.SpecificUpdate += Target_SpecificUpdate;
        target.AddStatus(this);
    }

    private void Target_SpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER trigger, Unit source)
    {
        if (unitTrigger == trigger && trigger != Unit.UNIT_SPECIFIC_TRIGGER.NONE)
        {
            Apply(source);
            //CheckUpdate();
            if (ui != null)
            {
                ui.Trigger(TurnManager.Instance.currentDuration, true);
            }
        }
    }

    public override void Notified(GeneralUtils.SUBJECT_TRIGGER currentTrigger)
    {
        if (currentTrigger == trigger && trigger != GeneralUtils.SUBJECT_TRIGGER.NONE)
        {
            if (unitTrigger == Unit.UNIT_SPECIFIC_TRIGGER.NONE) { Apply(); }
            if (!permanent) { duration -= 1; }
            CheckUpdate();
            if(ui != null)
            {
                ui.UpdateData();
                ui.Trigger(TurnManager.Instance.currentDuration, currentTrigger != GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN);
            }
        }
    }
    private bool CheckStatus(STATUS status, Unit target, int duration, bool permenent, GeneralUtils.SUBJECT_TRIGGER trigger)
    {
        if(target == null) { return false; }
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
                        s.ui?.UpdateData();
                        return false;
                    }
                }
                break;
            case STATUS.BUFF_STR:
                foreach(CombatStatus s in target.CurrentStatus)
                {
                    if(s.status == status && ((s.duration == duration && !permenent) || (s.permanent && permenent)))
                    {
                        s.value += value;
                        s.ui?.UpdateData();
                        return false;
                    }else if(s.status == STATUS.REDUCE_STR && (s.duration == duration && !permenent || (s.permanent && permenent)))
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
                            if (s.ui != null)
                            {
                                s.ui.UpdateData();
                                s.ui.setDestroy = true;
                                s.ui.Trigger(TurnManager.Instance.currentDuration);
                            }
                        }
                    }
                }
                break;
            case STATUS.REDUCE_STR:
                foreach (CombatStatus s in target.CurrentStatus)
                {
                    if (s.status == status && (s.duration == duration && !permenent || (s.permanent && permenent)))
                    {
                        s.value += value;
                        s.ui?.UpdateData();
                        return false;
                    }
                    else if (s.status == STATUS.BUFF_STR && (s.duration == duration && !permenent || (s.permanent && permenent)))
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
                            if(s.ui != null) {
                                s.ui.UpdateData();
                                s.ui.setDestroy = true;
                                s.ui.Trigger(TurnManager.Instance.currentDuration);
                            }
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
            case STATUS.FROST:
                foreach (CombatStatus s in target.CurrentStatus)
                {
                    if (s.status == status && 
                        (s.permanent ||
                        (permanent && !s.permanent ||
                        !s.permanent && !permanent)
                        ))
                    {
                        s.duration += duration;
                        if (permenent)
                        {
                            s.permanent = true;
                        }
                        s.ui?.UpdateData();
                        return false;
                    }
                }
                break;
            case STATUS.CHANNEL:
                foreach (CombatStatus s in target.CurrentStatus)
                {
                    if (s.status == status && (
                        (s.duration == duration) ||
                        (s.permanent && permanent)
                        ))
                    {
                        s.value += value;
                        s.ui?.UpdateData();
                        return false;
                    }
                }
                break;
            case STATUS.BUFF_SPEED:
                foreach (CombatStatus s in target.CurrentStatus)
                {
                    if (s.status == status && ((s.duration == duration && !permenent) || (s.permanent && permenent)))
                    {
                        s.value += value;
                        s.ui?.UpdateData();
                        return false;
                    }
                    else if (s.status == STATUS.REDUCE_STR && (s.duration == duration && !permenent || (s.permanent && permenent)))
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
                            {
                                s.ui.UpdateData();
                                s.ui.setDestroy = true;
                                s.ui.Trigger(TurnManager.Instance.currentDuration);
                            }
                        }
                    }
                }
                break;
            case STATUS.RECUDE_SPEED:
                foreach (CombatStatus s in target.CurrentStatus)
                {
                    if (s.status == status && (s.duration == duration && !permenent || (s.permanent && permenent)))
                    {
                        s.value += value;
                        s.ui?.UpdateData();
                        return false;
                    }
                    else if (s.status == STATUS.BUFF_STR && (s.duration == duration && !permenent || (s.permanent && permenent)))
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
                            {
                                s.ui.UpdateData();
                                s.ui.setDestroy = true;
                                s.ui.Trigger(TurnManager.Instance.currentDuration);
                            }
                        }
                    }
                }
                break;

        }
       
        return true;
    }

    private void Apply(Unit source=null)
    {
        switch (status)
        {
            case STATUS.REGEN:
                target.Heal(value);
                break;
            case STATUS.POISON:
                target.TakeDamage(value, Unit.DAMAGE_SOURCE_TYPE.STATUS);
                break;
            case STATUS.BURN:
                target.TakeDamage(value, Unit.DAMAGE_SOURCE_TYPE.STATUS);
                break;
            case STATUS.ACTION_GAIN:
                target.GainAction(value);
                break;
            case STATUS.CARD_DRAW:
                CombatManager.Instance.compagnionDeck.Draw(target);
                break;
            case STATUS.BLEED:
                target.TakeDamage(value, Unit.DAMAGE_SOURCE_TYPE.STATUS);
                value -= 1;
                break;
            case STATUS.STATUS_APPLY:
                foreach(CombatStatusFactory f in factory) {                                                                                      
                    f.GenerateApply(f.alternative == CombatEffect.ALTERNATIVE_TARGET.SELF ? target : source, data:miscData);//  source != null ? source : target, originalCard);
                }
                foreach(CombatEffectFactory effect in effectFactory)
                {
                    CombatEffect e = effect.Generate();
                    e.Perform(new List<Unit> { source }, target, statusData:miscData);
                }
                break;

        }
     //   CheckUpdate();
    }

    public void CheckUpdate(bool forceAnimation = false, bool forceRemove=false)
    {
        ui?.UpdateData();
        if((value <= 0 && !noValue) || (duration <= 0 && !permanent)|| forceRemove)
        {
            target.RemoveStatus(this);
            TurnManager.NotifyAll -= Notified;
            target.SpecificUpdate -= Target_SpecificUpdate;
            ui.setDestroy = true;
            ui?.Trigger(TurnManager.Instance.currentDuration, forceAnimation:forceAnimation);
        }
        else if (forceAnimation)
        {
            ui?.Trigger(
                Mathf.Min(TurnManager.Instance.currentDuration,
                TurnManager.Instance.currentEventTimeDuration), forceAnimation: true);
        }
    }

    public override string GetDescription()
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
        string amountStr = value.ToString();
        switch (status)
        {
            case STATUS.BLOCK:
                description = "Block: Prevent the next " + amountStr + " damages";
                break;
            case STATUS.BURN:
                description = "Burn: Take " + amountStr + " damages each" + timeMoment;
                setFinalDamage = true;
                break;
            case STATUS.POISON:
                description = "<i> Mixing poisons can bring death quickly... </i>\n";
                description += "Poison: Take " + amountStr + " damages each" + timeMoment;
                description += "\n(Does not cancel channel)";
                setFinalDamage = true;
                break;
            case STATUS.REGEN:
                description = "Regeneration: Gain " + amountStr + " health points each" + timeMoment;
                setFinalDamage = true;
                break;
            case STATUS.BUFF_STR:
                description = "Strength: Each instance of damage dealt by an attacks deals " + amountStr + " additional damages";
                break;
            case STATUS.REDUCE_STR:
                description = "Strength: Each instance of damage dealt by an attacks deals " + amountStr + " less damages";
                break;
            case STATUS.PARRY:
                description = "Parry: The next " + (value > 1 ? amountStr + " " : "") + "instance" + (value > 1 ? "s" : "") + "of damage from any attack going through block is prevented";
                break;
            case STATUS.FROST:
                description = "Frost: Reduce damage done by attacks by 2 (applied after strength, rounded down)";
                break;
            case STATUS.CHANNEL:
                description = "Concentration: All cards with channel will trigger " + amountStr + (value > 0 ? " more" :" less")+ " time" + (value > 1 ? "s" : "");
                break;
            case STATUS.BUFF_SPEED:
                description = "Speed: Each card played takes " + amountStr + " less ticks (never less than 0)";
                break;
            case STATUS.RECUDE_SPEED:
                description = "Speed: Each card played takes " + amountStr + " additional ticks";
                break;

            case STATUS.STATUS_APPLY:
                return "<i>" + miscData.Name + "</i>\n" + GetApplyDescription(trigger, unitTrigger, permanent,
                    duration, value, status, effectFactory, factory, null);// CombatEffect.LinkPrefixAndTime(GetFactoryAppliedDescription(factory, effectFactory, unitTrigger));
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


    private static List<List<string>> GetFactoryAppliedDescription(List<CombatStatusFactory> factory, List<CombatEffectFactory> effectFactory, Unit.UNIT_SPECIFIC_TRIGGER unitTrigger)
    {
        List<string> prefix = new List<string>();
        List<string> bodies = new List<string>();
        List<string> time = new List<string>();
        if (factory != null && factory.Count > 0)
        {
            string oldSubject = "";
            string currentSubject = "";
            for (int i = 0; i < factory.Count; i++)
            {
                {
                    CombatStatusFactory f = factory[i];
                    currentSubject = GetCurrentSubject(unitTrigger, f.alternative, oldSubject);
                    List<string> c = CombatStatusFactoryFactory.GetDescription(f.status, f.value, f.duration, f.permanent, f.trigger, f.variable);
                    prefix.Add(currentSubject + c[0]);
                    bodies.Add(c[1]);
                    time.Add(c[2]);

                }
            }
        }
        return new List<List<string>> { prefix, bodies, time };
    }

    public static string GetCurrentSubject(Unit.UNIT_SPECIFIC_TRIGGER unitTrigger, CombatEffect.ALTERNATIVE_TARGET alternative, string oldSubject)
    {
        string currentSubject = "";
        if (unitTrigger != Unit.UNIT_SPECIFIC_TRIGGER.ATTACKS) { currentSubject = "the attacker "; }
        else { currentSubject = "its target "; }
        
        switch (alternative)
        {
            case CombatEffect.ALTERNATIVE_TARGET.SELF:
                currentSubject = unitTrigger == Unit.UNIT_SPECIFIC_TRIGGER.NONE ? "this unit " : "it ";
                break;
            case CombatEffect.ALTERNATIVE_TARGET.ALL:
                currentSubject = "all units ";
                break;
            case CombatEffect.ALTERNATIVE_TARGET.ALL_ENEMY:
                currentSubject = "all enemies ";
                break;
            case CombatEffect.ALTERNATIVE_TARGET.ALL_FRIEND:
                currentSubject = "all friends ";
                break;
        }

        return currentSubject;
}

    public static string GetApplyDescription(GeneralUtils.SUBJECT_TRIGGER trigger, Unit.UNIT_SPECIFIC_TRIGGER unitTrigger,
        bool permanent, int duration, int value, STATUS status, List<CombatEffectFactory> effectFactory,
        List<CombatStatusFactory> factory, List<CombatStatusFactoryFactory> factoryOfFactory)
    {
        string de = "";
        if (trigger != GeneralUtils.SUBJECT_TRIGGER.NONE && unitTrigger != Unit.UNIT_SPECIFIC_TRIGGER.NONE && !permanent)
        {
            de += "For " + duration.ToString();
            switch (trigger)
            {
                case GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN:
                    de += " turn" + (duration > 1 ? "s" : "") + ": ";
                    break;
                case GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK:
                    de += " tick" + (duration > 1 ? "s" : "") + ": ";
                    break;
            }

        }
        if (trigger != GeneralUtils.SUBJECT_TRIGGER.NONE && unitTrigger == Unit.UNIT_SPECIFIC_TRIGGER.NONE)
        {
            if (permanent) { de = "Each"; }
            else { de += "each"; }
            switch (trigger)
            {
                case GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN:
                    de += " turn ";
                    break;
                case GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK:
                    de += " tick ";
                    break;
            }
        }
        if (unitTrigger != Unit.UNIT_SPECIFIC_TRIGGER.NONE)
        {
              if (trigger == GeneralUtils.SUBJECT_TRIGGER.NONE) { de = "Whenever"; }
               else { de += "whenever"; }
            switch (unitTrigger)
            {
                case Unit.UNIT_SPECIFIC_TRIGGER.ATTACKED:
                    de += " this is attacked, ";
                    break;
                case Unit.UNIT_SPECIFIC_TRIGGER.DAMAGE_DEALT:
                    de += " this takes damages from an attack, ";
                    break;
                case Unit.UNIT_SPECIFIC_TRIGGER.DAMAGE_BLOCKED:
                    de += " this blocks damages, ";
                    break;
                case Unit.UNIT_SPECIFIC_TRIGGER.ATTACKS:
                    de += " this attacks, ";
                    break;
                case Unit.UNIT_SPECIFIC_TRIGGER.HEAL:
                    de += " this is healed, ";
                    break;
            }
        }

        List<List<string>> statusDescription = factory != null ? GetFactoryAppliedDescription(factory, effectFactory, unitTrigger):
            CombatStatusFactory.GetFactoryFactoryAppliedDescriptions(factoryOfFactory, effectFactory, unitTrigger);
        if (effectFactory != null && effectFactory.Count > 0)
        {
                string oldSubject = "";
                string currentSubject = "";
            for (int i = 0; i < effectFactory.Count; i++)
            {
                CombatEffectFactory effect = effectFactory[i];
                currentSubject = GetCurrentSubject(unitTrigger, effect.alternative,//CombatEffect.ALTERNATIVE_TARGET.NONE,
                    oldSubject);
                List<string> current = effect.GetDescription();
                statusDescription[0].Add(currentSubject + current[0]);
                statusDescription[1].Add(current[1]);
                statusDescription[2].Add(current[2]);
                oldSubject = currentSubject;
            }
        }
        de += CombatEffect.LinkPrefixAndTime(statusDescription);
        return de;
    }

    public override string GetAnimationName()
    {
        return status.ToString();
    }

}
