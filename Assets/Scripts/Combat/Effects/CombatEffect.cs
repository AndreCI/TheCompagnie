using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
[Serializable]
public class CombatEffect
{
    public enum TYPE {DAMAGE,
    HEAL,
    APPLY_STATUS,
    DRAW,
    MANA_MODIFY,
    MOVE_INTENT,
    ACTION_GAIN,
    CANCEL_INTENT,
    MANA_REGEN_MODIFY};

    
    public enum ALTERNATIVE_TARGET { NONE, SELF, FRIEND, ALL_ENEMY, ALL_FRIEND, ALL, RANDOM_ENEMY, NEXT_INTENT_OWNER};
    public enum CONDITION { NONE, STATUS_BURN, TARGET_CHANNEL, STATUS_FROST, STATUS_BLOCK_NO, STATUS_POISON, HEALTH_IS_MAX_HEALTH};

    [Header("Status (only if APPLY_STATUS)")]
    public List<CombatStatusFactory> statusFactories;
    [Header("General Information")]
    public int amount;
    public TYPE type;
    public AnimationClipDatabase.T animation;
    [Header("Specific parameters")]
    public CardEffectVariable.VARIABLE variable;
    public ALTERNATIVE_TARGET alternative;
    public CONDITION condition;
    public bool OnPlay = false;

    public CombatEffect(int amount_, TYPE type_, CardEffectVariable.VARIABLE variable_, ALTERNATIVE_TARGET alt, CONDITION condition_)
    {
        amount = amount_;
        type = type_;
        variable = variable_;
        alternative = alt;
        condition = condition_;
        OnPlay = false;
        statusFactories = new List<CombatStatusFactory>();
    }

    private bool CheckCondition(Unit target, Unit source)
    {
        switch (condition)
        {
            case CONDITION.NONE:
                return true;
            case CONDITION.STATUS_BURN:
                return target.CurrentStatus.Any(x => x.status == CombatStatus.STATUS.BURN);
            case CONDITION.STATUS_FROST:
                return target.CurrentStatus.Any(x => x.status == CombatStatus.STATUS.FROST);
            case CONDITION.STATUS_POISON:
                return target.CurrentStatus.Any(x => x.status == CombatStatus.STATUS.POISON);
            case CONDITION.TARGET_CHANNEL:
                return TurnManager.Instance.GetCurrentEvents(true).Find(x => x.channel && x.source == target) != null;
            case CONDITION.STATUS_BLOCK_NO:
                return !target.CurrentStatus.Any(x => x.status == CombatStatus.STATUS.BLOCK);
            case CONDITION.HEALTH_IS_MAX_HEALTH:
                return target.CurrentHealth == target.maxHealth;

        }
        return true;
    }

    public static List<Unit> GetAlternativeTargets(List<Unit> targets, Unit source, ALTERNATIVE_TARGET alternative)
    {
        switch (alternative)
        {
            case ALTERNATIVE_TARGET.ALL:
                targets = new List<Unit>(CombatManager.Instance.compagnions);
                targets.AddRange(CombatManager.Instance.enemies);
                break;
            case ALTERNATIVE_TARGET.ALL_FRIEND:
                if (source.GetType() == typeof(Compagnion)){
                    targets = new List<Unit>(CombatManager.Instance.compagnions);
                }
                else
                {
                    targets = new List<Unit>(CombatManager.Instance.enemies);
                }
                break;
            case ALTERNATIVE_TARGET.ALL_ENEMY:
                if (source.GetType() != typeof(Compagnion)){
                    targets = new List<Unit>(CombatManager.Instance.compagnions);
                }
                else
                {
                    targets = new List<Unit>(CombatManager.Instance.enemies);
                }
                break;
            case ALTERNATIVE_TARGET.NEXT_INTENT_OWNER:
                targets = new List<Unit> { TurnManager.Instance.GetNextCombatEvent().source };
        
                break;

        }return targets;
    }


    private Unit.DAMAGE_SOURCE_TYPE GetDamageType(Card cardSource)
    {
        if(cardSource == null)
        {
            return Unit.DAMAGE_SOURCE_TYPE.STATUS;
        }if(alternative == ALTERNATIVE_TARGET.SELF || alternative == ALTERNATIVE_TARGET.ALL_FRIEND || alternative == ALTERNATIVE_TARGET.FRIEND)
        {
            return Unit.DAMAGE_SOURCE_TYPE.SELF_ATTACK;
        }
        return Unit.DAMAGE_SOURCE_TYPE.ATTACK;
        
    }
    public void Perform(List<Unit> targets, Unit source, Card cardSource=null, float timeFactor=1f, float forcedTime=0f, CombatStatusData statusData =null)
    {
        targets = GetAlternativeTargets(targets, source, this.alternative);
        
        for(int idx= 0; idx < targets.Count; idx++)
        {
            Unit target = targets[idx];
                    if(type != TYPE.APPLY_STATUS && statusFactories != null && statusFactories.Count > 0)
        {
            Debug.Log("Issue with combat effect " + type.ToString() + ";" + amount.ToString() + "; target:" + target.ToString() + "; source:" + source.ToString());
        }
            if (!CheckCondition(target, source)) { return; }
            if (alternative == ALTERNATIVE_TARGET.SELF) { target = source; }
        //s    if(alternative == ALTERNATIVE_TARGET.FRIEND) { target = (CombatManager.Instance.com)}
            amount = CardEffectVariable.GetVariable(variable, target, source, amount);

            CombatManager.Instance.GetUnitUI(target)?.animationHandler?.Play(PlayerInfos.Instance.animationDatabase.Get(animation),
                timeFactor: timeFactor, forcedTime: forcedTime, true);
            switch (type)
            {
                case TYPE.DAMAGE:
                    Unit.DAMAGE_SOURCE_TYPE type = GetDamageType(cardSource);
                    int damage = 0;
                    if (type == Unit.DAMAGE_SOURCE_TYPE.ATTACK)
                    {
                        source?.TriggerSpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.ATTACKS, target);
                        if(cardSource != null && cardSource.cancellable)
                        {
                            source?.TriggerSpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.ATTACKS_CANCELLABLE, target);
                        }
                        target?.TriggerSpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.ATTACKED, source);
                        float divide = (source != null && source.CurrentStatus.Any(x => x.status == CombatStatus.STATUS.FROST) ? 2f : 1f);
                        damage = Mathf.FloorToInt((float)(amount + source.CurrentStrength) / divide);
                    }
                    else { damage = amount; }
                    target?.TakeDamage(damage > 0 ? damage : 0, type,  source);
                    break;
                case TYPE.HEAL:
                    target.Heal(amount);
                    break;
                case TYPE.APPLY_STATUS:
                    foreach (CombatStatusFactory statusFactory in statusFactories)
                    {
                        statusFactory.GenerateApply(target, cardSource, statusData);

                    }
                    break;
                case TYPE.DRAW:
                    List<Card> drawnCards = CombatManager.Instance.compagnionDeck.DrawCards(new List<int> { amount }, new List<Unit> { target });
                    List<Card> returned = Hand.Instance.AddToHand(drawnCards);
                    CombatManager.Instance.compagnionDeck.AddCards(returned);
                    break;
                case TYPE.MANA_MODIFY:
                    target.ManaModify(amount);
                    break;
                case TYPE.MOVE_INTENT:
                    CombatEvent moved = TurnManager.Instance.GetNextCombatEvent(target);
                    if (moved != null)
                    {
                        if (amount + TurnManager.Instance.currentIndex > TurnManager.Instance.timeSteps.Count)
                        {
                            amount = TurnManager.Instance.timeSteps.Count - TurnManager.Instance.currentIndex;
                        }
                        if (amount + TurnManager.Instance.currentIndex < 0) { amount = 0; }
                        TurnManager.Instance.MoveCombatEvent(moved, amount);
                    }
                    break;
                case TYPE.CANCEL_INTENT:
                    CombatEvent nextE = TurnManager.Instance.GetNextCombatEvent(target);
                    if(nextE != null) { nextE.Remove(); }
                    break;
                case TYPE.ACTION_GAIN:
                    target.CurrentAction += amount;
                    break;
                case TYPE.MANA_REGEN_MODIFY:
                    target.CurrentManaRegen += amount;
                    break;
            }
        }
    }
    public string GetDescription(Unit source = null, Unit target = null, int channelLength=0, Card.CARD_TYPE cardType=Card.CARD_TYPE.NONE)
    {
        string de = "";
        if (OnPlay && cardType != Card.CARD_TYPE.ABILITY)
        {
            de += "<b>0n Play</b>: ";
        }
        switch (condition)
        {
            case CONDITION.NONE:
                break;
            case CONDITION.TARGET_CHANNEL:
                de += "If target is <b>channeling</b>: ";
                break;
            case CONDITION.STATUS_FROST:
                de += "If target is <b>frozen</b>: ";
                break;
            case CONDITION.STATUS_BURN:
                de += "If target is <b>burned</b>: ";
                break;
            case CONDITION.STATUS_BLOCK_NO:
                de += "If target has no <b>block</b>: ";
                break;
            case CONDITION.STATUS_POISON:
                de += "If target is <b>poisonned</b>: ";
                break;
            case CONDITION.HEALTH_IS_MAX_HEALTH:
                de += "If target has all its health: ";
                break;
        }
        string amountStr = "";
        int currentAmount = 0;
        if (variable == CardEffectVariable.VARIABLE.STATIC)
        {
            if (source == null || source.CurrentStrength == 0 || type != TYPE.DAMAGE)
            {
                amountStr += amount.ToString();
                currentAmount = amount;
            }
            else
            {
                string strgamount = (source.CurrentStrength > 0 ? "+" + source.CurrentStrength.ToString() : source.CurrentStrength.ToString());
                amountStr += (amount.ToString() + strgamount);
                currentAmount = amount + source.CurrentStrength;
                if (channelLength <= 0)
                {
                    float divide = (source != null && source.CurrentStatus.Any(x => x.status == CombatStatus.STATUS.FROST) ? 2f : 1f);
                    int damage = Mathf.FloorToInt((float)(amount + (source != null ? source.CurrentStrength : 0)) / divide);
                    amountStr += " (" + (damage > 0 ? damage : 0).ToString() + ")";
                    currentAmount = damage > 0 ? damage : 0;
                }
            }
            if (channelLength > 0)
            {
                float divide = (source != null && source.CurrentStatus.Any(x => x.status == CombatStatus.STATUS.FROST) ? 2f : 1f);
                int damage = Mathf.FloorToInt((float)(amount + (source != null ? source.CurrentStrength : 0)) / divide);
                int totalA = source == null || type != TYPE.DAMAGE ? amount : damage;
                if (totalA < 0) { totalA = 0; }
                amountStr += " (" + (totalA * channelLength).ToString() + ")";
                currentAmount = totalA * channelLength;
            }
        }
        else
        {
            amountStr = CardEffectVariable.GetDescription(variable, target, source, amount);
            currentAmount = (source != null && target != null) ? CardEffectVariable.GetVariable(variable, target, source, amount) : 0;
        }
        switch (alternative)
        {
            case ALTERNATIVE_TARGET.SELF:
                de += "Self : ";
                break;
            case ALTERNATIVE_TARGET.ALL_ENEMY:
                de += "All enemies : ";
                break;
            case ALTERNATIVE_TARGET.ALL_FRIEND:
                de += "All friends : ";
                break;
            case ALTERNATIVE_TARGET.ALL:
                de += "All : ";
                break;
            case ALTERNATIVE_TARGET.NEXT_INTENT_OWNER:
                de += "Next reacting unit : ";
                break;
        }

        switch (type)
        {
            case TYPE.DAMAGE:
                de += (alternative != ALTERNATIVE_TARGET.NONE ? "Take " : "Deal ");
                de += amountStr + " damages";
                break;
            case TYPE.HEAL:
                de += (alternative != ALTERNATIVE_TARGET.NONE ? "Heal " : "Target <b>heals</b> ") + amountStr + " health points";
                break;
            case TYPE.MANA_MODIFY:
                if(amount < 0) { amountStr = amountStr.Substring(1, amountStr.Count() - 1); }
                de += (alternative != ALTERNATIVE_TARGET.NONE ?
                    (amount >= 0 ? "Gain " : "Loose ") :
                    (amount >= 0 ? "Target gains " : "Target looses ")) + amountStr + " mana points";
                break;
            case TYPE.DRAW:
                de += (alternative != ALTERNATIVE_TARGET.NONE ? "<b>Draw</b> " : "Target <b>draws</b> ") + amountStr + " card" + (currentAmount > 1 ? "s" : "");
                break;
            case TYPE.APPLY_STATUS:
                de += GetStatusDescriptions(target);
                break;
            case TYPE.MOVE_INTENT:
                de += "Move " + (alternative != ALTERNATIVE_TARGET.NONE ? "next " : "targets ") +"action by " + amountStr + " tick" + (currentAmount > 1 ? "s" : "");
                break;
            case TYPE.CANCEL_INTENT:
                de += "Cancel " + (alternative != ALTERNATIVE_TARGET.NONE ? "next " : "targets ") + "action (refound its mana)";
                break;

            case TYPE.ACTION_GAIN:
                if (amount < 0) { amountStr = amountStr.Substring(1, amountStr.Count() - 1); }
                de += (alternative != ALTERNATIVE_TARGET.NONE ?
                    (amount >= 0 ? "Gain " : "Loose ") :
                    (amount >= 0 ? "Target gains " : "Target looses ")) + amountStr + " action point"+ (currentAmount > 1 ? "s" : "");
                break;
            case TYPE.MANA_REGEN_MODIFY:
                if (amount < 0) { amountStr = amountStr.Substring(1, amountStr.Count() - 1); }
                de += (alternative != ALTERNATIVE_TARGET.NONE ? 
                    (amount >= 0 ? "Gain " : "Loose "): 
                    (amount>= 0 ? "Target gains " : "Target looses ")) + amountStr + " mana regeneration per turn";
                break;
        }
        return de + ". ";
    }

    private string GetStatusDescriptions(Unit target)
    {
        List<string> prefix = new List<string>();
        List<string> bodies = new List<string>();
        List<string> time = new List<string>();
        foreach (CombatStatusFactory csf in statusFactories)
        {
            List<string> sde = csf.GetDescription(target);
            prefix.Add(sde[0]);
            bodies.Add(sde[1]);
            time.Add(sde[2]);
        }
        return LinkPrefixAndTime(statusFactories.Count, prefix, bodies, time);
    }
    public static string LinkPrefixAndTime(List<List<string>> data)
    {
        return LinkPrefixAndTime(data[0].Count(), data[0], data[1], data[2]);
    }



   public static string LinkPrefixAndTime(int length, List<string> prefix, List<string> bodies, List<string> time) { 
        string oldPrefix = "";
        string nextTime = "";
        string de = "";
        for(int i = 0; i <length; i++)
        {
            string currentDescription = "";
            string currentPrefix = prefix[i];
            string currentTime = time[i];
            nextTime = i < length - 1 ? time[i + 1] : "";
            if(!oldPrefix.Contains(currentPrefix)) //!= oldPrefix)
            {
                currentDescription += (i > 0 ? currentPrefix.ToLower() : currentPrefix);
            }
            currentDescription += bodies[i];
            if(currentTime != nextTime)
            {
                currentDescription += currentTime;
            }
            de += currentDescription;
            if(i == length - 2)
            {
                de += " and ";
            }else if(i < length - 2)
            {
                de += ", ";
            }
            oldPrefix = currentPrefix;
        }
        return de;
    }
}
 