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
    public UnitUI p2;
    public UnitUI enemy;
    public UnitUI e2;

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
        p2.SetInfos(PlayerInfos.Instance.compagnions[1]);
        enemy.SetInfos(PlayerInfos.Instance.enemies[0]);
        e2.SetInfos(PlayerInfos.Instance.enemies[1]);

        StartCombat(PlayerInfos.Instance.compagnions, PlayerInfos.Instance.enemies);
    }

    public void StartCombat(List<Compagnion> compagnions_, List<Enemy> enemies_)
    {
        compagnions = compagnions_;
        enemies = enemies_;
        compagnionDeck = PlayerInfos.Instance.persistentPartyDeck.GenerateCombatDeck(compagnions);
        enemiesDeck = PlayerInfos.Instance.persistentEnemyPartyDeck.GenerateCombatDeck(enemies);

        compagnionDeck.Shuffle();
        enemiesDeck.Shuffle();

        compagnionDiscard = new CombatPartyDeck(compagnions, null);
        enemiesDiscard = new CombatPartyDeck(enemies, null);

        List<Card> drawnCards = compagnionDeck.DrawCards(new List<int> { 2, 2 }, compagnions);
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
            
            card.Play(new List<Unit> { compagnions[0] });
        }
    }

    public void Win()
    {
        compagnions[0].TakeDamage(3);
        SceneManager.LoadScene(0);
    }

    public void DisplayDeck()
    {
        PlayerInfos.Instance.deckMenu.gameObject.SetActive(true);
        PlayerInfos.Instance.deckMenu.SetInfos(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT),
            compagnionDeck.GetCards(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT)));
    }

    public void DisplayDiscard()
    {
        PlayerInfos.Instance.deckMenu.gameObject.SetActive(true);
        PlayerInfos.Instance.deckMenu.SetInfos(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT),
            compagnionDiscard.GetCards(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT)));
    }
}