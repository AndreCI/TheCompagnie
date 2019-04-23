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
    public GeneralUtils.SUBJECT_TRIGGER trigger;

    public CombatStatus GenerateApply(Unit target)
    {
        return new CombatStatus(status, value, duration, trigger, target);
    }
}