using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class CombatEffectFactory { 

    public int amount;
    public CombatEffect.TYPE type;
    public CardEffectVariable.VARIABLE variable;
    public CombatEffect.ALTERNATIVE_TARGET alternative;
    public CombatEffect.CONDITION condition;

    public CombatEffect Generate()
    {
        return new CombatEffect(amount, type, variable, alternative, condition);
    }

    public List<string> GetDescription()
    {
        string prefix = "";
        string body = "";
        string suffix = "";
        string amountStr = amount.ToString();
        switch (type)
        {
            case CombatEffect.TYPE.DAMAGE:
                body += "takes " +amountStr + " damages";
                break;
            case CombatEffect.TYPE.HEAL:
                body += "<b>heals</b> " + amountStr + " health points";
                break;
            case CombatEffect.TYPE.MANA_MODIFY:
                if (amount < 0) { amountStr = amountStr.Substring(1, amountStr.Count() - 1); }
                body += (amount >= 0 ? "gains " : "looses ") + amountStr + " mana points";
                break;
            case CombatEffect.TYPE.DRAW:
                body += "<b>draws</b> " + amountStr + " cards";
                break;
            case CombatEffect.TYPE.APPLY_STATUS:
                throw new NotImplementedException();
            case CombatEffect.TYPE.MOVE_INTENT:
                body += "moves its next action by " + amountStr + " ticks";
                break;
            case CombatEffect.TYPE.CANCEL_INTENT:
                body += "cancel its next action";
                break;
            case CombatEffect.TYPE.ACTION_GAIN:
                if (amount < 0) { amountStr = amountStr.Substring(1, amountStr.Count() - 1); }
                body += (amount >= 0 ? "gains " : "looses ") + amountStr + " action point" + (amount > 1 ? "s" : "");
                break;
            case CombatEffect.TYPE.MANA_REGEN_MODIFY:
                if (amount < 0) { amountStr = amountStr.Substring(1, amountStr.Count() - 1); }
                body += (amount >= 0 ? "gains ":"looses ") + amountStr + " mana regeneration";
                break;

        }
        return new List<string> { prefix, body, suffix };
    }
}