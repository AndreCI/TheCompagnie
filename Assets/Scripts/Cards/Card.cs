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
    public enum CARD_TYPE {NONE, ATTACK, SPELL, ABILITY, POWER, CURSE}
    [Header("Basic Informations")]
    public string Name;
    public int manaCost;
    public int delay;
    public int actionCost = 1;
    [Header("Mechanics & effects")]
    public List<CombatEffect> effects;
    public int channelLenght;
    public bool channel = false;
    public bool cancellable;
    public bool multipleTarget;
    public POTENTIAL_TARGET potential_target;
    public bool hidden = false;

    [Header("UI & Animations")]
    public Sprite sprite;
    public string Description;
    public AudioSound.AUDIO_SET set;

    [Header("Database infos")]
    public CardDatabase.RARITY rarity;
    public string databasePath;
    public CardDatabase.CARDCLASS cardClass;
    public CardDatabase.SUBCARDCLASS subClass;
    public CardDatabase.BRANCH branch;
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
        cancellable = baseCard.cancellable;
        channelLenght = baseCard.channelLenght;
        hidden = baseCard.hidden;
        rarity = baseCard.rarity;
        databasePath = baseCard.databasePath;
        cardClass = baseCard.cardClass;
        subClass = baseCard.subClass;
        set = baseCard.set;
        branch = baseCard.branch;
        owner = owner_;

}
    

    public virtual void Play(List<Unit> targets)
    {
        owner.CurrentAction -= actionCost;
        if (!channel) { 
            AddEvent(Mathf.Max(0,owner.currentSpeed + delay), targets);
        }
        else
        {
            CombatEvent link = null;
            int channelValue = Mathf.Max(0, channelLenght + owner.CurrentChannelValue);
            for(int i = 0; i < channelValue; i++)
            {
                link = AddEvent(Mathf.Max(0, owner.currentSpeed + delay) + i, targets, link);
            }
        }
        owner.TriggerSpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.PLAY_CARD, owner);
        if (cancellable)
        {
            owner.TriggerSpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.PLAY_CANCEL_CARD, owner);

        }



    }

    private CombatEvent AddEvent(int timeIndex, List<Unit> targets, CombatEvent previous=null)
    {
        if (manaCost <= owner.CurrentMana)
        {
            owner.CurrentMana -= manaCost;
            CombatEvent cardEvent = new CombatEvent(owner, targets, timeIndex, effects, this, set, channel, cancellable);
            if (previous != null) { previous.nextChannelEvent = cardEvent; }
            if (effects.Count(x => !x.OnPlay) > 0 || owner.GetType() == typeof(Enemy)) { TurnManager.Instance.AddCombatEvent(cardEvent); }
            return cardEvent;
        }
        return null;
    }
    public override string ToString()
    {
        string d = "Card: " + Name + "; Class: "+cardClass.ToString() +" (MC " + manaCost.ToString() + "; D " + delay.ToString() + "; AC " + actionCost.ToString() + ")\n";
         d += GetDescription();
         d += "\n";
        d += GetKeywordDescription();
        d += "\n";
        d += rarity.ToString() + " " + branch.ToString() + " " + ID.ToString();
        return d;
    }
    public string GetKeywordDescription()
    {
        string description = "";

        if (channel)
        {
            description += "<b>Channel X</b>: This card takes longer than one tick, its effects are duplicate X times with an increasing delay. \n" +
                
                (manaCost > 0? 
                "Each copy cost the same mana cost displayed on the card. If you don't have enough for all copies, later one will not be duplicated.\n" +
                "<i>For instance, casting this card completly will cost "+ (channelLenght * manaCost).ToString()+" mana.</i>\n"
                : "");
        }
        if (cancellable)
        {
            description += "<b>Cancellable</b>: If the owner of this card is hit by an attack, this card will be cancelled: its effects will not happend and its mana cost will be refounded.\n";
        }
        if (multipleTarget)
        {
            if(potential_target == POTENTIAL_TARGET.ENEMIES) {
                description += "<b>All enemies</b>: This card affects all enemies.\n";
            }else if(potential_target == POTENTIAL_TARGET.PARTY)
            {
                description += "<b>All friends</b>: This card affects all friends.\n";
            }
        }
        List<CombatStatusFactory> statusEffects = effects.FindAll(x => x.type == CombatEffect.TYPE.APPLY_STATUS).SelectMany(e=>e.statusFactories).ToList();
        statusEffects.AddRange(statusEffects.FindAll(x => x.status == CombatStatus.STATUS.STATUS_APPLY).SelectMany(e => e.factory.Select(f=>f.Generate())));
        if(statusEffects.Find(x=>x.status == CombatStatus.STATUS.BLOCK) !=null)
        {
            description += "<b>Block</b>: Prevents damage from attacks.\n";
        }if (statusEffects.Find(x => x.status == CombatStatus.STATUS.BUFF_STR) != null || (statusEffects.Find(x => x.status == CombatStatus.STATUS.REDUCE_STR) != null))
        {
            description += "<b>Strength</b>: Increase or decrease damages from attacks.\n";
        }
        if (statusEffects.Find(x => x.status == CombatStatus.STATUS.BUFF_SPEED) != null || (statusEffects.Find(x => x.status == CombatStatus.STATUS.RECUDE_SPEED) != null))
        {
            description += "<b>Speed</b>: Increase or decrease the number of ticks needed to play a card.\n";
        }
        if (statusEffects.Find(x => x.status == CombatStatus.STATUS.FROST) != null)
        {
            description += "<b>Frost</b>: All attacks from the target deal half damage (rounded down, applied after strength).\n";
        
        }if (statusEffects.Find(x => x.status == CombatStatus.STATUS.BUFF_CHANNEL) != null || (statusEffects.Find(x => x.status == CombatStatus.STATUS.REDUCE_CHANNEL) != null))
        {
            description += "<b>Concentration</b>: Increase or decrease the number of intent created by all cards with <b>channel</b>.\n";
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
            int channelbuff = source == null ? 0 : source.CurrentChannelValue;
            desc += "<b>Channel</b> " + channelLenght.ToString();
            if(channelbuff > 0) { desc += " (+" + channelbuff.ToString() + ")"; }
            if(channelbuff < 0) { desc += " (" + channelbuff.ToString() + ")"; }
            desc += ": ";
        }
        
        for(int i = 0; i < effects.Count; i ++)// (CombatEffect ce in effects)
        {
            CombatEffect ce = effects[i];
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
            string current =  ce.GetDescription(source, targets != null? targets[0] : null, channel? channelLenght : 0, GetCardType());
          //  if(i!= 0) { current = current.ToLower(); }
            desc += current + (i== effects.Count - 1 ? "" : "\n");
        }
        if(actionCost == 0)
        {
            desc += "Cost no action.";
        }
        if (cancellable)
        {
            desc += "<b>Cancellable</b>.";
        }
        return desc;
    }

    public int GetPrevisionDamage()
    {
        List<int> damages = effects.FindAll(x => x.type == CombatEffect.TYPE.DAMAGE && x.variable == CardEffectVariable.VARIABLE.STATIC).Select(x => x.amount).ToList();
        int total = damages.Sum() + (owner == null ? 0 : owner.CurrentStrength) * damages.Count();
        
        if (channel) { int channelValue = Mathf.Max(0, channelLenght + (owner == null ? 0 : owner.CurrentChannelValue));
            total *= channelValue; }
        total = Mathf.FloorToInt((float)total/(owner == null || owner.CurrentStatus.Find(x=>x.status == CombatStatus.STATUS.FROST) ==  null ? 1f : 2f));
        return total;
    }

    public string GetTypeAndRarityDescription()
    {
        string de = "";

        de += GetCardType().ToString().ToLower();
        de += " - " + rarity.ToString().ToLower();

        return de;
    }

    public string GetTargetTypeDescription()
    {
        switch (potential_target)
        {
            case POTENTIAL_TARGET.ENEMIES:
                return multipleTarget ? "All enemies" : "Any enemy";
            case POTENTIAL_TARGET.FRIEND:
                return "Any other ally";
            case POTENTIAL_TARGET.SELF:
                return "Self only";
            case POTENTIAL_TARGET.PARTY:
                return multipleTarget ? "All allies" : "Any ally";
        }
        return "";
    }

    public string GetCardClassName()
    {
        if(owner != null && owner.GetType() == typeof(Enemy))
        {
            if(subClass != CardDatabase.SUBCARDCLASS.GLOBAL)
            {
                return owner.unitName.ToUpper();
            }
            else
            {
                return cardClass.ToString();
            }
        }
        else
        {
            switch (cardClass)
            {
                case CardDatabase.CARDCLASS.PALADIN:
                    if(branch == CardDatabase.BRANCH.NONE && subClass == CardDatabase.SUBCARDCLASS.TYPE1 ||
                        branch == CardDatabase.BRANCH.BASIC) { return "Fighter"; }
                    else if (branch == CardDatabase.BRANCH.NONE && subClass == CardDatabase.SUBCARDCLASS.TYPE2 ||
                        branch == CardDatabase.BRANCH.B1) { return "Paladin"; }
                    else if (branch == CardDatabase.BRANCH.NONE && subClass == CardDatabase.SUBCARDCLASS.TYPE3 ||
                        branch == CardDatabase.BRANCH.B2) { return "Warrior"; }
                    break;
                case CardDatabase.CARDCLASS.ELEM:

                    if (branch == CardDatabase.BRANCH.NONE && subClass == CardDatabase.SUBCARDCLASS.TYPE1 ||
                        branch == CardDatabase.BRANCH.BASIC) { return "Arcane"; }
                    else if (branch == CardDatabase.BRANCH.NONE && subClass == CardDatabase.SUBCARDCLASS.TYPE2 ||
                        branch == CardDatabase.BRANCH.B1) { return "Fire"; }
                    else if (branch == CardDatabase.BRANCH.NONE && subClass == CardDatabase.SUBCARDCLASS.TYPE3 ||
                        branch == CardDatabase.BRANCH.B2) { return "Ice"; }
                    break;

                case CardDatabase.CARDCLASS.HUNTER:

                    if (branch == CardDatabase.BRANCH.NONE && subClass == CardDatabase.SUBCARDCLASS.TYPE1 ||
                        branch == CardDatabase.BRANCH.BASIC) { return "Hunter"; }
                    else if (branch == CardDatabase.BRANCH.NONE && subClass == CardDatabase.SUBCARDCLASS.TYPE2 ||
                        branch == CardDatabase.BRANCH.B1) { return "Assassin"; }
                    else if (branch == CardDatabase.BRANCH.NONE && subClass == CardDatabase.SUBCARDCLASS.TYPE3 ||
                        branch == CardDatabase.BRANCH.B2) { return "Ranger"; }
                    break;
            }
        }
        return "";
    }

    public CARD_TYPE GetCardType()
    {
        if (effects.Find(x => x.type == CombatEffect.TYPE.DAMAGE && x.alternative != CombatEffect.ALTERNATIVE_TARGET.SELF) != null)
        {
            return CARD_TYPE.ATTACK;

        }
        else if (effects.Count(x => !x.OnPlay) == 0)
        {
            return CARD_TYPE.ABILITY;
        }
        else if (effects.FindAll(x => x.type == CombatEffect.TYPE.APPLY_STATUS).
            SelectMany(y=>y.statusFactories).ToList().
            Find(sf=>sf.status == CombatStatus.STATUS.STATUS_APPLY)
             != null)
        {
            return (potential_target == POTENTIAL_TARGET.SELF || potential_target == POTENTIAL_TARGET.PARTY || potential_target == POTENTIAL_TARGET.FRIEND) ?
                CARD_TYPE.POWER : CARD_TYPE.CURSE;
        }
        else
        {
            return CARD_TYPE.SPELL;
        }

    }

}

