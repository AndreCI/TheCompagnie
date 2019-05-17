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

    public CombatStatus GenerateApply(Unit target)
    {
        return new CombatStatus(status, value, duration, permanent, trigger, target);
    }

    public string GetDescription()
    {
        string descr = "Apply " + value.ToString();
        switch (status)
        {
            case CombatStatus.STATUS.BLOCK:
                descr += " blocks ";
                break;
            case CombatStatus.STATUS.BUFF_STR:
                descr = "Augment strength by " + value.ToString() + " ";
                break;
            case CombatStatus.STATUS.REDUCE_STR:
                descr = "Reduce strength by " + value.ToString() + " ";
                break;
            case CombatStatus.STATUS.BURN:
                descr += " burn ";
                break;
            case CombatStatus.STATUS.PARRY:
                descr += " parry ";
                break;
            case CombatStatus.STATUS.POISON:
                descr += " poison ";
                break;
            case CombatStatus.STATUS.REGEN:
                descr += " regen ";
                break;
        }
        if((status == CombatStatus.STATUS.BLOCK && duration == 1))
        {
            descr = "Apply " + value.ToString() + " blocks";
        }
        else if (!permanent)
        {
            descr += " for " + duration.ToString();
            switch (trigger)
            {
                case GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN:
                    descr += " turn(s)";
                    break;
                case GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK:
                    descr += " tick(s)";
                    break;
            }
        }
        else
        {
            descr += "until end of combat";
        }
        descr += '.';
        return descr;
    }
}