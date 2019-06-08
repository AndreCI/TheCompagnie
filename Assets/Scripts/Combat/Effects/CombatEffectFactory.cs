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
                body += "heals " + amountStr + " health points";
                break;
            case CombatEffect.TYPE.MANA_GAIN:
                body += "gains " + amountStr + " mana points";
                break;
            case CombatEffect.TYPE.DRAW:
                body += "draws " + amountStr + " cards";
                break;
            case CombatEffect.TYPE.APPLY_STATUS:
                throw new NotImplementedException();
            case CombatEffect.TYPE.MOVE_INTENT:
                body += "moves its next intent by " + amountStr + " ticks";
                break;

        }
        return new List<string> { prefix, body, suffix };
    }
}