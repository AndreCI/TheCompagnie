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
    MANA_GAIN,
    MOVE_INTENT};

    
    public enum ALTERNATIVE_TARGET { NONE, SELF};
    public enum CONDITION { NONE, STATUS_BURN, TARGET_CHANNEL};

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

    private bool CheckCondition(Unit target, Unit source)
    {
        switch (condition)
        {
            case CONDITION.NONE:
                return true;
            case CONDITION.STATUS_BURN:
                return target.CurrentStatus.Any(x => x.status == CombatStatus.STATUS.BURN);
            case CONDITION.TARGET_CHANNEL:
                return TurnManager.Instance.GetCurrentEvents(true).Find(x => x.channel && x.source == source) != null;
          
        }
        return true;
    }

    public void Perform(Unit target, Unit source, float timeFactor=1f, float forcedTime=0f)
    {
        if(type != TYPE.APPLY_STATUS && statusFactories != null && statusFactories.Count > 0)
        {
            Debug.Log("Issue with combat effect " + type.ToString() + ";" + amount.ToString() + "; target:" + target.ToString() + "; source:" + source.ToString());
        }
        if(!CheckCondition(target, source)) { return; }
        if(alternative == ALTERNATIVE_TARGET.SELF) { target = source; }
        amount = CardEffectVariable.GetVariable(this, target, source);
        PlayerInfos.Instance.animationDatabase.Get(animation)?.Activate(CombatManager.Instance.GetUnitUI(target), timeFactor:timeFactor, forcedTime:forcedTime);
        switch (type)
        {
            case TYPE.DAMAGE:
                source.TriggerSpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.ATTACKS);
                target.TriggerSpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.ATTACKED);
                target.TakeDamage(amount + source.currentStrength);
                break;
            case TYPE.HEAL:
                target.Heal(amount);
                break;
            case TYPE.APPLY_STATUS:
                foreach(CombatStatusFactory statusFactory in statusFactories)
                        {
                            CombatStatus status = statusFactory.GenerateApply(target);
           
                        }
                break;
            case TYPE.DRAW:
                List<Card> drawnCards = CombatManager.Instance.compagnionDeck.DrawCards(new List<int> { amount }, new List<Unit> { target });
                List<Card> returned = Hand.Instance.AddToHand(drawnCards);
                CombatManager.Instance.compagnionDeck.AddCards(returned);
                break;
            case TYPE.MANA_GAIN:
                target.GainMana(amount);
                break;
            case TYPE.MOVE_INTENT:
                CombatEvent moved = TurnManager.Instance.GetNextCombatEvent(target);
                if(moved != null)
                {
                    if(amount + TurnManager.Instance.currentIndex> TurnManager.Instance.timeSteps.Count)
                    {
                        amount = TurnManager.Instance.timeSteps.Count - TurnManager.Instance.currentIndex;
                    }
                    if(amount + TurnManager.Instance.currentIndex < 0) { amount = 0; }
                    TurnManager.Instance.MoveCombatEvent(moved, amount);
                }
                break;
        }
        
    }
    public string GetDescription(Unit source = null, Unit target = null, int channelLength=0)
    {
        string constrct = "";
        if (OnPlay)
        {
            constrct += "0n Play: ";
        }
        switch (condition)
        {
            case CONDITION.NONE:
                break;
            case CONDITION.TARGET_CHANNEL:
                constrct += "If target is channeling: ";
                break;
        }
        string amountStr = "";
        switch (variable)
        {
            case CardEffectVariable.VARIABLE.STATIC:
                if (source == null || source.currentStrength == 0 ||type!= TYPE.DAMAGE)
                {
                    amountStr += amount.ToString();
                }
                else {
                    string strgamount = (source.currentStrength > 0 ? "+" + source.currentStrength.ToString() : source.currentStrength.ToString());
                    amountStr += (amount.ToString() + strgamount);
                    if(channelLength <= 0) {
                        amountStr += " (" + (amount + source.currentStrength > 0 ? amount + source.currentStrength : 0).ToString() + ")";
                    }
                }
                if (channelLength > 0)
                {
                    int totalA = source == null || type != TYPE.DAMAGE ? amount : amount + source.currentStrength;
                    if (totalA < 0) { totalA = 0; }
                    amountStr += " (" + (totalA * channelLength).ToString() +")";
                }
                break;
            case CardEffectVariable.VARIABLE.MISSING_CURRENT_HEALTH:
                int v = (source != null && target != null) ? CardEffectVariable.GetVariable(this, target, source) : 0;
                amountStr += v.ToString() + " (missing current health)";
                break;
            case CardEffectVariable.VARIABLE.BURN_STATUS:
                if (source == null)
                {
                    amountStr += (amount).ToString();
                }
                else { amountStr += (amount + source?.currentStrength).ToString(); }

                break;
        }
        if(alternative == ALTERNATIVE_TARGET.SELF && type != TYPE.DAMAGE)
        {
            constrct += "Self : ";
        }

        switch (type)
        {
            case TYPE.DAMAGE:
                constrct += (alternative == ALTERNATIVE_TARGET.SELF ? "Take " : "Deal ");
                constrct += amountStr + " damages";
                break;
            case TYPE.HEAL:
                constrct += "Heal " + amountStr + " health points";
                break;
            case TYPE.MANA_GAIN:
                constrct += "Gain " + amountStr + " mana points";
                break;
            case TYPE.DRAW:
                constrct += "Draw " + amountStr + " cards";
                break;
            case TYPE.APPLY_STATUS:
                foreach(CombatStatusFactory csf in statusFactories)
                {
                    constrct += csf.GetDescription() + " ";
                }
               // constrct = constrct.Remove(0, constrct.Length - 1);
                break;
            case TYPE.MOVE_INTENT:
                constrct += "Move intent by " + amountStr + " ticks";
                break;

        }
        if (type != TYPE.APPLY_STATUS)
        {
            constrct += ". ";
        }
        return constrct;
    }
}
 