using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

    public Intent intent;
    public Button endTurnButton;
    public Scrollbar time;
    public CardUI cardPlaceHolder;
    public Transform enemiesIntentsTransform;
    public Transform compIntentsTransform;
    public List<VerticalLayoutGroup> enemiesIntents;
    public List<VerticalLayoutGroup> compIntents;
    private List<Queue<CombatEvent>> registeredEvents;
    private static TurnManager instance;
    private List<GeneralUtils.SUBJECT_TRIGGER> triggers;
   // private List<List<Observer>> observers;
    public static TurnManager Instance { get => instance; }

    public delegate void Notify(GeneralUtils.SUBJECT_TRIGGER trigger);
    public static event Notify NotifyAll;

    void Start()
    {
        registeredEvents = new List<Queue<CombatEvent>>();
        for (int i = 0; i < 10; i++)
        {
            registeredEvents.Add(new Queue<CombatEvent>());
        }
        triggers = new List<GeneralUtils.SUBJECT_TRIGGER>();
      //  observers = new List<List<Observer>>();
        instance = this;
        enemiesIntents = new List<VerticalLayoutGroup>(enemiesIntentsTransform.GetComponentsInChildren<VerticalLayoutGroup>());
        compIntents = new List<VerticalLayoutGroup>(compIntentsTransform.GetComponentsInChildren<VerticalLayoutGroup>());


    }

    public void AddCombatEvent(CombatEvent newEvent)
    {

        registeredEvents[newEvent.timeIndex].Enqueue(newEvent);
        Intent newIntent = Instantiate(intent);
        List<VerticalLayoutGroup> intentsLayout = (newEvent.source.GetType() == typeof(Compagnion)) ? compIntents : enemiesIntents;
        newIntent.transform.SetParent(intentsLayout[newEvent.timeIndex].transform);
        newIntent.card = newEvent.cardSource;
        newIntent.UI = cardPlaceHolder;
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
        time.value = 0;
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
       
        for(int i = (int)time.value; i < time.numberOfSteps; i++)
        {
            yield return StartCoroutine(PerformTimeStep(i));
        }
        yield return new WaitForSeconds(0.1f);
        CombatManager.Instance.AddEnemiesIntents();
        StartTurn();
    }

    private IEnumerator PerformTimeStep(int i)
    {
        time.value = (float)i/(float)time.numberOfSteps;
        NotifyAll?.Invoke(GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK);
        while(registeredEvents[i].Count > 0)
        {
            CombatEvent currentEvent = registeredEvents[i].Dequeue();
            currentEvent.PerformEffect();
            Destroy(currentEvent.intent.gameObject, 0.5f);
        }
        yield return new WaitForSeconds(0.3f);
    }

}
