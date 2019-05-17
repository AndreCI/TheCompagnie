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
    public enum POTENTIAL_TARGET {ENEMIES, PARTY, NONE, SELF, FRIEND };
    [Header("Basic Informations")]
    public string Name;
    public int manaCost;
    public int delay;
    public int actionCost = 1;
    [Header("Mechanics & effects")]
    public List<CombatEffect> effects;
    public bool channel = false;
    public int channelLenght;
    public bool multipleTarget;
    public POTENTIAL_TARGET potential_target;
    public bool hidden = false;

    [Header("UI & Animations")]
    public Sprite sprite;
    public string Description;

    [Header("Database infos")]
    public CardDatabase.RARITY rarity;
    public string databasePath;
    public CardDatabase.CARDCLASS cardClass;
    public int ID;


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
        hidden = baseCard.hidden;
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
            CombatEvent link = null;
            for(int i = 0; i < channelLenght; i++)
            {
                link = AddEvent(owner.currentSpeed + delay + i, targets, link);
            }
        }
        
        
    }

    private CombatEvent AddEvent(int timeIndex, List<Unit> targets, CombatEvent previous=null)
    {
        if (manaCost <= owner.CurrentMana)
        {
            owner.CurrentMana -= manaCost;
            // if (timeIndex > 9) { timeIndex = 9; }
            CombatEvent cardEvent = new CombatEvent(owner, targets, timeIndex, effects, this, channel);
            if (previous != null) { previous.nextChannelEvent = cardEvent; }
            TurnManager.Instance.AddCombatEvent(cardEvent);
            return cardEvent;
        }
        return null;
    }
    public string GetKeywordDescription()
    {
        string description = "";

        if (channel)
        {
            description += "<b>Channel X</b>: This card takes longer than one tick, its effects are duplicate X times with an increasing delay. \n" +
                "Each copy cost mana, and if you don't have enough for all copies, later one will not be duplicated. \nLoosing HP from any attack cancel all " +
                "remaining copies, refunding mana. \n<i>For instance, casting this card completly will cost "+ (channelLenght * manaCost).ToString()+" mana.</i>\n";
        }
        if (multipleTarget)
        {
            if(potential_target == POTENTIAL_TARGET.ENEMIES) {
                description += "<b>All enemies</b>: This card affects all enemies.\n";
            }else if(potential_target == POTENTIAL_TARGET.FRIEND)
            {
                description += "<b>All friends</b>: This card affects all friends.\n";
            }
        }
        List<CombatStatusFactory> statusEffects = effects.FindAll(x => x.type == CombatEffect.TYPE.APPLY_STATUS).SelectMany(e=>e.statusFactories).ToList();
        if(statusEffects.Find(x=>x.status == CombatStatus.STATUS.BLOCK) !=null)
        {
            description += "<b>Block</b>: Prevents damage.\n";
        }if (statusEffects.Find(x => x.status == CombatStatus.STATUS.BUFF_STR) != null || (statusEffects.Find(x => x.status == CombatStatus.STATUS.REDUCE_STR) != null))
        {
            description += "<b>Strength</b>: Increase or decrease damages from attacks.\n";
        }
        if (statusEffects.Find(x => x.status == CombatStatus.STATUS.BURN) != null)
        {
            description += "<b>Burn</b>: Deals periodic damages to the target.\n";
        }
        if (statusEffects.Find(x => x.status == CombatStatus.STATUS.PARRY) != null)
        {
            description += "<b>Parry</b>: Completly prevent the next instance of damage from an attack, usually for a short time.\n";

        }
        if (statusEffects.Find(x => x.status == CombatStatus.STATUS.POISON) != null)
        {
            description += "<b>Poison</b>: Deals periodic damages. \nIf a target is already poisonned when getting poisonned, its new state will consists of the "+
                "maximum of the two durations and damages. \n<i>Mixing poisons can easily be lethal.</i>\n";

        }
        if (statusEffects.Find(x => x.status == CombatStatus.STATUS.REGEN) != null)
        {
            description += "<b>Regen</b>: Gain health periodically.\n";

        }
        if(description != "")
            description = description.Substring(0, description.Length - 1);
        return description;
    }

    public string GetDescription(Unit source = null, List<Unit> targets = null)
    {
        if (Name == "Hidden") {return "<i>Hidden until the time starts...</i>"; }
        bool alreadyMultp = false;
        string desc = "";
        if (channel)
        {
            desc += "Channel " + channelLenght.ToString() + ": ";
        }
        
        foreach (CombatEffect ce in effects)
        {
            if (multipleTarget && !alreadyMultp && ce.alternative!=CombatEffect.ALTERNATIVE_TARGET.SELF)
            {
                alreadyMultp = true;
                if (potential_target == POTENTIAL_TARGET.FRIEND)
                {
                    desc += "All friends: ";
                }
                else if (potential_target == POTENTIAL_TARGET.ENEMIES)
                {
                    desc += "All enemies: ";
                }
                else
                {
                    desc += "All: ";
                }
            }
            desc += ce.GetDescription(source, targets != null? targets[0] : null, channel? channelLenght : 0) + "\n";
        }
        desc = desc.Substring(0, desc.Length - 1);
        if(actionCost == 0)
        {
            desc += "Cost no action.";
        }
        return desc;
    }

    public int GetPrevisionDamage()
    {
        int total = effects.FindAll(x => x.type == CombatEffect.TYPE.DAMAGE && x.variable == CardEffectVariable.VARIABLE.STATIC).Select(x => x.amount).Sum();
        if (channel) { total *= channelLenght; }
        return total;
    }

}

