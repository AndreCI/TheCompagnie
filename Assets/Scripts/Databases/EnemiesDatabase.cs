using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemiesDatabase : ScriptableObject
{

    public List<Enemy> enemies;

    /// <summary>
    /// Get the specified ItemInfo by index.
    /// </summary>
    /// <param name="type">Type.</param>
    public Enemy Get(int index)
    {
        return enemies[index];
    }

    public Enemy GetRandom()
    {
        return enemies[(int)(new System.Random()).Next(enemies.Count)];
    }

}