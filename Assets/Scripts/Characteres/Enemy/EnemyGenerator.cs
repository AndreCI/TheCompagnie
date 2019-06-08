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
            enemies = database.GetRandomFromClass(CardDatabase.CARDCLASS.BEAST, CardDatabase.SUBCARDCLASS.TYPE2, 1); //GetRandomFromType(Enemy.ENEMY_TYPE.BEAST, 1);

        }
        else
        {

            enemies = database.GetRandomFromClass(CardDatabase.CARDCLASS.BEAST, 2);
        }/*
        else if (combatIndex == 1 || combatIndex == 2)
        {
            enemies = database.GetRandomFromClass(CardDatabase.CARDCLASS.BEAST, 2); //GetRandomFromType(Enemy.ENEMY_TYPE.BEAST, 1);
        }
        else if (combatIndex == 3)
        {
            enemies = database.GetRandomFromType(Enemy.ENEMY_TYPE.UNDEAD, 1);
        }
        else //if(combatIndex == 3)
        {
            if (combatIndex % 4 == 0)
            {
                enemies = database.GetRandomFromType(Enemy.ENEMY_TYPE.BEAST, 2);

            }
            else
            {
                //  enemies = new List<Enemy> { GeneralUtils.Copy<Enemy>(database.GetRandom()), GeneralUtils.Copy<Enemy>(database.GetRandom()) };
                enemies = database.GetRandomFromType(Enemy.ENEMY_TYPE.UNDEAD, 2);
            }
        }
        */
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