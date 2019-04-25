using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UICardDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public bool locked = false;

	public void OnPointerEnter(PointerEventData eventData) {
        //Debug.Log("OnPointerEnter");
        if (!locked)
        {
            if (eventData.pointerDrag == null)
                return;

            CardUI d = eventData.pointerDrag.GetComponent<CardUI>();
            if (d != null && d.Playable)
            {
                d.placeholderParent = this.transform;
            }
        }
	}
	
	public void OnPointerExit(PointerEventData eventData) {
        //Debug.Log("OnPointerExit");
        if (!locked)
        {
            if (eventData.pointerDrag == null)
                return;

            CardUI d = eventData.pointerDrag.GetComponent<CardUI>();
            if (d != null && d.Playable && d.placeholderParent == this.transform)
            {
                d.placeholderParent = d.parentToReturnTo;
            }
        }
	}
	
	public void OnDrop(PointerEventData eventData) {
        if (!locked)
        {
            Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

            CardUI d = eventData.pointerDrag.GetComponent<CardUI>();
            if (d != null && d.Playable)
            {
                d.parentToReturnTo = this.transform;
            }
        }

	}
}
