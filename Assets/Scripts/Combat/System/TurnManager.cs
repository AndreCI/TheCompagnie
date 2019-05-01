﻿using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public float timeMin = 2f;
    public float timePerEvent = 0.5f;

    public Intent intent;
    public Button endTurnButton;
    public GameObject timeHolder;
    public List<TimeStep> timeSteps;
    public CardUI cardPlaceHolder;
    public List<GridLayoutGroup> enemiesIntents;
    public List<GridLayoutGroup> compIntents;
    public float currentDuration = 0f;
    private List<Queue<CombatEvent>> registeredEvents;
    private static TurnManager instance;
    private List<GeneralUtils.SUBJECT_TRIGGER> triggers;
   // private List<List<Observer>> observers;
    public static TurnManager Instance { get => instance; }

    public delegate void Notify(GeneralUtils.SUBJECT_TRIGGER trigger);
    public static event Notify NotifyAll;

    void Start()
    {
        timeSteps = new List<TimeStep>(timeHolder.GetComponentsInChildren<TimeStep>());
        timeSteps.Sort((x, y) => x.index.CompareTo(y.index));
        registeredEvents = new List<Queue<CombatEvent>>();
        for (int i = 0; i < 10; i++)
        {
            registeredEvents.Add(new Queue<CombatEvent>());
        }
        triggers = new List<GeneralUtils.SUBJECT_TRIGGER>();
      //  observers = new List<List<Observer>>();
        instance = this;
        enemiesIntents = new List<GridLayoutGroup>();
        compIntents = new List<GridLayoutGroup>();
        foreach (TimeStep timeStep in timeSteps)
        {
            enemiesIntents.Add(timeStep.enemiesIntents);
            compIntents.Add(timeStep.compIntents);

        }

    }

    public void AddCombatEvent(CombatEvent newEvent)
    {

        registeredEvents[newEvent.timeIndex].Enqueue(newEvent);
        Intent newIntent = Instantiate(intent);
        List<GridLayoutGroup> intentsLayout = (newEvent.source.GetType() == typeof(Compagnion)) ? compIntents : enemiesIntents;
        newIntent.transform.SetParent(intentsLayout[newEvent.timeIndex].transform);
        newIntent.Setup(newEvent.cardSource, cardPlaceHolder, newEvent);
        newEvent.intent = newIntent;
       
    }

    /*Turn slicing:
     * 1) Effect Phase
     * 2) Draw Phase
     * 3) Play Phase
     * 4) blabla TODO
     */
    public void StartTurn()
    {
        NotifyAll?.Invoke(GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN);
        foreach(TimeStep timeStep in timeSteps)
        {
            timeStep.Reset();
        }
        List<Card> drawnCards = CombatManager.Instance.compagnionDeck.DrawCards();
        Hand.Instance.AddToHand(drawnCards);
        foreach(Compagnion u in CombatManager.Instance.compagnions)
        {
            u.GainMana(2);
            u.GainAction(1);
            Hand.Instance.SetLock(false);
        }
    }

    public void EndTurn()
    {
        Hand.Instance.SetLock(true);
        StartCoroutine(PerformTime());
    }

    private IEnumerator PerformTime()
    {
       
        for(int i = 0; i < timeSteps.Count; i++)
        {
            yield return StartCoroutine(PerformTimeStep(i));
        }
        yield return new WaitForSeconds(0.1f);
        CombatManager.Instance.AddEnemiesIntents();
        StartTurn();
    }

    private IEnumerator PerformTimeStep(int i)
    {
        int eventNbr = registeredEvents[i].Count;
        currentDuration = Mathf.Max(eventNbr * timePerEvent, timeMin / (float)timeSteps.Count);
        NotifyAll?.Invoke(GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK);
        timeSteps[i].Activate(currentDuration);
        while(registeredEvents[i].Count > 0)
        {
            CombatEvent currentEvent = registeredEvents[i].Dequeue();
            currentEvent.PerformEffect();
            currentEvent.intent.Trigger(timePerEvent);
            yield return new WaitForSeconds(timePerEvent);
        }
        yield return new WaitForSeconds(timeMin/(float)timeSteps.Count);
    }

}
