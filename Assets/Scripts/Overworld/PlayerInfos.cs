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
    public CardCollection collection;
    public EffectDatabase effectDatabase;
    public CardDatabase cardDatabase;
    public CompagnionsDatabase compagnionsDatabase;
    public AnimationClipDatabase animationDatabase;

    public MapNode currentPosition;
    public List<Compagnion> compagnions;
    public PartyMenu unitsWindow;
    public PersistentPartyDeck persistentPartyDeck;

    public OverworldMap globalMap;

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
        settings = new PlayerSettings(1f);
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