﻿using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
 
    public Button endTurnButton;
    public Scrollbar time;

    private List<Queue<CombatEvent>> registeredEvents;
    private static TurnManager instance;
    public static TurnManager Instance { get => instance; }

    void Start()
    {
        registeredEvents = new List<Queue<CombatEvent>>();
        for (int i = 0; i < 10; i++)
        {
            registeredEvents.Add(new Queue<CombatEvent>());
        }
        instance = this;
    }

    public void AddCombatEvent(CombatEvent newEvent)
    {

        registeredEvents[newEvent.timeIndex].Enqueue(newEvent);
       
    }

    /*Turn slicing:
     * 1) Effect Phase
     * 2) Draw Phase
     * 3) Play Phase
     * 4) blabla TODO
     */
    public void StartTurn()
    {
        time.value = 0;
        List<Card> drawnCards = CombatManager.Instance.compagnionDeck.Draw();
        Hand.Instance.AddToHand(drawnCards);
    }

    public void EndTurn()
    {
        StartCoroutine(PerformTime());
    }

    private IEnumerator PerformTime()
    {
       
        for(int i = (int)time.value; i < time.numberOfSteps; i++)
        {
            yield return StartCoroutine(PerformTimeStep(i));
        }
        yield return new WaitForSeconds(0.1f);
        StartTurn();
    }

    private IEnumerator PerformTimeStep(int i)
    {
        time.value = (float)i/(float)time.numberOfSteps;
        while(registeredEvents[i].Count > 0)
        {
            registeredEvents[i].Dequeue().PerformEffect();
            yield return new WaitForSeconds(0.8f);
        }
        yield return new WaitForSeconds(0.3f);
    } 

}
