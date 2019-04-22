using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Card : Draggable
{
    public Unit owner;
    public CombatEffect effect;
    public CardHandler handler;
    public int manaCost;



    public virtual void Play(Unit target)
    {
        owner.currentMana -= manaCost;
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

