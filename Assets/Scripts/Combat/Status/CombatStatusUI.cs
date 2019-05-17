using DuloGames.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CombatStatusUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITooltipActivator
{
    public Image icon;
    public Image iconMask;
    public Text text1;
    public Text text2;

    public CombatStatus combatStatus;

    private float duration;
    private bool activated;
    [HideInInspector]
    public bool setDestroy;
    [HideInInspector] private CombatStatusData data;

    public float tooltipDelay = 0.4f;
    private bool tooltip = false;
    public bool ToolTipShow { get => tooltip; set => tooltip = value; }

    private void Update()
    {
        if (activated)
        {
            if(iconMask.fillAmount >= 1)
            {
                activated = false;
                iconMask.fillAmount = 0f;
                if (setDestroy)
                {
                    if (ToolTipShow) { OnToolTip(false); }
                    Destroy(gameObject);
                }
            }
            else
            {
                iconMask.fillAmount += Time.deltaTime / duration;
            }
        }
    }

    public void Setup(CombatStatus s)
    {
        combatStatus = s;
        data = PlayerInfos.Instance.effectDatabase.Get(s.status);
        iconMask.sprite = data.icon;
        icon.sprite = data.icon;
        if (s.permanent)
        {
            text2.transform.parent.parent.gameObject.SetActive(false);
        }
        UpdateData();
    }

    public void UpdateData()
    {
        text1.text = combatStatus.value.ToString();
        if (!combatStatus.permanent)
        {
            text2.text = combatStatus.duration.ToString();
        }
    }

    public void Trigger(float duration_ = 0.3f)
    {
        if (!activated)
        {
            UpdateData();
            duration = duration_;
            activated = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        icon.sprite = data.highlightedSprite;
        OnToolTip(true);
     }

    public void OnPointerExit(PointerEventData eventData)
    {
        icon.sprite = data.icon;
        OnToolTip(false);

    }

    public void OnToolTip(bool show)
    {
        ToggleTipWindow.Instance.gameObject.SetActive(show);
        ToggleTipWindow.Instance.GetComponent<RectTransform>().pivot = new Vector2(typeof(Compagnion) == combatStatus.target.GetType() ? 0f : 1f, 1f);
        ToggleTipWindow.Instance.ToggleText.text = combatStatus.GetDescription();
        ToggleTipWindow.Instance.transform.position = Input.mousePosition;
        ToolTipShow = show;
    }
}