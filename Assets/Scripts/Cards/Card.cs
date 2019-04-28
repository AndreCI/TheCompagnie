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

    public enum POTENTIAL_TARGET {ENEMIES, PARTY, NONE };
    [HideInInspector]
    public Unit owner;

    public string Name;
    public Sprite sprite;
    public POTENTIAL_TARGET potential_target;
    public List<CombatEffect> effects;
    public int manaCost;
    public int actionCost = 1;
    public bool multipleTarget;

    public int ID;
    public string Description;
    public int delay;

    public Card(Unit owner_, Card baseCard)
    {
        owner = owner_;
        sprite = baseCard.sprite;
        effects = new List<CombatEffect>(baseCard.effects);
        manaCost = baseCard.manaCost;
        potential_target = baseCard.potential_target;
        actionCost = baseCard.actionCost;
        multipleTarget = baseCard.multipleTarget;
        Name = baseCard.Name;
        delay = baseCard.delay;
        ID = baseCard.ID;
        Description = baseCard.Description;

    }


    public virtual void Play(List<Unit> targets)
    {
        owner.CurrentMana -= manaCost;
        owner.CurrentAction -= actionCost;
        int timeIndex = owner.currentSpeed + delay;
        if (timeIndex > 9) { timeIndex = 9; }
        foreach (Unit target in targets)
        {
            CombatEvent cardEvent = new CombatEvent(owner, target, timeIndex, effects, this);
            TurnManager.Instance.AddCombatEvent(cardEvent);
        }
        
    }


}

