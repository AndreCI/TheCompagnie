using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UICardDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public bool locked = false;
    public bool targeting = false;
    public bool selectorNotified = true;
    public Card.POTENTIAL_TARGET target_type;
    public bool partyDropZone;

	public virtual void OnPointerEnter(PointerEventData eventData) {
        if (!locked)
        {
            if (eventData.pointerDrag == null)
                return;

            CardUI d = eventData.pointerDrag.GetComponent<CardUI>();
            if (d != null && d.Playable && IsAcceptableTarget(d))
            {
                d.placeholderParent = this.transform;
                targeting = true;
                selectorNotified = false;
            }
        }
	}
	
	public virtual void OnPointerExit(PointerEventData eventData) { 
        if (!locked)
        {
            if (eventData.pointerDrag == null)
                return;

            CardUI d = eventData.pointerDrag.GetComponent<CardUI>();
            if (d != null && d.Playable && IsAcceptableTarget(d) && d.placeholderParent == this.transform)
            {
                d.placeholderParent = d.parentToReturnTo;
                targeting = false;
                selectorNotified = false;
            }
        }
	}
	
	public virtual void OnDrop(PointerEventData eventData) {
        if (!locked)
        {
//            Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

            CardUI d = eventData.pointerDrag.GetComponent<CardUI>();
            if (d != null && d.Playable && IsAcceptableTarget(d))
            {
                d.parentToReturnTo = this.transform;
                targeting = false;
                selectorNotified = false;
            }
        }

	}

    public virtual bool IsAcceptableTarget(CardUI card)
    {
        return card.card.potential_target == target_type;// && card.card.multipleTarget == partyDropZone;
    }
}
