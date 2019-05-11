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
    public CombatStatus.STATUS status;
    public Sprite icon;
    public Sprite highlightedSprite;
}