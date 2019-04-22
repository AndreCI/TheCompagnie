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
    public CardHandler handler;


    public void Play()
    {

    }

    public override void OnEndDrag(PointerEventData pointerEventData)
    {
        base.OnEndDrag(pointerEventData);
        Unit target = this.transform.parent.transform.parent.GetComponent<Unit>();
        if (target != null)
        {
            target.TakeDamage(6);
        }
    }
}
