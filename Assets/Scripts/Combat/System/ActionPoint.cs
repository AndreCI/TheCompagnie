﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ActionPoint : MonoBehaviour
{

    public Image image;
    public int index;

    public Unit unit;



    public ActionPoint Setup(Unit u)
    {
        if(u.maxAction <= index)
        {
            Destroy(gameObject);
            return null;
        }
        u.NotifyUpdate += UpdateInfos;
        unit = u;
        return this;
    }

    public void UpdateInfos()
    {
        image.gameObject.SetActive(unit.CurrentAction > index);
    }
}
