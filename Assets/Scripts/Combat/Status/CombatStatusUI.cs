using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class CombatStatusUI : MonoBehaviour
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
        iconMask.sprite = PlayerInfos.Instance.effectDatabase.Get(s.status);
        icon.sprite = PlayerInfos.Instance.effectDatabase.Get(s.status);
        UpdateData();
    }

    public void UpdateData()
    {
        text1.text = combatStatus.value.ToString();
        text2.text = combatStatus.duration.ToString();
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

}