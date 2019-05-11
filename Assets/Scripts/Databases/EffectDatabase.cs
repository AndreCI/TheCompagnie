using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectDatabase : ScriptableObject
{

    public List<CombatStatusData> combatStatusDatas;

    /// <summary>
    /// Get the specified ItemInfo by index.
    /// </summary>
    /// <param name="type">Type.</param>
    public CombatStatusData Get(CombatStatus.STATUS type)
    {
        return combatStatusDatas.Find(x=>x.status == type);
    }


}