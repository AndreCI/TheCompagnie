using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfos : MonoBehaviour
{
    private static PlayerInfos _instance;
    public static PlayerInfos Instance { get => _instance; }

    public CardCollection collection;
    public EffectDatabase effectDatabase;
    public CardDatabase cardDatabase;
    public UIDisplay cardsDisplay;

    public MapNode currentPosition;
    public List<Compagnion> compagnions;
    public PartyMenu unitsWindow;
    public PersistentPartyDeck persistentPartyDeck;
    public PersistentPartyDeck persistentEnemyPartyDeck;

    public List<Enemy> enemies;

    private OverworldMap globalMap;

    void Start()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        GameObject.DontDestroyOnLoad(gameObject);
        globalMap = OverworldMap.Instance;
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        List<PersistentUnitDeck> decks = new List<PersistentUnitDeck>();

        foreach (Enemy enemy in enemies)
        {
            enemy.Setup();
            List<Card> cards = new List<Card>();
            foreach (Card card in cardDatabase.GetCardsFromClass(enemy.availableCards))
            {
                cards.Add(new Card(enemy, card));
            }
            enemy.persistentDeck = new PersistentUnitDeck(cards);
            decks.Add(enemy.persistentDeck);
        }
        persistentEnemyPartyDeck = new PersistentPartyDeck(enemies, decks);

        cardDatabase.Setup();
        decks = new List<PersistentUnitDeck>();
        foreach(Compagnion c in compagnions)
        {
            c.Setup();
            List<Card> cards = new List<Card>();
            foreach(Card card in cardDatabase.GetCardsFromClass(c.availableCards))
            {
                cards.Add(new Card(c, card));
            }
           // cards.Add(new Card(c, collection.cards[5]));
            c.persistentDeck = new PersistentUnitDeck(cards);
            decks.Add(c.persistentDeck);
        }

        persistentPartyDeck = new PersistentPartyDeck(compagnions, decks);



    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (globalMap != null)
        {
            globalMap.gameObject.SetActive(scene.name == "Overworld");
        }
    }
}