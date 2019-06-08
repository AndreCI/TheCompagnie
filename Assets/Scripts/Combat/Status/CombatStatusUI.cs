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

    public UnitStatus unitStatus;

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
                    unitStatus.showUi = false;
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
    public void Setup(Talent t)
    {
        unitStatus = t;
        data = new CombatStatusData(t.name, CombatStatus.STATUS.BLEED, t.icon, t.icon);
        Setup();
    }

    private void Setup()
    {
        iconMask.sprite = data.icon;
        icon.sprite = data.icon;

        if (unitStatus.noValue)
        {
            text1.transform.parent.parent.gameObject.SetActive(false);
        }
        if (unitStatus.permanent)
        {
            text2.transform.parent.parent.gameObject.SetActive(false);
        }
        UpdateData();
    }
    public void Setup(CombatStatus s)
    {
        unitStatus = s;
        data = s.miscData;
        data.mainAnimation?.Setup();
        data.finalAnimation?.Setup();
        Setup();
    }

    public void UpdateData()
    {
        if(this == null) { return; }
        if (!unitStatus.noValue)
        {
            text1.text = unitStatus.value.ToString();
        }
        if (!unitStatus.permanent)
        {
            text2.text = unitStatus.duration.ToString();
        }
    }

    public void Trigger(float duration_ = 0.3f, bool forceAnimation=true)
    {
        if (!activated && unitStatus != null)
        {
            UpdateData();
            duration = duration_;
            activated = true;
            AudioManager.Instance.Play(unitStatus.GetAnimationName(), false);
            if (setDestroy && data.finalAnimation != null) {
                CombatManager.Instance.GetUnitUI(unitStatus.target).animationHandler.Play(data.finalAnimation, forcedTime:duration,forcePlay:forceAnimation);}
            else if(data.mainAnimation!=null) { CombatManager.Instance.GetUnitUI(unitStatus.target).animationHandler.Play(data.mainAnimation, forcedTime: duration, forcePlay: forceAnimation); }
            
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
        ToggleTipWindow.Instance.GetComponent<RectTransform>().pivot = new Vector2(typeof(Compagnion) == unitStatus.target.GetType() ? 0f : 1f, 1f);
        ToggleTipWindow.Instance.ToggleText.text = unitStatus.GetDescription();
        ToggleTipWindow.Instance.transform.position = Input.mousePosition;
        ToolTipShow = show;
    }
}