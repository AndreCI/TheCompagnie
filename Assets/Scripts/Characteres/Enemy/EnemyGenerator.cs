using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    private static EnemyGenerator _instance;
    public EnemiesDatabase database;
    public int combatIndex;
    public static EnemyGenerator Instance { get => _instance; }

    private void Start()
    {
        _instance = this;
        combatIndex = 0;
    }

    public List<Enemy> GenerateEnemies()
    {
        List<Enemy> enemies = new List<Enemy>() ;
        if (combatIndex == 0)
        {
            enemies = new List<Enemy> { GeneralUtils.Copy<Enemy>(database.Get(0))};

        }
        else if (combatIndex == 1)
        {
            enemies = new List<Enemy> { GeneralUtils.Copy<Enemy>(database.Get(0)), GeneralUtils.Copy<Enemy>(database.Get(0)) };
        }else if (combatIndex == 2)
        {
            enemies = new List<Enemy> { GeneralUtils.Copy<Enemy>(database.Get(1)) };
        }else if(combatIndex == 3)
        {
            enemies = new List<Enemy> { GeneralUtils.Copy<Enemy>(database.Get(1)), GeneralUtils.Copy<Enemy>(database.Get(1)) };

        }
        List<Enemy> copies = new List<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            Enemy copy = enemy.Setup() as Enemy;
            copies.Add(copy);
            
        }
        Debug.Log(copies[0].persistentDeck.ToString());
        combatIndex++;
        return copies;
    }
}