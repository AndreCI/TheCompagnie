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

        foreach(Compagnion c in compagnions)
        {
            c.Setup();
        }
        enemy.Setup();
        
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        globalMap.gameObject.SetActive(scene.name == "Overworld");
    }
}