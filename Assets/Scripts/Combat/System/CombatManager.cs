using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    private static CombatManager instance;
    public static CombatManager Instance { get => instance; }
    public UnitUI player;
    public UnitUI enemy;

    public List<Compagnion> compagnions;
    public Deck compagnionDeck;
    public Deck compagnionDiscard;
    public List<Enemy> enemies;
    public List<Deck> enemiesDeck;

    void Start()
    {
        instance = this;
        player.SetInfos(PlayerInfos.Instance.compagnions[0]);
        enemy.SetInfos(PlayerInfos.Instance.enemy);
        StartCombat(PlayerInfos.Instance.compagnions, new List<Enemy> { PlayerInfos.Instance.enemy });
        List<Deck> discards = new List<Deck>();
        foreach(Compagnion c in compagnions)
        {
            discards.Add(new Deck(c, new List<Card>()));
        }
        compagnionDiscard = new Deck(discards);

    }

    public void StartCombat(List<Compagnion> compagnions_, List<Enemy> enemies_)
    {
        compagnions = compagnions_;
        enemies = enemies_;
        List<Deck> cd = new List<Deck>();
        foreach (Compagnion c in compagnions)
        {
            cd.Add(c.GetDeck());
        }
        compagnionDeck = new Deck(cd);
        enemiesDeck = new List<Deck>();
        foreach (Enemy e in enemies)
        {
            e.GetDeck().Shuffle();
            enemiesDeck.Add(e.GetDeck());
        }
        compagnionDeck.Shuffle();

        List<Card> drawnCards = CombatManager.Instance.compagnionDeck.Draw();
        Hand.Instance.AddToHand(drawnCards);
        drawnCards = CombatManager.Instance.compagnionDeck.Draw();
        Hand.Instance.AddToHand(drawnCards);
        TurnManager.Instance.StartTurn();
    }


    public void AddEnemiesIntents()
    {
        foreach (Deck deck in enemiesDeck)
        {
            Card card = deck.Draw()[0];
            card.Play(compagnions[0]);
        }
    }

}