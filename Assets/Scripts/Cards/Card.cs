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
    [Header("Basic Informations")]
    public int ID;
    public string Name;
    public int manaCost;
    public int delay;
    public int actionCost = 1;
    [Header("UI & Animations")]
    public Sprite sprite;
    public string Description;
    [Header("Mechanics & effects")]
    public bool multipleTarget;
    public POTENTIAL_TARGET potential_target;
    public List<CombatEffect> effects;
    public bool channel = false;
    public int channelLenght;

    [Header("Database infos")]
    public CardDatabase.RARITY rarity;
    public string databasePath;
    public CardDatabase.CARDCLASS cardClass;


    [HideInInspector]
    public Unit owner;



    public Card(Unit owner_, Card baseCard)
    {

        /*Card copy = GeneralUtils.Copy<Card>(baseCard);
        copy.owner = owner_;
        this = copy;*/
        

        ID = baseCard.ID;
        Name = baseCard.Name;
        manaCost = baseCard.manaCost;
        delay = baseCard.delay;
        actionCost = baseCard.actionCost;
        sprite = baseCard.sprite;
        Description = baseCard.Description;
        multipleTarget = baseCard.multipleTarget;
        potential_target = baseCard.potential_target;
        effects = new List<CombatEffect>(baseCard.effects);
        channel = baseCard.channel;
        channelLenght = baseCard.channelLenght;
        rarity = baseCard.rarity;
        databasePath = baseCard.databasePath;
        cardClass = baseCard.cardClass;
        owner = owner_;

}


    public virtual void Play(List<Unit> targets)
    {
        owner.CurrentAction -= actionCost;
        if (!channel) { 
            AddEvent(owner.currentSpeed + delay, targets);
        }
        else
        {
            for(int i = 0; i < channelLenght; i++)
            {
                AddEvent(owner.currentSpeed + delay + i, targets);
            }
        }
        
        
    }

    private void AddEvent(int timeIndex, List<Unit> targets)
    {
        owner.CurrentMana -= manaCost;
       // if (timeIndex > 9) { timeIndex = 9; }
        CombatEvent cardEvent = new CombatEvent(owner, targets, timeIndex, effects, this, channel);
        TurnManager.Instance.AddCombatEvent(cardEvent);
    }


}

