using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Intent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Card card;
    public CardUI UI;
    public CombatEvent linkedEvent;
    public Image icon;
    public Image iconMask;

    private float duration;
    private bool activated;

    private void Update()
    {
        if (activated)
        {
            if (iconMask.fillAmount >= 1)
            {
                activated = false;
                iconMask.fillAmount = 0f;
                Destroy(gameObject);
            }
            else
            {
                iconMask.fillAmount += Time.deltaTime / duration;
            }
        }
    }
    public void Setup(Card card_, CardUI ui_, CombatEvent linkedEvent_)
    {
        card = card_;
        UI = ui_;
        linkedEvent = linkedEvent_;
        icon.sprite = card.sprite;
        iconMask.sprite = card.sprite;
    }

    public void Trigger(float duration_)
    {
        if (!activated)
        {
            activated = true;
            duration = duration_;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.gameObject.SetActive(true);
        UI.Setup(card);
        UI.Playable = false;
        UnitSelector.Instance.ForceSelection(new List<Unit> { linkedEvent.target }, UnitSelector.SELECTION_MODE.TCURRENT);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.gameObject.SetActive(false);
        UnitSelector.Instance.EndForceSelection(UnitSelector.SELECTION_MODE.TCURRENT);
    }
}