using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    private static CombatManager instance;
    public static CombatManager Instance { get => instance; }
    public Compagnion player;
    public Enemy enemy;


    public List<Compagnion> compagnions;
    public Deck compagnionDeck;
    public List<Enemy> enemies;
    public List<Deck> enemiesDeck;

    void Start()
    {
        instance = this;
        StartCombat(new List<Compagnion> { player }, new List<Enemy> { enemy });
    }

    public void StartCombat(List<Compagnion> compagnions_, List<Enemy> enemies_)
    {
        compagnions = compagnions_;
        enemies = enemies_;
        List<Deck> cd = new List<Deck>();
        foreach(Compagnion c in compagnions)
        {
            cd.Add(c.GetDeck());
        }
        compagnionDeck = new Deck(cd);
        enemiesDeck = new List<Deck>();
        foreach(Enemy e in enemies)
        {
            enemiesDeck.Add(e.GetDeck());
        }
        TurnManager.Instance.StartTurn();
    }
}