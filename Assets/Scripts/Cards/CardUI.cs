using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : Draggable, Observer
{
    public Card card;
    public Image image;

    private bool playable;
    public bool Playable
    {
        get => playable; set
        {
            playable = value;
            //draggable = value;
        }
    }

    public void Setup(Card card_)
    {
        card = card_;
        image.sprite = card.sprite;
    }
    public void Play(Unit target)
    {
        if (playable)
        {
            card.Play(target);
        }
    }


    public override void OnEndDrag(PointerEventData pointerEventData)
    {
        base.OnEndDrag(pointerEventData);
        Unit target = this.transform.parent.transform.parent.GetComponent<Unit>();
        if (target != null)
        {
            Play(target);
        }
    }

    public void Notified(Subject subject)
    {

    }
}

