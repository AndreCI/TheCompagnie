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
    APPLY_STATUS};
    public TYPE type;
    public int amount;
    public List<CombatStatusFactory> statusFactories;
    private Unit source;
    public CombatEffect(TYPE type_, Unit source_)
    {
        type = type_;
        source = source_;
    }

    public void Perform(Unit target)
    {
        switch (type)
        {
            case TYPE.DAMAGE:
                target.TakeDamage(amount);
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
        }
        
    }
}