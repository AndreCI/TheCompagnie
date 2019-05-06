using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CompagnionsDatabase : ScriptableObject
{

    public List<Compagnion> compagnions;

    /// <summary>
    /// Get the specified ItemInfo by index.
    /// </summary>
    /// <param name="type">Type.</param>
    public Compagnion Get(int index)
    {
        return compagnions[index];
    }

    

}