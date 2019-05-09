using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

[Serializable]
public class PlayerSettings
{
    public float timeSpeed;
    public float eventSpeed;
    public float themeVolume;


    private static PlayerSettings instance;
    public static PlayerSettings Instance
    {
        get
        {
            if(instance == null){ instance = new PlayerSettings(1f, 1f, 1f); }
            return instance;
        }
    }

    private PlayerSettings(float timeSpeed_ = 1f, float eventSpeed_ = 1f, float themeVolume_=1f)
    {
        timeSpeed = timeSpeed_;
        eventSpeed = eventSpeed_;
        themeVolume = themeVolume_;
    }

}