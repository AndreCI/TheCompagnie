using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
 
    public static bool timeIsRunning;
    private float fixedTimeMin = 10f;
    private float fixedTimePerEvent = 1f;
    [HideInInspector]
    public float timeMin;
    [HideInInspector]
    public float timePerEvent;

    public Intent intent;
    public Button endTurnButton;
    public GameObject timeHolder;
    public List<TimeStep> timeSteps;
    public CardUI cardPlaceHolder;
    public List<GridLayoutGroup> enemiesIntents;
    public List<GridLayoutGroup> compIntents;
    public float currentDuration = 0f;
    private List<List<CombatEvent>> registeredEvents;
    private static TurnManager instance;
    private List<GeneralUtils.SUBJECT_TRIGGER> triggers;
   // private List<List<Observer>> observers;
    public static TurnManager Instance { get => instance; }

    public delegate void Notify(GeneralUtils.SUBJECT_TRIGGER trigger);
    public static event Notify NotifyAll;

    void Start()
    {
        endTurnButton.interactable = true;
        timeIsRunning = false;
        timeMin = fixedTimeMin * PlayerInfos.Instance.settings.timeSpeed;
        timePerEvent = fixedTimePerEvent * PlayerInfos.Instance.settings.eventSpeed;
        timeSteps = new List<TimeStep>(timeHolder.GetComponentsInChildren<TimeStep>());
        timeSteps.Sort((x, y) => x.index.CompareTo(y.index));
        registeredEvents = new List<List<CombatEvent>>();
        for (int i = 0; i < 10; i++)
        {
            registeredEvents.Add(new List<CombatEvent>());
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

        registeredEvents[newEvent.timeIndex].Add(newEvent);
        Intent newIntent = Instantiate(intent);
        List<GridLayoutGroup> intentsLayout = (newEvent.source.GetType() == typeof(Compagnion)) ? compIntents : enemiesIntents;
        newIntent.transform.SetParent(intentsLayout[newEvent.timeIndex].transform);
        newIntent.Setup(newEvent.cardSource, cardPlaceHolder, newEvent);
        newEvent.intent = newIntent;
       
    }
    
    public void RemoveAllEvent(Unit removed)
    {
        foreach (List<CombatEvent> revents in registeredEvents)
        {
            foreach(CombatEvent e in revents)
            {
                if(e.source == removed)
                {
                    Destroy(e.intent.gameObject);
                }
            }
            revents.RemoveAll(x => x.source == removed);
        }
    }
    
    /*Turn slicing:
     * 1) Effect Phase
     * 2) Draw Phase
     * 3) Play Phase
     * 4) blabla TODO
     */
    public void StartTurn()
    {
        if (CombatManager.Instance.compagnions.All(x=>x.currentHealth <= 0)) { PlayerInfos.Instance.Quit(); }
        if(CombatManager.Instance.enemies.Count == 0) { CombatManager.Instance.Win(); return; }
        timeMin = fixedTimeMin * PlayerInfos.Instance.settings.timeSpeed;
        timePerEvent = fixedTimePerEvent * PlayerInfos.Instance.settings.eventSpeed;
        foreach (UnitUI ui in CombatManager.Instance.enemiesUI)
        {
            ui.statusAnimator.ResetAll();
        }
        foreach (UnitUI ui in CombatManager.Instance.playersUI)
        {
            ui.statusAnimator.ResetAll();
        }
        NotifyAll?.Invoke(GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN);
        foreach(TimeStep timeStep in timeSteps)
        {
            timeStep.Activate(timeMin/((float)timeSteps.Count * 2), backwards:true);
        }
        List<Card> drawnCards = CombatManager.Instance.compagnionDeck.DrawCards();
        Hand.Instance.AddToHand(drawnCards);
        foreach(Compagnion u in CombatManager.Instance.compagnions)
        {
            u.GainMana(2);
            u.GainAction(1);
            Hand.Instance.SetLock(false);
        }

        timeIsRunning = false;
        endTurnButton.interactable = true;
    }

    public void EndTurn()
    {
        if (!timeIsRunning)
        {
            endTurnButton.interactable = false;
            timeIsRunning = true;
            Hand.Instance.SetLock(true);
            StartCoroutine(PerformTime());
        }
    }

    private IEnumerator PerformTime()
    {
       
        for(int i = 0; i < timeSteps.Count; i++)
        {
            yield return StartCoroutine(PerformTimeStep(i));
        }
        yield return new WaitForSeconds(0.5f);
        CombatManager.Instance.AddEnemiesIntents();
        yield return new WaitForSeconds(0.5f);
        StartTurn();
    }

    private IEnumerator PerformTimeStep(int i)
    {

        timeMin = fixedTimeMin * PlayerInfos.Instance.settings.timeSpeed;
        timePerEvent = fixedTimePerEvent * PlayerInfos.Instance.settings.eventSpeed;

        int eventNbr = registeredEvents[i].Count;
        currentDuration = Mathf.Max(eventNbr * timePerEvent, timeMin / (float)timeSteps.Count);
        NotifyAll?.Invoke(GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK);
        timeSteps[i].Activate(currentDuration);
        while(registeredEvents[i].Count > 0)
        {
            CombatEvent currentEvent = registeredEvents[i][registeredEvents[i].Count - 1];
            registeredEvents[i].RemoveAt(registeredEvents[i].Count - 1);
            currentEvent.PerformEffect();
            currentEvent.intent.Trigger(timePerEvent);
            yield return new WaitForSeconds(timePerEvent);
        }
        yield return new WaitForSeconds(timeMin/(float)timeSteps.Count);
    }

}
