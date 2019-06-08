using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CombatStatusData
{
    public string Name;
    public CombatStatus.STATUS status;
    public Sprite icon;
    public Sprite highlightedSprite;
    public EffectAnimation mainAnimation;
    public EffectAnimation finalAnimation;


    public CombatStatusData(string name, CombatStatus.STATUS s, Sprite i, Sprite hi)
    {
        status = s;
        icon = i;
        highlightedSprite = hi;
        Name = name;
    }


}