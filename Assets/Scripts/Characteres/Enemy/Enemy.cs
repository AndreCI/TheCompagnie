using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Enemy : Unit
{

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if(currentHealth <= 0)
        {
            CombatManager.Instance.OnUnitDeath(this);
        }
    }

}
