using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CombatStatus : Observer
{
    public enum STATUS {
        REGEN
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
        TurnManager.Instance.AddObserver(this, trigger);
        target.currentStatus.Add(this);
    }

    public void Notified(Subject subject, GeneralUtils.SUBJECT_TRIGGER currentTrigger)
    {
        Debug.Log("trigger");
        if(currentTrigger == trigger)
        {
            Apply();
            duration -= 1;
            if(duration <= 0)
            {
                //pass
            }
        }
    }

    private void Apply()
    {
        switch (status)
        {
            case STATUS.REGEN:
            Debug.Log("apply");
                target.Heal(value);
                break;
        }
    }
}
