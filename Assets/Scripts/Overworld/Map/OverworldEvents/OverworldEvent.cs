using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class OverworldEvent
{
    [Header("Basic event data")]
    public string eventName;
    public List<string> description;
    public Sprite background;
    public Sprite frontground;

    [Header("Effects data")]
    public List<string> buttonsTexts;
    public List<ControlOverworldEffect> effects;

    public OverworldEventDatabase.EVENT_TYPE dtype;
    public int ID;
    public string GetDescription()
    {
        return description.Aggregate((x, y) => x + "\n" + y); ;
    }

    public void Setup(OverworldEventDatabase.EVENT_TYPE t)
    {
        dtype = t;
        foreach(ControlOverworldEffect e in effects)
        {
            e.Setup(this, effects.IndexOf(e));
        }
    }

    public void UpdateData()
    {
        throw new NotImplementedException();
    }
    
    public void ButtonClick(int index)
    {
        effects[index].Perform();
    }
}
