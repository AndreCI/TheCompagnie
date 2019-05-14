using System;
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
    public bool addComp = false;
    public bool tavern = false;
    public void Update()
    {
        if (waiting)
        {
            currentTime += Time.deltaTime;
            slider.value = currentTime / waitingTime;
            if(currentTime>= waitingTime)
            {
                if (!tavern)
                {
                    AddCompagnion();
                wait.interactable = true;
                }
                else
                {
                    FullHeal();
                }
                waiting = false;
                slider.value = 0;
                wait.interactable = true;
                leave.interactable = true;
            }
        }
    }

    public void Activate()
    {
        wait.interactable = true;
        leave.interactable = true;
        currentTime = 0;
        waiting = true;
    }

    public void AddCompagnion()
    {
        if (!addComp)
        {
            addComp = true;
            if (!tavern)
                PlayerInfos.Instance.AddCompagnion(PlayerInfos.Instance.compagnionsDatabase.Get(1));
        }
    }

    private void FullHeal()
    {
        foreach (Compagnion c in PlayerInfos.Instance.compagnions)
        {
            c.CurrentHealth = c.maxHealth;
            c.CurrentMana = c.maxMana;
        }
    }
}