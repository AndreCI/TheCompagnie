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
    public enum CONDITION { NONE, STATUS_BURN};

    public TYPE type;
    public ALTERNATIVE_TARGET alternative;
    public CONDITION condition;
    public AnimationClipDatabase.T animation;
    public CardEffectVariable.VARIABLE variable;
    public int amount;
    [Header("Status (only if APPLY_STATUS)")]
    public List<CombatStatusFactory> statusFactories;
    

    private bool CheckCondition(Unit target, Unit source)
    {
        switch (condition)
        {
            case CONDITION.NONE:
                return true;
            case CONDITION.STATUS_BURN:
                return target.CurrentStatus.Any(x => x.status == CombatStatus.STATUS.BURN);

          
        }
        return true;
    }

    public void Perform(Unit target, Unit source, float timeFactor=1f, float forcedTime=0f)
    {
        if(type != TYPE.APPLY_STATUS && statusFactories != null && statusFactories.Count > 0)
        {
            Debug.Log("Issue with combat effect " + type.ToString() + ";" + amount.ToString() + "; target:" + target.ToString() + "; source:" + source.ToString());
        }
        if(alternative == ALTERNATIVE_TARGET.SELF) { target = source; }
        if(!CheckCondition(target, source)) { return; }
        amount = CardEffectVariable.GetVariable(this, target, source);
        PlayerInfos.Instance.animationDatabase.Get(animation)?.Activate(CombatManager.Instance.GetUnitUI(target), timeFactor:timeFactor, forcedTime:forcedTime);
        switch (type)
        {
            case TYPE.DAMAGE:
                //if(variable == CardEffectVariable.VARIABLE.BURN_STATUS) { Debug.Log(amount); }
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
}