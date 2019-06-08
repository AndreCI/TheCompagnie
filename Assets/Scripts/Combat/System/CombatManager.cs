using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    private static CombatManager instance;
    public static CombatManager Instance { get => instance; }
    public List<UnitUI> playersUI;
    public List<UnitUI> enemiesUI;
    public WinWindow winWindow;
    public CombatStatusUI combaStatusUI;

    public List<Compagnion> compagnions;
    public CombatPartyDeck compagnionDeck;
    public CombatPartyDeck compagnionDiscard;
    public List<Enemy> enemies;
    public CombatPartyDeck enemiesDeck;
    public CombatPartyDeck enemiesDiscard;

    public Button deckButton;
    public Button discardButton;

    public SpriteRenderer back;
    public SpriteRenderer front;
    public SpriteRenderer darkest;

    public List<Sprite> backPossibilites;
    public List<Sprite> frontPossibilites;

    [HideInInspector] public Card HiddenCard;
    private int winXpValue;

    void Start()
    {
        instance = this;
        for (int i = 0; i < playersUI.Count; i++)
        {
            playersUI[i].gameObject.SetActive(true);
            if (i < PlayerInfos.Instance.compagnions.Count)
            {
                playersUI[i].SetInfos(PlayerInfos.Instance.compagnions[i]);
            }
            else
            {
                playersUI[i].Disable();
            }
        }
        back.sprite = backPossibilites.OrderBy(x => Utils.rdx.Next()).First();
        front.sprite = frontPossibilites.OrderBy(x => Utils.rdx.Next()).First();
        HiddenCard = new Card(null, PlayerInfos.Instance.cardDatabase.GetByName("Hidden"));
        StartCombat(PlayerInfos.Instance.compagnions, EnemyGenerator.Instance.GenerateEnemies());
        UnitSelector.Instance.Unselect();

    }


    public void StartCombat(List<Compagnion> compagnions_, List<Enemy> enemies_)
    {

        compagnions = compagnions_;
        enemies = enemies_;
        winXpValue = 0;
        foreach (Enemy e in enemies) {
            winXpValue += e.xpValue;
        }
        for (int i = 0; i < enemiesUI.Count; i++)
        {
            enemiesUI[i].gameObject.SetActive(true);
            if (i < enemies.Count)
            {
                enemiesUI[i].SetInfos(enemies[i]);
            }
            else
            {
                enemiesUI[i].Disable();
            }
        }
        compagnionDeck = PlayerInfos.Instance.persistentPartyDeck.GenerateCombatDeck(compagnions);
        enemiesDeck = new PersistentPartyDeck(enemies, enemies.Select(x => x.persistentDeck).ToList()).GenerateCombatDeck(enemies);

        compagnionDeck.Shuffle();
        enemiesDeck.Shuffle();

        compagnionDiscard = new CombatPartyDeck(compagnions, null);
        enemiesDiscard = new CombatPartyDeck(enemies, null);
        foreach(Compagnion c in compagnions)
        {
            c.CurrentAction = 0;
            c.talentTree.StartCombat();
        }

        TurnManager.Instance.StartFirstTurn();
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.COMBAT);

    }

    public void OnUnitDeath(Enemy unit)
    {
        TurnManager.Instance.RemoveAllEvent(unit);
         enemies.Remove(unit);
         
    }

    private bool IACardPlay(Card card, Enemy current)
    {
        if(card == null || card.manaCost > card.owner.CurrentMana || current.IsCurrentCardStatus(card))
        {
            return true;
        }
        bool returnValue = true;

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
                    card.Play(new List<Unit> { compagnions[Utils.rdx.Next(compagnions.Count)] });
                }
            }
            else if (card.potential_target == Card.POTENTIAL_TARGET.PARTY || card.potential_target == Card.POTENTIAL_TARGET.SELF)
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
        if(card.actionCost == 0) {  Debug.Log("what"); return false;}
        return returnValue;
    }

    public void AddEnemiesIntents()
    {
     //   List<Card> cards = enemiesDeck.DrawCards(owners:enemies);
        foreach(Enemy e in enemies)
        {
            //enemiesDeck.Shuffle();
            Card card = enemiesDeck.Draw(e as Unit);
            bool b = IACardPlay(card, e);
            int noPermanentLoopVerification = 0; //Rare case if a unit has no mana & all cards cost mana...
            while (!b && card != null && noPermanentLoopVerification < 100)
            {
                noPermanentLoopVerification += 1;
                card = enemiesDeck.Draw(e as Unit);
                b = IACardPlay(card, e);
            }
            if(card == null)
            {
                enemiesDeck.RenewDeck(e as Unit, e.persistentDeck.GenerateCombatDeck());
            }
            /*  if (card.manaCost > card.owner.CurrentMana)
              {
                  card = enemiesDeck.Redraw(new List<Card> { card })[0];
              }*/
            //IACardPlay(card);
        }

    }

    public void Win()
    {
        if (PlayerInfos.Instance.readyForBoss)
        {
            PlayerInfos.Instance.gameOver.SetActive(true);
            PlayerInfos.Instance.gameOverText.text = "Congratulation! You won! \n" +
                "`You recolted "+ PlayerInfos.Instance.CurrentShards.ToString()+ " shards!";
        }
        foreach(Compagnion c in compagnions)
        {
            foreach(CombatStatus cs in new List<UnitStatus>(c.CurrentStatus))
            {
                cs.CheckUpdate(forceRemove: true);
            }
            c.CurrentStatus = new List<CombatStatus>();
            c.talentTree.EndCombat();
        }

        winWindow.gameObject.SetActive(true);
        winWindow.Setup(winXpValue);
    }

    public void DisplayDeck()
    {
        PlayerInfos.Instance.unitsWindow.gameObject.SetActive(true);
        IEnumerable<Unit> selected = UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT);
        
       PlayerInfos.Instance.unitsWindow.deckDisplay.Setup(
            compagnionDeck.GetCards(selected.All(x => x.GetType() == typeof(Compagnion)) ? selected : null));
    }

    public void DisplayDiscard()
    {
        PlayerInfos.Instance.unitsWindow.gameObject.SetActive(true);
        IEnumerable<Unit> selected = UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT);

        PlayerInfos.Instance.unitsWindow.deckDisplay.Setup(
             compagnionDiscard.GetCards(selected.All(x => x.GetType() == typeof(Compagnion)) ? selected : null));
    }

    public UnitUI GetUnitUI(Unit u)
    {
        UnitUI s = playersUI.Find(x => x.unit == u);
        if (s == null)
        {
            s = enemiesUI.Find(x => x.unit == u);
        }
        return s;
    }

    public List<UnitUI> GetFriendsUnitUI(Unit original)
    {
        List<UnitUI> uis = new List<UnitUI>();
        UnitUI s = playersUI.Find(x => x.unit == original);
        if (s == null)
        {
            s = enemiesUI.Find(x => x.unit == original);
            uis = new List<UnitUI>(enemiesUI.Except(new List<UnitUI> { s }));
        }
        else
        {
            uis = new List<UnitUI>(playersUI.Except(new List<UnitUI> { s }));
        }
        return uis.FindAll(x => x.isActiveAndEnabled);
    }
}