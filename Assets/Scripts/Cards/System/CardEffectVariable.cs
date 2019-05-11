using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardEffectVariable
{
    public enum VARIABLE { STATIC, MISSING_CURRENT_HEALTH, BURN_STATUS};

    public static int GetVariable(CombatEffect effect, Unit target=null, Unit source=null)
    {

        switch (effect.variable)
        {
            case VARIABLE.STATIC:
                return effect.amount;
            case VARIABLE.MISSING_CURRENT_HEALTH:
                return source.maxHealth - source.currentHealth;
            case VARIABLE.BURN_STATUS:
                return target.CurrentStatus.Count(x => x.status == CombatStatus.STATUS.BURN) * 20;
        }
        return 0;
    }
}
