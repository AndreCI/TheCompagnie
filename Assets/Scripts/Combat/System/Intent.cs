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
    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.gameObject.SetActive(true);
        UI.Setup(card);
        UI.Playable = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.gameObject.SetActive(false);
    }
}