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
        PARRY
};

    public STATUS status;
    public int value;
    public int duration;
    public GeneralUtils.SUBJECT_TRIGGER trigger;
    public Unit target;

    public CombatStatus(STATUS status_, int value_, int duration_, GeneralUtils.SUBJECT_TRIGGER trigger_, Unit target_)
    {
        status = status_;
        value = value_;
        duration = duration_;
        trigger = trigger_;
        target = target_;
        TurnManager.NotifyAll += Notified;
        target.currentStatus.Add(this);
    }

    public void Notified(GeneralUtils.SUBJECT_TRIGGER currentTrigger)
    {
        if (currentTrigger == trigger)
        {
            Apply();
            duration -= 1;
            CheckUpdate();
        }
    }
    private void Apply()
    {
        switch (status)
        {
            case STATUS.REGEN:
                target.Heal(value);
                break;

        }
    }

    public void CheckUpdate()
    {
        if(value == 0 || duration <= 0)
        {
            target.currentStatus.Remove(this);
            TurnManager.NotifyAll -= Notified;
        }
    }

}
