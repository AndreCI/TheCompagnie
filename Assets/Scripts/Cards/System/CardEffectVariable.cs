using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardEffectVariable
{
    public enum VARIABLE { STATIC, MISSING_CURRENT_HEALTH, BURN_STATUS, BURN_DAMAGE, MISSING_CURRENT_MANA,
    FROST_DURATION};

    public static int GetVariable(VARIABLE variable, Unit target=null, Unit source=null, int value=0)
    {

        switch (variable)
        {
            case VARIABLE.MISSING_CURRENT_HEALTH:
                return source.maxHealth - source.CurrentHealth;
            case VARIABLE.BURN_STATUS:
                return target.CurrentStatus.Count(x => x.status == CombatStatus.STATUS.BURN);
            case VARIABLE.BURN_DAMAGE:
                return target.CurrentStatus.FindAll(x => x.status == CombatStatus.STATUS.BURN).Select(x => x.value).Sum();
            case VARIABLE.MISSING_CURRENT_MANA:
                return source.maxMana - source.CurrentMana;
            case VARIABLE.FROST_DURATION:
                return target.CurrentStatus.FindAll(x => x.status == CombatStatus.STATUS.FROST).Select(x => x.permanent ? 10 : x.duration).Sum();
        }
        return value;
    }


    public static string GetDescription(VARIABLE variable, Unit target = null, Unit source = null, int value = 0)
    {
        string amountStr = "";
        int v = (source != null && target != null) ? CardEffectVariable.GetVariable(variable, target, source, value) : 0;
        switch (variable)
        {
            case CardEffectVariable.VARIABLE.MISSING_CURRENT_HEALTH:
                amountStr += v.ToString() + " (missing current health)";
                break;
            case CardEffectVariable.VARIABLE.BURN_STATUS:
                amountStr += v.ToString() + " (number of burn status)";
                break;
            case CardEffectVariable.VARIABLE.MISSING_CURRENT_MANA:
                amountStr += v.ToString() + " (missing current mana)";
                break;
            case CardEffectVariable.VARIABLE.BURN_DAMAGE:
                amountStr += v.ToString() + " (burn damage on the next turn)";
                break;
            case VARIABLE.FROST_DURATION:
                amountStr += v.ToString() + " (frost duration)";
                break;
        }
        return amountStr;
    }

}
