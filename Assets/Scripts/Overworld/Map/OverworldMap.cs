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
    public GameObject eventWindow;
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
        int i = 0;
        foreach(MapNode n in nodes)
        {
            n.nextNodes = new List<MapNode>();
            if (i > 0)
            {
                n.nextNodes.Add(nodes[i - 1]);
            }
            if(i < nodes.Count - 1)
            {
                n.nextNodes.Add(nodes[i + 1]);
            }
            i++;
            n.Setup();
        }
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.OVERWORLD);
    }

    public void StartCombat()
    {
        SceneManager.LoadScene(2);
        gameObject.SetActive(false);
    }

    public void StartEvent()
    {
        eventWindow.SetActive(true);
        OnEnable();
    }
}
