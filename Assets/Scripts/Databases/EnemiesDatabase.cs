using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemiesDatabase : ScriptableObject
{

    public List<Enemy> enemies;

    public List<Enemy> GetRandomFromClass(CardDatabase.CARDCLASS current, CardDatabase.SUBCARDCLASS subclass, int count)
    {
        return enemies.FindAll(x => x.availableCards == current && x.subclass == subclass).OrderBy(y => Utils.rdx.Next()).Take(count).ToList();
    }
    public List<Enemy> GetRandomFromClass(CardDatabase.CARDCLASS current, int count)
    {
        return enemies.FindAll(x => x.availableCards == current).OrderBy(y => Utils.rdx.Next()).Take(count).ToList();
    }

    public List<Enemy> GetRandomFromType(Enemy.ENEMY_TYPE type, int count)
    {
        List<Enemy> typedEnemies = Shuffle(new List<Enemy>(enemies.FindAll(x => x.type == type))).Take(count).ToList(); ;
        List<Enemy> retured = new List<Enemy>();
        foreach(Enemy e in typedEnemies)
        {
            retured.Add(GeneralUtils.Copy<Enemy>(e));
        }
        return retured;
       // return typedEnemies.FindAll(x=>x.type == type)[(int)(new System.Random()).Next(typedEnemies.Count)];

    }

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
        return enemies[(int)Utils.rdx.Next(enemies.Count)];
    }

    public static List<Enemy> Shuffle(List<Enemy> units)
    {
        int n = units.Count;
        while (n > 1)
        {
            n--;
            int k = Utils.rdx.Next(n + 1);
            Enemy value = units[k];
            units[k] = units[n];
            units[n] = value;
        }
        return units;
    }

}