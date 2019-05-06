using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

[Serializable]
public class PlayerSettings
{
    public float timeSpeed;
    public float eventSpeed;
    public float themeVolume;

    public PlayerSettings potentialInstance;

    public PlayerSettings(float timeSpeed_ = 1f, float eventSpeed_ = 1f, float themeVolume_=1f)
    {
        timeSpeed = timeSpeed_;
        eventSpeed = eventSpeed_;
        themeVolume = themeVolume_;
    }

}