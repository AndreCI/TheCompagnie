using System.Collections.Generic;
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
    public Text versionText;

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
        compagnions = new List<Compagnion>();

        cardDatabase.Setup();
        animationDatabase.Setup();

        persistentPartyDeck = new PersistentPartyDeck();

        AddCompagnion(compagnionsDatabase.Get(0));

      //  AddCompagnion(compagnionsDatabase.Get(1));
        unitsWindow?.gameObject.SetActive(false);
        versionText.text = "v" + TutorialManager.versionNumber;
    }

    public void AddCompagnion(Compagnion newComp)
    {        
        Compagnion c = newComp.Setup() as Compagnion;
        compagnions.Add(c);
        persistentPartyDeck.AddDeck(c, c.persistentDeck);
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

    public void OpenFeedback()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSdT7eLixwCpVmH7yRivF5uhUOrrlG5AMcv9gIkEjU8gDCVWFg/viewform");

    }
    

}