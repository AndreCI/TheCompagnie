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

    public bool setToDestroy = true;
    private void Update()
    {
        if (activated)
        {
            if (iconMask.fillAmount >= 1)
            {
                activated = false;
                iconMask.fillAmount = 0f;
                if (setToDestroy)
                {
                    Destroy(gameObject);
                }
                else
                {
                    setToDestroy = true;
                }
            }
            else
            {
                iconMask.fillAmount += Time.deltaTime / duration;
            }
        }
    }
    public void Setup(Card card_, CardUI ui_, CombatEvent linkedEvent_, bool inverseSprite)
    {
        transform.localScale = new Vector3(1f, inverseSprite ? -1f : 1f, 1f);
        card = card_;
        UI = ui_;
        linkedEvent = linkedEvent_;
        icon.sprite = card.hidden ? CombatManager.Instance.HiddenCard.sprite : card.sprite;
        iconMask.sprite = icon.sprite;
        if (linkedEvent.channel)
        {
            iconMask.fillMethod = Image.FillMethod.Horizontal;
        }
        setToDestroy = true;
        if (card.hidden)
        {
            TurnManager.NotifyAll += TurnManager_NotifyAll;
        }
    }

    private void OnDestroy()
    {
        TurnManager.NotifyAll -= TurnManager_NotifyAll;
    }

    private void TurnManager_NotifyAll(GeneralUtils.SUBJECT_TRIGGER trigger)
    {
        if(trigger == GeneralUtils.SUBJECT_TRIGGER.START_OF_TIME)
        {
            if (card.hidden)
            {
                icon.sprite = card.sprite;
                iconMask.sprite = icon.sprite;
                card.hidden = false;
            }
        }
    }

    public void Trigger(float duration_)
    {
        if (!activated || setToDestroy)
        {
            iconMask.fillAmount = 0f;
            activated = true;
            duration = duration_;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.gameObject.SetActive(true);
        if (card.hidden && !Hand.Instance.locked)
        {
            UI.Setup(CombatManager.Instance.HiddenCard);
        }
        else
        {
            UI.Setup(card);
            UnitSelector.Instance.ForceSelection(new List<Unit> { linkedEvent.source }, UnitSelector.SELECTION_MODE.SHOWSOURCE);
            UnitSelector.Instance.ForceSelection(linkedEvent.targets , UnitSelector.SELECTION_MODE.TCURRENT);
        }
        UI.Playable = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.gameObject.SetActive(false);
        if (!card.hidden || Hand.Instance.locked)
        {
            UnitSelector.Instance.EndForceSelection(UnitSelector.SELECTION_MODE.SHOWSOURCE);
            UnitSelector.Instance.EndForceSelection(UnitSelector.SELECTION_MODE.TCURRENT);

        }
    }
}