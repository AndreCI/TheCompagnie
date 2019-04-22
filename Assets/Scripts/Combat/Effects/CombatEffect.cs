using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

public class CombatEffect
{
    public enum TYPE {DAMAGE,
    HEAL};
    private TYPE type;
    private Unit source;
    private Unit target;
    private int amount;
    public CombatEffect(TYPE type_, Unit source_, Unit target_, int amount_)
    {
        type = type_;
        source = source_;
        target = target_;
        amount = amount_;
    }

    public void Perform()
    {
        switch (type)
        {
            case TYPE.DAMAGE:
                target.TakeDamage(amount);
                break;
            case TYPE.HEAL:
                target.Heal(amount);
                break;
        }
    }
}