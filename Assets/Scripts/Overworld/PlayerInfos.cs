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
    public UIDisplay cardsDisplay;

    public MapNode currentPosition;
    public List<Compagnion> compagnions;
    public Enemy enemy;

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
        enemy.Setup();
        List<Card> cards = new List<Card>();
        for (int i = 0; i < 15; i++)
        {
            cards.Add(new Card(enemy, collection.cards[0]));
        }
        cards.Add(new Card(enemy, collection.cards[1]));
        enemy.deck = new Deck(enemy, cards);
        foreach(Compagnion c in compagnions)
        {
            c.Setup();

            cards = new List<Card>();
            for (int i = 0; i < 3; i++)
            {
                cards.Add(new Card(c, collection.cards[0]));
            }
            cards.Add(new Card(c, collection.cards[1]));
            c.deck = new Deck(c, cards);
        }



    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (globalMap != null)
        {
            globalMap.gameObject.SetActive(scene.name == "Overworld");
        }
    }
}