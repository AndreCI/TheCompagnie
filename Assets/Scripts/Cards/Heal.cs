using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class Heal : Card
{


    public override void Play(Unit target)
    {
        base.Play(target);
        effect = new CombatEffect(CombatEffect.TYPE.HEAL, owner, target, 4);
        CombatEvent cardEvent = new CombatEvent(owner, new List<Unit> { target }, owner.speed, new List<CombatEffect> { effect }, this);
        TurnManager.Instance.AddCombatEvent(cardEvent);
    }
}

