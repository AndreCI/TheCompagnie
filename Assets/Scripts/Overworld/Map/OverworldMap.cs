using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

public class OverworldMap: MonoBehaviour
{
    private static OverworldMap _instance;
    public static OverworldMap Instance { get => _instance; }
    public List<MapNode> nodes;

    void Start()
    {
        if(_instance != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        _instance = this;
        nodes = new List<MapNode>(GetComponentsInChildren<MapNode>());
        foreach(MapNode node in nodes)
        {
            node.nextNodes = nodes;
        }
        GameObject.DontDestroyOnLoad(gameObject);
    }
}
