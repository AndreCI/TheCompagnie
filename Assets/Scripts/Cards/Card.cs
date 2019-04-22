using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : Draggable
{
    public Unit owner;
    public CombatEffect effect = new CombatEffect();
    public CardHandler handler;



    public void Play(Unit target)
    {
        CombatEvent cardEvent = new CombatEvent(owner, target, owner.speed, effect, this);
        TurnManager.Instance.AddCombatEvent(cardEvent);
    }


public override void OnEndDrag(PointerEventData pointerEventData)
    {
        base.OnEndDrag(pointerEventData);
        Unit target = this.transform.parent.transform.parent.GetComponent<Unit>();
        if (target != null)
        {
            Play(target);
        }
    }
}

