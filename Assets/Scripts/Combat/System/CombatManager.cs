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
    public UnitUI enemyUI1;
    public UnitUI enemyUI2;
    public CombatStatusUI combaStatusUI;
    public WinWindow winWindow;

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
        enemyUI1.SetInfos(PlayerInfos.Instance.enemies[0]);
        enemyUI2.SetInfos(PlayerInfos.Instance.enemies[1]);

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

    public void OnUnitDeath(Enemy unit)
    {
        if (enemies.Count == 1)
        {
            Win();
        }
        else
        {
            enemies.Remove(unit);
            if(enemyUI1.unit == unit){
                enemyUI1.gameObject.SetActive(false);
            }else if (enemyUI2.unit == unit)
            {
                enemyUI2.gameObject.SetActive(false);
            }
        }
    }

    public void AddEnemiesIntents()
    {
        List<Card> cards = enemiesDeck.DrawCards();
        for(int i = 0; i < cards.Count; i ++)
        {
            Card card = cards[i];
            if (card.manaCost <= card.owner.CurrentMana)
            {
                card = enemiesDeck.Redraw(new List<Card> { card })[0];
            }
            if (enemies.Contains((Enemy)card.owner))
            {
                if (card.potential_target == Card.POTENTIAL_TARGET.ENEMIES)
                {
                    if (card.multipleTarget)
                    {
                        card.Play(new List<Unit>(compagnions));

                    }
                    else
                    {
                        card.Play(new List<Unit> { compagnions[(new System.Random()).Next(compagnions.Count)] });
                    }
                }
                else if (card.potential_target == Card.POTENTIAL_TARGET.PARTY)
                {
                    if (card.multipleTarget)
                    {
                        card.Play(new List<Unit>(enemies));

                    }
                    else
                    {
                        card.Play(new List<Unit> { card.owner });
                    }
                }
            }
        }

    }

    public void Win()
    {
        winWindow.gameObject.SetActive(true);
        winWindow.Setup(24);
    }

    public void DisplayDeck()
    {
        PlayerInfos.Instance.unitsWindow.gameObject.SetActive(true);
        PlayerInfos.Instance.unitsWindow.deckDisplay.Setup(
            compagnionDeck.GetCards(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT)));
    }

    public void DisplayDiscard()
    {
        PlayerInfos.Instance.unitsWindow.gameObject.SetActive(true);
        PlayerInfos.Instance.unitsWindow.deckDisplay.Setup(
            compagnionDiscard.GetCards(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT)));
    }
}