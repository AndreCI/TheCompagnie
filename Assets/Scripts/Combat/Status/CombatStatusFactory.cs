using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[Serializable]
public class CombatStatusFactory
{
    public CombatStatus.STATUS status;
    public int value;
    public int duration;
    public bool permanent = false;
    public GeneralUtils.SUBJECT_TRIGGER trigger;
    public Unit.UNIT_SPECIFIC_TRIGGER unitTrigger;
    public List<CombatStatusFactoryFactory> factory;
    public List<CombatEffectFactory> effectFactory;
    public CardEffectVariable.VARIABLE variable;
    [NonSerialized] public CombatEffect.ALTERNATIVE_TARGET alternative;
    
    public CombatStatusFactory(CombatStatus.STATUS status_, int value_, int duration_, bool permanent_, List<CombatEffectFactory> effectFactory_,
        GeneralUtils.SUBJECT_TRIGGER baseTrigger, Unit.UNIT_SPECIFIC_TRIGGER unitTrigger_, CombatEffect.ALTERNATIVE_TARGET alternative_, 
        CardEffectVariable.VARIABLE variable_)
    {
        status = status_;
        value = value_;
        duration = duration_;
        permanent = permanent_;
        effectFactory = effectFactory_;
        trigger = baseTrigger;
        unitTrigger = unitTrigger_;
        alternative = alternative_;
        variable = variable_;
        factory = new List<CombatStatusFactoryFactory>();

    }

    public bool IsBeneficial()
    {
        if(status == CombatStatus.STATUS.STATUS_APPLY)
        {
            return factory.Select(x => CombatStatus.IsStatusGood(x.status)).All(x=>x);
        }
        else
        {
            return CombatStatus.IsStatusGood(status);
        }
    }

    public void GenerateApply(Unit target,Card source=null, CombatStatusData data=null)
    {
        if(data == null || status != CombatStatus.STATUS.STATUS_APPLY)
        {

            data = PlayerInfos.Instance.effectDatabase.Get(status);
        }if(source != null && status == CombatStatus.STATUS.STATUS_APPLY)
        {
            data = new CombatStatusData(source.Name, status, source.sprite, source.sprite);

        }List<Unit> alternativeTargets = CombatEffect.GetAlternativeTargets(new List<Unit>(), target, alternative);
        if(variable != CardEffectVariable.VARIABLE.STATIC) { value = CardEffectVariable.GetVariable(variable, target, source.owner, value); }
        if(alternativeTargets.Count > 0)
        {
            foreach(Unit altT in alternativeTargets)
            {
                CombatStatus current = new CombatStatus(status, value, duration, permanent, trigger, altT, unitTrigger, new List<CombatStatusFactory>(
            factory.Select(x => x.Generate())), effectFactory, data);
            }
            return;
        }
      //  if (onSelf) { target = source; }
        new CombatStatus(status, value, duration, permanent, trigger, target, unitTrigger, new List<CombatStatusFactory>(
            factory.Select(x=>x.Generate())), effectFactory, data);
    }

    public List<string> GetDescription(Unit target)
    {
        string prefix = "Apply ";
        string body = "";
        string time = "";
        string amountStr = value.ToString();
        if(variable != CardEffectVariable.VARIABLE.STATIC) { amountStr = CardEffectVariable.GetDescription(variable, target, target, value); }
        switch (status)
        {
            case CombatStatus.STATUS.BLOCK:
                body = amountStr + " <b>blocks</b>";
                break;
            case CombatStatus.STATUS.BUFF_STR:
                prefix = "Augment ";
                body = "<b>strength</b> by " + amountStr;
                break;
            case CombatStatus.STATUS.REDUCE_STR:
                prefix = "Reduce ";
                body = "<b>strength</b> by " + amountStr;
                break;
            case CombatStatus.STATUS.BUFF_SPEED:
                prefix = "Augment ";
                body = "<b>speed</b> by " + amountStr;
                break;
            case CombatStatus.STATUS.RECUDE_SPEED:
                prefix = "Reduce ";
                body = "<b>speed</b> by " + amountStr;
                break;
            case CombatStatus.STATUS.BURN:
                body = amountStr + " <b>burn</b>";
                break;
            case CombatStatus.STATUS.PARRY:
                body = amountStr + " <b>parry</b>";
                break;
            case CombatStatus.STATUS.POISON:
                body = amountStr + " <b>poison</b>";
                break;
            case CombatStatus.STATUS.REGEN:
                body = amountStr + " <b>regeneration</b>";
                break;
            case CombatStatus.STATUS.FROST:
                body = " <b>frost</b>";
                break;
            case CombatStatus.STATUS.BUFF_CHANNEL:
                prefix = "Augment ";
                body = "<b>conentration</b> by " + amountStr;
                break;
            case CombatStatus.STATUS.REDUCE_CHANNEL:
                prefix = "Reduce ";
                body = "<b>concentration</b> by " + amountStr;
                break;
            case CombatStatus.STATUS.STATUS_APPLY:
                return GetApplyDescr();

        }
        if (!permanent)
        {
            time = " for " + duration.ToString();
            switch (trigger)
            {
                case GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN:
                    time += " turn" + (duration > 1 ? "s" : "");
                    break;
                case GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK:
                    time += " tick" + (duration > 1 ? "s" : "");
                    break;
            }
        }
        else
        {
            time = " until end of combat";
        }

        return new List<string>{prefix, body, time};
    }

    private List<string> GetApplyDescr()
    {
        return new List<string>{"",
            CombatStatus.GetApplyDescription(trigger, unitTrigger, permanent, duration, value, status, effectFactory, null, factory),
        ""};
    }

    public static List<List<string>> GetFactoryFactoryAppliedDescriptions(List<CombatStatusFactoryFactory> factory, List<CombatEffectFactory> effectFactory, GeneralUtils.SUBJECT_TRIGGER trigger, Unit.UNIT_SPECIFIC_TRIGGER unitTrigger)
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
                    CombatStatusFactoryFactory f = factory[i];
                    currentSubject = CombatStatus.GetCurrentSubject(trigger, unitTrigger, f.alternative);
                    oldSubject = currentSubject;
                    List<string> c = CombatStatusFactoryFactory.GetDescription(f.status, f.value, f.duration, f.permanent, f.trigger, f.variable);
                    prefix.Add(currentSubject + c[0]);
                    bodies.Add(c[1]);
                    time.Add(c[2]);

                }
            }
        }
        
        return new List<List<string>> { prefix, bodies, time };
    }

}