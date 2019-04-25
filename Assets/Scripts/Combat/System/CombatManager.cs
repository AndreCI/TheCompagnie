using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    private static CombatManager instance;
    public static CombatManager Instance { get => instance; }
    public UnitUI player;
    public UnitUI enemy;

    public List<Compagnion> compagnions;
    public CombatPartyDeck compagnionDeck;
    public CombatPartyDeck compagnionDiscard;
    public List<Enemy> enemies;
    public CombatPartyDeck enemiesDeck;
    public CombatPartyDeck enemiesDiscard;

    void Start()
    {
        instance = this;
        player.SetInfos(PlayerInfos.Instance.compagnions[0]);
        enemy.SetInfos(PlayerInfos.Instance.enemy);
        StartCombat(PlayerInfos.Instance.compagnions, new List<Enemy> { PlayerInfos.Instance.enemy });
    }

    public void StartCombat(List<Compagnion> compagnions_, List<Enemy> enemies_)
    {
        compagnions = compagnions_;
        enemies = enemies_;
        compagnionDeck = PlayerInfos.Instance.persistentPartyDeck.GenerateCombatDeck(compagnions);
        List<CombatUnitDeck> cd = new List<CombatUnitDeck>();
        foreach (Enemy e in enemies)
        {
            cd.Add(e.GetNewDeck());
        }
        enemiesDeck = new CombatPartyDeck(enemies, cd);

        compagnionDeck.Shuffle();
        enemiesDeck.Shuffle();

        compagnionDiscard = new CombatPartyDeck(compagnions, null);
        enemiesDiscard = new CombatPartyDeck(enemies, null);

        List<Card> drawnCards = compagnionDeck.DrawCards(new List<int> { 3 }, compagnions);
        Hand.Instance.AddToHand(drawnCards);
        TurnManager.Instance.StartTurn();
    }

    public void OnUnitDeath(Unit unit)
    {
        if (enemies.Count == 1)
        {
            Win();
        }
    }

    public void AddEnemiesIntents()
    {
        List<Card> cards = enemiesDeck.DrawCards();
        foreach (Card card in cards)
        {
            
            card.Play(compagnions[0]);
        }
    }

    public void Win()
    {
        compagnions[0].TakeDamage(3);
        SceneManager.LoadScene(0);
    }

    public void DisplayDeck()
    {
        PlayerInfos.Instance.cardsDisplay.gameObject.SetActive(true);
        PlayerInfos.Instance.cardsDisplay.DisplayDeck(compagnionDeck);
    }

    public void DisplayDiscard()
    {
        PlayerInfos.Instance.cardsDisplay.gameObject.SetActive(true);
        PlayerInfos.Instance.cardsDisplay.DisplayDeck(compagnionDiscard);

    }
}