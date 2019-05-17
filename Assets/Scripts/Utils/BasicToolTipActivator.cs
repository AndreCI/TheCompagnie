using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicToolTipActivator : MonoBehaviour, ITooltipActivator, IPointerEnterHandler, IPointerExitHandler
{
    public bool trackMouse = true;
    public List<string> description;
    private bool tooltip = false;
    public bool ToolTipShow { get => tooltip; set => tooltip = value; }

    public void OnDestroy()
    {
        OnToolTip(false);
    }
    public void OnDisable()
    {
        OnToolTip(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnToolTip(true);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        OnToolTip(false);

    }

    public void OnToolTip(bool show)
    {
        if (ToolTipShow != show)
        {
            ToggleTipWindow.Instance.gameObject.SetActive(show);
            ToggleTipWindow.Instance.trackMouse = trackMouse && show;
            ToggleTipWindow.Instance.GetComponent<RectTransform>().pivot = new Vector2(Input.mousePosition.x < Screen.width / 2f ? 0f : 1f, 1f);
            ToggleTipWindow.Instance.ToggleText.text = description.Aggregate((x, y) => x + "\n" + y); ;
            ToggleTipWindow.Instance.transform.position = Input.mousePosition;
            ToolTipShow = show;
        }
    }
}
