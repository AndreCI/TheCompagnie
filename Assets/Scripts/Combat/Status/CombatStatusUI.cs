using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CombatStatusUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        if (combatStatus.permanent)
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        icon.sprite = data.icon;
    }
}