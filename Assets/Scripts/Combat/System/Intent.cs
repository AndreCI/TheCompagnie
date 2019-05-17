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
    public Image glowingMask;
    public Color phantomColor;
    public Color focusColor;
    public Color channelColor;
    public Color activatedColor;

    private float duration;
    private bool activated;
    public bool phantom = false;

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
    public void Setup(Card card_, CardUI ui_, CombatEvent linkedEvent_, bool inverseSprite, bool _phantom = false)
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
        phantom = _phantom;
        SetGlowingColor();
    }
    private void SetGlowingColor()
    {
        if (phantom)
        {
            glowingMask.CrossFadeColor(phantomColor, 0.2f, false, true);
        }else if (activated)
        {
            glowingMask.CrossFadeColor(new Color(activatedColor.r, activatedColor.g, activatedColor.b, 0f), duration, false, true);
        }
        else if (card.channel)
        {
            glowingMask.CrossFadeColor(channelColor, 0.2f, false, true);
        }
        else
            glowingMask.CrossFadeColor(new Color(0f, 0f, 0f, 0f), 0.2f, false, true);
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
            glowingMask.CrossFadeColor(activatedColor, 0f, false, true); ;
            SetGlowingColor();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!phantom)
        {
            glowingMask.CrossFadeColor(focusColor, 0.2f, false, true);
            UI.gameObject.SetActive(true);
            if (CursorManager.Instance.type == CursorManager.CURSOR_TYPE.DEFAULT)
                CursorManager.Instance.type = CursorManager.CURSOR_TYPE.DEFAULT_ARROW;
            if (card.hidden && !Hand.Instance.locked)
            {
                CombatManager.Instance.HiddenCard.actionCost = 1;
                CombatManager.Instance.HiddenCard.delay = card.delay;
                CombatManager.Instance.HiddenCard.manaCost = card.manaCost;

                UI.Setup(CombatManager.Instance.HiddenCard);
            }
            else
            {
                UI.Setup(card);
                UI.description.text = card.GetDescription(linkedEvent.source, linkedEvent.targets);
                UnitSelector.Instance.ForceSelection(new List<Unit> { linkedEvent.source }, UnitSelector.SELECTION_MODE.SHOWSOURCE);
                UnitSelector.Instance.ForceSelection(linkedEvent.targets, UnitSelector.SELECTION_MODE.TCURRENT);
            }
            UI.Playable = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!phantom)
        {
            SetGlowingColor();
            UI.gameObject.SetActive(false);
            if (CursorManager.Instance.type == CursorManager.CURSOR_TYPE.DEFAULT_ARROW)
                CursorManager.Instance.type = CursorManager.CURSOR_TYPE.DEFAULT;
            if (!card.hidden || Hand.Instance.locked)
            {
                UnitSelector.Instance.EndForceSelection(UnitSelector.SELECTION_MODE.SHOWSOURCE);
                UnitSelector.Instance.EndForceSelection(UnitSelector.SELECTION_MODE.TCURRENT);

            }
        }
    }
}