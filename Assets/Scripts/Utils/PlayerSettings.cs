using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[Serializable]
public class PlayerSettings
{
    public float timeSpeed;
    public float eventSpeed;
    public float themeVolume;
    public bool disableTutorial;

    [NonSerialized]
    public bool[][] unlockedClasses;


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
        disableTutorial = true;
        unlockedClasses = new bool[3][];
        unlockedClasses[0] = new bool[3] { true, false, false };
        unlockedClasses[1] = new bool[3] { false, false, false };
        unlockedClasses[2] = new bool[3] { false, false, false };
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
            }
        }
        if(instance == null)
        {
            return false;
        }
        instance.unlockedClasses = new bool[3][];
        instance.unlockedClasses[0] = new bool[3];
        instance.unlockedClasses[1] = new bool[3];
        instance.unlockedClasses[2] = new bool[3];
        path = "ola.json";
        if (File.Exists(path))
        {

            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                int index = 0;
                foreach (string s in json.Split('!')) { 
               
                    //string json = r.ReadToEnd();
                    try
                    {
                        instance.unlockedClasses[index] = GeneralUtils.FromJson<bool>(s);// JsonUtility.FromJson<PlayerSettings>(json);
                    }
                    catch (ArgumentException e)
                    {
                        return false;
                    }
                    index++;

                }
                return true;
            }
        }
        return false;
        }

    public void WriteToDisk()
    {
        string path = "PlayerSettings.json";
        string result = JsonUtility.ToJson(this);// GeneralUtils.ToJson(unlockedClasses, true);
        using(StreamWriter w = new StreamWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
        {
            w.WriteLine(result);
        }
        path = "ola.json";
        result = GeneralUtils.ToJson(unlockedClasses[0], false);
        result += '!';
        result += GeneralUtils.ToJson(unlockedClasses[1], false);
        result += '!';
        result += GeneralUtils.ToJson(unlockedClasses[2], false);
        using (StreamWriter w = new StreamWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
        {
            w.WriteLine(result);
        }
    }

    public void Unlock(CardDatabase.CARDCLASS type, CardDatabase.SUBCARDCLASS deck = CardDatabase.SUBCARDCLASS.TYPE1)
    {
        int classIndex = -1;
        switch(type){
            case CardDatabase.CARDCLASS.PALADIN:
                classIndex = 0;
                break;
            case CardDatabase.CARDCLASS.ELEM:
                classIndex = 1;
                break;
            case CardDatabase.CARDCLASS.HUNTER:
                classIndex = 2;
                break;
        }
        int index = -1;
        switch (deck)
        {
            case CardDatabase.SUBCARDCLASS.TYPE1:
                index = 0;
                break;
            case CardDatabase.SUBCARDCLASS.TYPE2:
                index = 1;
                break;
            case CardDatabase.SUBCARDCLASS.TYPE3:
                index = 2;
                break;
        }
     
            unlockedClasses[classIndex][index] = true;
        
        WriteToDisk();
    }
}