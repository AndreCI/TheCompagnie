using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[Serializable]
public class CombatStatusFactoryFactory
{
    public CombatStatus.STATUS status;
    public int value;
    public int duration;
    public bool permanent = false;
    public GeneralUtils.SUBJECT_TRIGGER trigger;
    public Unit.UNIT_SPECIFIC_TRIGGER unitTrigger;
    public CombatEffect.ALTERNATIVE_TARGET alternative;
    public CardEffectVariable.VARIABLE variable;

    private CombatStatusFactoryFactory()
    {
        status = CombatStatus.STATUS.BLEED;
    }

    public CombatStatusFactory Generate()
    {
        if(status == CombatStatus.STATUS.BLEED) { return null; }
        CombatStatusFactory f = new CombatStatusFactory(status, value, duration, permanent, new List<CombatEffectFactory>(), trigger, unitTrigger, alternative, variable);
        f.factory = new List<CombatStatusFactoryFactory>();
        return f;
    }

    public static List<string> GetDescription(CombatStatus.STATUS status, int value, int duration, bool permanent, GeneralUtils.SUBJECT_TRIGGER trigger, CardEffectVariable.VARIABLE variable, bool plural=false)
    {
        string prefix = "gain" + (plural?" " : "s ");
        string body = "";
        string time = "";
        string amountStr = value.ToString();
        if (variable != CardEffectVariable.VARIABLE.STATIC) { value = CardEffectVariable.GetVariable(variable, null, null, value); }
        switch (status)
        {
            case CombatStatus.STATUS.BLOCK:
                body = amountStr + " <b>blocks</b>";
                break;
            case CombatStatus.STATUS.BUFF_STR:
                body = amountStr + " <b>strength</b>";
                break;
            case CombatStatus.STATUS.REDUCE_STR:
                prefix = plural ? "lose " : "looses ";
                body = amountStr + " <b>strength</b>";
                break;
            case CombatStatus.STATUS.BUFF_SPEED:
                body = amountStr + " <b>speed</b>";
                break;
            case CombatStatus.STATUS.RECUDE_SPEED:
                prefix = plural ? "lose " : "looses ";
                body = amountStr + " <b>speed</b>";
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
                body = "<b>frost</b>";
                break;
            case CombatStatus.STATUS.BUFF_CHANNEL:
                body = amountStr + " <b>concentration</b>";
                break;
            case CombatStatus.STATUS.REDUCE_CHANNEL:
                prefix = plural ? "lose " : "looses ";
                body = amountStr + " <b>concentration</b>";
                break;
            case CombatStatus.STATUS.STATUS_APPLY:
                throw new InvalidOperationException();

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

        return new List<string> { prefix, body, time };
    }

}