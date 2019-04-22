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

            Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
            if (d != null && d.draggable)
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

            Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
            if (d != null && d.draggable && d.placeholderParent == this.transform)
            {
                d.placeholderParent = d.parentToReturnTo;
            }
        }
	}
	
	public void OnDrop(PointerEventData eventData) {
        if (!locked)
        {
            Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

            Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
            if (d != null && d.draggable)
            {
                d.parentToReturnTo = this.transform;
            }
        }

	}
}
