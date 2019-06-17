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
        if (PlayerInfos.Instance.readyForBoss)
        {
            enemies.Add(GeneralUtils.Copy<Enemy>(database.Get(6)));
        }
        else if (combatIndex == 0)
        {//database.Get(7)) };//d
            enemies = database.GetRandomFromClass(CardDatabase.CARDCLASS.BEAST,CardDatabase.SUBCARDCLASS.TYPE2, 1); //GetRandomFromType(Enemy.ENEMY_TYPE.BEAST, 1);

        }
        else if (combatIndex == 1)
        {
            enemies = database.GetRandomFromClass(CardDatabase.CARDCLASS.BEAST, 2);
        }else if(combatIndex == 2)
        {
            enemies = database.GetRandomFromClass(CardDatabase.CARDCLASS.SPIRIT, 2);

        }
        else if (combatIndex == 3)
        {
            enemies = database.GetRandomFromClass(CardDatabase.CARDCLASS.UNDEAD, 1); //GetRandomFromType(Enemy.ENEMY_TYPE.BEAST, 1);
        }
        else //if(combatIndex == 3)
        {
            float random = (float)Utils.rdx.Next(100);
            CardDatabase.CARDCLASS selectedClass;
            if(random > 80)
            {
                selectedClass = CardDatabase.CARDCLASS.BEAST;
            }else if(random > 40) {
                selectedClass = CardDatabase.CARDCLASS.SPIRIT;
            }
            else
            {
                selectedClass = CardDatabase.CARDCLASS.UNDEAD;
            }
            enemies = database.GetRandomFromClass(selectedClass, 2); 

        }

        while (enemies.Select(x=>x.level.currentLevel).Sum() < PlayerInfos.Instance.compagnions.Select(y => y.level.currentLevel).Sum() -2)
        {
            Enemy nextEnemy = EnemiesDatabase.Shuffle(enemies).First();
            nextEnemy.AddLevel();
        }
        List<Enemy> copies = new List<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            Enemy copy = enemy.Setup() as Enemy;

            copy.id = Unit.currentId++;
            copies.Add(copy);
            
        }
        combatIndex++;
        return copies;
    }

 
}