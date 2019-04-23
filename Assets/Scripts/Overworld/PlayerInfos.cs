using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class PlayerInfos : MonoBehaviour
{
    private static PlayerInfos _instance;
    public static PlayerInfos Instance { get => _instance; }

    public MapNode currentPosition;
    public List<Compagnion> compagnions;
    public Enemy enemy;

    private void Start()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        GameObject.DontDestroyOnLoad(gameObject);
        foreach(Compagnion c in compagnions)
        {
            c.Setup();
        }
        enemy.Setup();
    }
}