using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

public class CombatEffect
{
    public int damage = 6;
    public CombatEffect()
    {

    }

    public void Perform(Unit target)
    {
        target.TakeDamage(damage);
    }
}