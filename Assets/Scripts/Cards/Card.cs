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
    public override void OnEndDrag(PointerEventData pointerEventData)
    {
        base.OnEndDrag(pointerEventData);
        Debug.Log("Toast");
        Compagnion target = this.transform.parent.transform.parent.GetComponent<Compagnion>();
        if (target != null)
        {
            target.TakeDamage(6);
        }
    }
}
