using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class Card
{
    [HideInInspector]
    public Unit owner;
    public Sprite sprite;
    public List<CombatEffect> effects;
    public CardHandler handler;
    public int manaCost;
    public int actionCost = 1;

    public Card(Unit owner_, Card baseCard)
    {
        owner = owner_;
        sprite = baseCard.sprite;
        effects = baseCard.effects;
        manaCost = baseCard.manaCost;

    }


    public virtual void Play(Unit target)
    {
        owner.currentMana -= manaCost;
        owner.currentAction -= actionCost;
        owner.UpdateInfo();
        CombatEvent cardEvent = new CombatEvent(owner, new List<Unit> { target }, owner.speed, effects, this);
        TurnManager.Instance.AddCombatEvent(cardEvent);
    }
}

