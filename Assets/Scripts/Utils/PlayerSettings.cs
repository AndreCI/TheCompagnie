using System;
using System.IO;
using UnityEngine;
[Serializable]
public class PlayerSettings
{
    public float timeSpeed;
    public float eventSpeed;
    public float themeVolume;
    public bool disableTutorial;


    private static PlayerSettings instance;
    public static PlayerSettings Instance
    {
        get
        {
            if(instance == null){ SetInstance(); }
            return instance;
        }
    }

    private static void SetInstance()
    {
        if (LoadFromDisk())
        {
            return;
        }
        instance = new PlayerSettings();
        instance.WriteToDisk();
    }

    private PlayerSettings(float timeSpeed_ = 1f, float eventSpeed_ = 1f, float themeVolume_=1f)
    {
        
        timeSpeed = timeSpeed_;
        eventSpeed = eventSpeed_;
        themeVolume = themeVolume_;
        disableTutorial = false;
    }

    private static bool LoadFromDisk()
    
        {
            string path = "PlayerSettings.json";
        if (File.Exists(path))
        {

            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                try
                {
                    instance = JsonUtility.FromJson<PlayerSettings>(json);
                }catch(ArgumentException e)
                {
                    return false;
                }
                return true;
            }
        }
        return false;
        }

    public void WriteToDisk()
    {
        string path = "PlayerSettings.json";
        string result = JsonUtility.ToJson(this);
        using(StreamWriter w = new StreamWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
        {
            w.WriteLine(result);
        }
    }

}