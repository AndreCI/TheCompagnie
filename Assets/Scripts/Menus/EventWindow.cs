﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class EventWindow : MonoBehaviour
{
    public Slider slider;
    public Button wait;
    public Button leave;

    public bool waiting;
    public float waitingTime;
    public float currentTime;

    public void Update()
    {
        if (waiting)
        {
            currentTime += Time.deltaTime;
            slider.value = currentTime / waitingTime;
            if(currentTime>= waitingTime)
            {
                waiting = false;
                slider.value = 0;
                wait.interactable = true;
                leave.interactable = true;
            }
        }
    }

    public void Activate()
    {
        wait.interactable = false;
        leave.interactable = false;
        currentTime = 0;
        waiting = true;
    }
}