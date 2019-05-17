using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldMap: MonoBehaviour
{
    private static OverworldMap _instance;
    public static OverworldMap Instance { get => _instance; }
    public List<MapNode> nodes;

    public Sprite combat;
    public Sprite locked;
    public Sprite unknown;
    public Sprite visited;
    public Sprite current;
    public Sprite eventIcon;
    public Sprite boss;

    public GameObject eventWindow;
    public GameObject townWindow;
    public GameObject bossWindow;

    public bool noTavern = true;
    void Start()
    {
        if(_instance != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        _instance = this;
        nodes = new List<MapNode>(GetComponentsInChildren<MapNode>());
        GameObject.DontDestroyOnLoad(gameObject);
        PlayerInfos.Instance.currentPosition = nodes[0];
        OnEnable();
    }

    private void OnEnable()
    {
        foreach(MapNode n in nodes)
        {
           
            n.Setup(nodes);
        }
        UpdateNodes();
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.OVERWORLD);
    }

    public void UpdateNodes()
    {
        foreach(MapNode n in nodes)
        {
            n.UpdateNode();
        }
    }
    public void StartCombat()
    {
        SceneManager.LoadScene(2);
        gameObject.SetActive(false);
    }

    public void StartEvent()
    {
        eventWindow.SetActive(true);
        UpdateNodes();
    }

    public void StartTown()
    {
        townWindow.SetActive(true);
    }

    public void StartBoss()
    {
        PlayerInfos.Instance.readyForBoss = true;
        bossWindow.SetActive(true);
    }
}
