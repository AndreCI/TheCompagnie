using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class Compagnion : Unit
{
    public Leveling level;

    public override void Setup()
    {
        base.Setup();
        level = new Leveling();
    }
}
