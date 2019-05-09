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

    public PlayerSettings settings;
    public EffectDatabase effectDatabase;
    public CardDatabase cardDatabase;
    public CompagnionsDatabase compagnionsDatabase;
    public AnimationClipDatabase animationDatabase;

    public MapNode currentPosition;
    public List<Compagnion> compagnions;
    public PartyMenu unitsWindow;
    public PersistentPartyDeck persistentPartyDeck;

    public OverworldMap globalMap;
    public GameObject gameOver;

    void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;


        GameObject.DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        settings = PlayerSettings.Instance;
        List<Compagnion> originals = new List<Compagnion> { compagnionsDatabase.Get(0) };
        compagnions = new List<Compagnion>();

        cardDatabase.Setup();
        animationDatabase.Setup();
        List<PersistentUnitDeck> decks = new List<PersistentUnitDeck>();
        foreach(Compagnion o in originals)
        {
            Compagnion c = o.Setup() as Compagnion;
            compagnions.Add(c);
            decks.Add(c.persistentDeck);
        }

        persistentPartyDeck = new PersistentPartyDeck(compagnions, decks);


        unitsWindow?.gameObject.SetActive(false);
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (globalMap != null)
        {
            globalMap.gameObject.SetActive(scene.name == "Overworld");
        }
    }

    public void Quit()
    {
        Application.Quit();
        

    }
}