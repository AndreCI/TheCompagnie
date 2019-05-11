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
        icon.sprite = card.sprite;
        iconMask.sprite = card.sprite;
        if (linkedEvent.channel)
        {
            iconMask.fillMethod = Image.FillMethod.Horizontal;
        }
        setToDestroy = true;
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
        UnitSelector.Instance.ForceSelection(linkedEvent.targets , UnitSelector.SELECTION_MODE.TCURRENT);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.gameObject.SetActive(false);
        UnitSelector.Instance.EndForceSelection(UnitSelector.SELECTION_MODE.TCURRENT);
    }
}