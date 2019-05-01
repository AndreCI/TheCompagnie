using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectDatabase : ScriptableObject
{

    public List<CombatStatus.STATUS> status;
    public List<Sprite> sprites;

    /// <summary>
    /// Get the specified ItemInfo by index.
    /// </summary>
    /// <param name="type">Type.</param>
    public Sprite Get(CombatStatus.STATUS type)
    {
        return sprites[status.IndexOf(type)];
    }


}