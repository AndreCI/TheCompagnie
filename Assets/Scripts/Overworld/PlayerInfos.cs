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
    private int currentShards;
    public int CurrentShards { get => currentShards; set
        {
            ShardDisplay.Instance.AddShards(value - currentShards);
            currentShards = value;
        } }
    public List<Compagnion> compagnions;
    public PartyMenu unitsWindow;
    public GameObject exitMenu;
    public PersistentPartyDeck persistentPartyDeck;

    public OverworldMap globalMap;
    public GameObject gameOver;
    public Text gameOverText;
    public Text versionText; //UI Text
    public bool readyForBoss = false; //Summon boss with enemy gen

    void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        currentShards = 0;
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitMenu.SetActive(!exitMenu.activeSelf);
        }else if (Input.GetKeyDown(KeyCode.W))
        {
            unitsWindow.gameObject.SetActive(!unitsWindow.gameObject.activeSelf);
        }
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