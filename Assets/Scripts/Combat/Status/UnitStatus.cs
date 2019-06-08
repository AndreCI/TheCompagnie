using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

[Serializable]
public abstract class UnitStatus
{
    public int value;
    [HideInInspector] public bool noValue;
    public int duration;
    public bool permanent;
    [HideInInspector] public Unit target;
    [HideInInspector] public bool showUi;

    public abstract string GetAnimationName();
    public abstract string GetDescription();
    public abstract void Notified(GeneralUtils.SUBJECT_TRIGGER currentTrigger);

}