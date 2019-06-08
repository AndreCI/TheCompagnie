using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

    public int turnNumber = 0;
    public static bool timeIsRunning;
    private float fixedTimeMin = 10f;
    private float fixedTimePerEvent = 1f;
    [HideInInspector]
    public float timeMin;
    [HideInInspector]
    public float timePerEvent;
    [HideInInspector]
    public float currentEventTimeDuration;

    public Intent intent;
    public Button endTurnButton;
    public GameObject timeHolder;
    public List<TimeStep> timeSteps;
    public CardUI cardPlaceHolder;
    public List<StretchyGridLayoutGroup> enemiesIntents;
    public List<StretchyGridLayoutGroup> compIntents;
    public float currentDuration = 0f;
    [HideInInspector] public CombatEvent currentEvent;
    private List<List<CombatEvent>> registeredEvents;
    private List<CombatEvent> futurEvents;
    private List<CombatEvent> phantomEvents;
    private static TurnManager instance;
    private List<GeneralUtils.SUBJECT_TRIGGER> triggers;
    public int currentIndex;
    public static TurnManager Instance { get => instance; }

    public delegate void Notify(GeneralUtils.SUBJECT_TRIGGER trigger);
    public static event Notify NotifyAll;

    void Awake()
    {
        turnNumber = 0;
        currentIndex = 0;
        endTurnButton.interactable = true;
        timeIsRunning = false;
        timeMin = fixedTimeMin * PlayerInfos.Instance.settings.timeSpeed;
        timePerEvent = fixedTimePerEvent * PlayerInfos.Instance.settings.eventSpeed;
        timeSteps = new List<TimeStep>(timeHolder.GetComponentsInChildren<TimeStep>());
        timeSteps.Sort((x, y) => x.index.CompareTo(y.index));
        phantomEvents = new List<CombatEvent>();   
        futurEvents = new List<CombatEvent>();
        registeredEvents = new List<List<CombatEvent>>();
        for (int i = 0; i < 10; i++)
        {
            registeredEvents.Add(new List<CombatEvent>());
        }
        triggers = new List<GeneralUtils.SUBJECT_TRIGGER>();

        instance = this;
        enemiesIntents = new List<StretchyGridLayoutGroup>();
        compIntents = new List<StretchyGridLayoutGroup>();
        foreach (TimeStep timeStep in timeSteps)
        {
            enemiesIntents.Add(timeStep.enemiesIntents);
            compIntents.Add(timeStep.compIntents);

        }

    }

    public void StartFirstTurn()
    {
        NotifyAll?.Invoke(GeneralUtils.SUBJECT_TRIGGER.START_OF_COMBAT);
        List<Card> drawnCards = CombatManager.Instance.compagnionDeck.DrawCards(new List<int>{ 2, 2});
        List<Card> returned = Hand.Instance.AddToHand(drawnCards);
        CombatManager.Instance.compagnionDeck.AddCards(returned);
        StartTurn();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }else if (Input.GetKeyDown(KeyCode.X))
        {
            CombatManager.Instance.DisplayDeck();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            CombatManager.Instance.DisplayDiscard();
        }
    }

    public List<CombatEvent> GetCurrentEvents(bool addFuture = false)
    {
        List<CombatEvent> eventsR = new List<CombatEvent>();
        foreach(List<CombatEvent> e in registeredEvents)
        {
            eventsR.AddRange(e);
        }
        if (addFuture)
        {
            eventsR.AddRange(futurEvents);
        }
        return eventsR;
    }

    public CombatEvent GetNextCombatEvent(Unit u = null)
    {
        for(int i =currentIndex; i < registeredEvents.Count; i++)
        {
            foreach(CombatEvent e in registeredEvents[i])
            {
                if(!e.channel && (u == null || e.source == u))
                {
                    return e;
                }
            }
        }
        return futurEvents.Find(x => x.timeIndex <= futurEvents.Min(y => y.timeIndex));
    }

    public void RemoveCombatEvent(CombatEvent e)
    {
        registeredEvents[e.timeIndex].Remove(e);
        Destroy(e.intent.gameObject);
    }

    public void MoveCombatEvent(CombatEvent e, int amount)
    {
        if (e.channel && e.nextChannelEvent != null)
        {
            MoveCombatEvent(e.nextChannelEvent, amount);
        }
        if (futurEvents.Contains(e))
        {
            e.timeIndex += amount;
            return;
        }
        registeredEvents[e.timeIndex].Remove(e);
        e.timeIndex += amount;
        if(e.timeIndex>= timeSteps.Count) {
            futurEvents.Add(e);
            if (e.intent != null)
            {
                Destroy(e.intent.gameObject);
                e.intent = null;
            }
            return;
        }
        if (e.intent != null)
        {
            //e.intent.setToDestroy = false;
            //e.intent.Trigger(timePerEvent);
            registeredEvents[e.timeIndex].Add(e);
            List<StretchyGridLayoutGroup> intentsLayout = (e.source.GetType() == typeof(Compagnion)) ? compIntents : enemiesIntents;
            e.intent.transform.SetParent(intentsLayout[e.timeIndex].transform);
            e.intent.Setup(e.cardSource, cardPlaceHolder, e, !(e.source.GetType() == typeof(Compagnion)));
        }
    }

    public void RemovePhantomEvents()
    {
        foreach(CombatEvent phantom in phantomEvents)
        {

            Destroy(phantom.intent.gameObject);
        }
        phantomEvents = new List<CombatEvent>();
    }
    public void AddPhantomCombatEvent(Card c)
    {
        int delay = Mathf.Max(0,c.delay + c.owner.currentSpeed);
        bool future = delay >= timeSteps.Count;
        int timeindex =delay % timeSteps.Count;
        
        if (!c.channel)
        {
            CombatEvent phantom = new CombatEvent(c.owner, new List<Unit>(), timeindex, new List<CombatEffect>(), c, c.set, false);
            AddPhantom(phantom, future);
        }
        else
        {
            int channelValue = c.channel ? Mathf.Max(0, c.channelLenght + (c.owner == null ? 0 : c.owner.CurrentChannelValue)) : 0;
            for (int i = 0; i < channelValue; i++)
            {
                if((i+1) * c.manaCost <= c.owner.CurrentMana)
                {
                    if(timeindex + i >= timeSteps.Count) { future = true;  timeindex -= timeSteps.Count; }
                    CombatEvent phantom = new CombatEvent(c.owner, new List<Unit>(), timeindex + i, new List<CombatEffect>(), c, c.set, true);
                    AddPhantom(phantom, future);
                }
            }
        }
    }

    private void AddPhantom(CombatEvent phantom, bool futurePhantom = false)
    {
        phantomEvents.Add(phantom);
        Intent phantomIntent = Instantiate(intent);
        List<StretchyGridLayoutGroup> intentsLayout = (phantom.source.GetType() == typeof(Compagnion)) ? compIntents : enemiesIntents;
        phantomIntent.transform.SetParent(intentsLayout[phantom.timeIndex].transform);
        phantomIntent.Setup(phantom.cardSource, cardPlaceHolder, phantom, !(phantom.source.GetType() == typeof(Compagnion)), _phantom: true, futurePhantom);
        phantom.intent = phantomIntent;
    }

    public void AddCombatEvent(CombatEvent newEvent)
    {
        if(newEvent.timeIndex >= timeSteps.Count)
        {
            futurEvents.Add(newEvent);
            return;
        }
        registeredEvents[newEvent.timeIndex].Add(newEvent);
        Intent newIntent = Instantiate(intent);
        List<StretchyGridLayoutGroup> intentsLayout = (newEvent.source.GetType() == typeof(Compagnion)) ? compIntents : enemiesIntents;
        newIntent.transform.SetParent(intentsLayout[newEvent.timeIndex].transform);
        newIntent.Setup(newEvent.cardSource, cardPlaceHolder, newEvent, !(newEvent.source.GetType() == typeof(Compagnion)));
        newEvent.intent = newIntent;
       
    }
    
    public void RemoveAllEvent(Unit removed)
    {
        foreach (List<CombatEvent> revents in registeredEvents)
        {
            foreach(CombatEvent e in revents)
            {
                if(e != null && e.intent != null && (e.source == removed || e.targets.Contains(removed)))
                {
                    Destroy(e.intent.gameObject);
                }
            }
            revents.RemoveAll(x => x.source == removed);
        }
    }

    private void BeforeTurnStart()
    {

        foreach (UnitUI u in CombatManager.Instance.enemiesUI.FindAll(x => x!= null && x.unit!=null && x.unit.CurrentHealth <= 0))
        {
            (u.unit as Enemy).CheckDeath();
            u.gameObject.SetActive(false);
        }

        currentIndex = 0;
        if (CombatManager.Instance.compagnions.All(x => x.CurrentHealth <= 0)) { PlayerInfos.Instance.Quit(); }
        if (CombatManager.Instance.enemies.Count == 0) { CombatManager.Instance.Win(); return; }
        timeMin = fixedTimeMin * PlayerInfos.Instance.settings.timeSpeed;
        timePerEvent = fixedTimePerEvent * PlayerInfos.Instance.settings.eventSpeed;

        foreach (CombatEvent e in futurEvents)
        {
            e.timeIndex -= timeSteps.Count;
        }
        foreach (CombatEvent e in futurEvents.FindAll(x => x.timeIndex <= timeSteps.Count))
        {
            AddCombatEvent(e);
        }
        futurEvents.RemoveAll(x => x.timeIndex <= timeSteps.Count);


    }
    /*Turn slicing:
     * 1) Effect Phase
     * 2) Draw Phase
     * 3) Play Phase
     * 4) blabla TODO
     */
    public void StartTurn()
    {  
        turnNumber += 1;
        NotifyAll?.Invoke(GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN);

        if (CombatManager.Instance.enemies.Count == 0) { CombatManager.Instance.Win(); return; }

        foreach (TimeStep timeStep in timeSteps)
        {
            timeStep.Activate(timeMin/((float)timeSteps.Count * 2), backwards:true);
        }

        List<Card> drawnCards = CombatManager.Instance.compagnionDeck.DrawCards();
        List<Card> returned = Hand.Instance.AddToHand(drawnCards);
        CombatManager.Instance.compagnionDeck.AddCards(returned);
        foreach(Compagnion u in CombatManager.Instance.compagnions)
        {
            u.GainMana(u.CurrentManaRegen);
            u.GainAction(1);
            Hand.Instance.SetLock(false);
        }
        CombatManager.Instance.AddEnemiesIntents();

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
            NotifyAll?.Invoke(GeneralUtils.SUBJECT_TRIGGER.START_OF_TIME);
            StartCoroutine(PerformTime());
        }
    }

    private IEnumerator PerformTime()
    {
       
        for(int i = 0; i < timeSteps.Count; i++)
        {
            currentIndex = i;
            yield return StartCoroutine(PerformTimeStep(i));
        }
        BeforeTurnStart();
        StartTurn();
    }

 

    private IEnumerator PerformTimeStep(int i)
    {

        timeMin = fixedTimeMin * PlayerInfos.Instance.settings.timeSpeed;
        timePerEvent = fixedTimePerEvent * PlayerInfos.Instance.settings.eventSpeed;

        int eventNbr = registeredEvents[i].Count;
        if (eventNbr > 0)
        {
            currentDuration = Mathf.Max(Enumerable.Sum(registeredEvents[i].Select(x => x.GetTime(timePerEvent))), timeMin / (float)timeSteps.Count);
        }
        else
        {
            currentDuration = timeMin / (float)timeSteps.Count;
        }
        //  currentDuration = Mathf.Max(eventNbr * timePerEvent, timeMin / (float)timeSteps.Count);
        NotifyAll?.Invoke(GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK);
        timeSteps[i].Activate(currentDuration);
        yield return StartCoroutine(PerformEvents(registeredEvents[i].FindAll(x => x.source.GetType() == typeof(Enemy)), i));
        yield return StartCoroutine(PerformEvents(registeredEvents[i].FindAll(x => !x.channel), i));
        yield return StartCoroutine(PerformEvents(registeredEvents[i], i));
        yield return new WaitForSeconds(timeMin/(float)timeSteps.Count);
    }

    private IEnumerator PerformEvents(IEnumerable<CombatEvent> events_, int i)
    {
        List<CombatEvent> events = new List<CombatEvent>(events_);
        while (events.Count > 0)
        {
            CombatEvent currentEvent = events[events.Count - 1];
            registeredEvents[i].Remove(currentEvent);
            events.Remove(currentEvent);
            this.currentEvent = currentEvent;
            currentEventTimeDuration = currentEvent.GetTime(timePerEvent);
            currentEvent.PerformEffect(timePerEvent);
            yield return new WaitForSeconds(currentEvent.GetTime(timePerEvent));
        }
    }
}
