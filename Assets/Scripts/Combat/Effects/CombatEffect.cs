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
    [Header("Status (only if APPLY_STATUS)")]
    public List<CombatStatusFactory> statusFactories;


    public void Perform(Unit target, Unit source)
    {
        if(type != TYPE.APPLY_STATUS && statusFactories != null && statusFactories.Count > 0)
        {
            Debug.Log("Issue with combat effect " + type.ToString() + ";" + amount.ToString() + "; target:" + target.ToString() + "; source:" + source.ToString());
        }
        switch (type)
        {
            case TYPE.DAMAGE:
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
        }
        
    }
}