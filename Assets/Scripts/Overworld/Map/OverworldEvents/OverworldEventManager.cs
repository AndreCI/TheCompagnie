using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

public class OverworldEventManager : MonoBehaviour
{
    private static OverworldEventManager instance;
    public static OverworldEventManager Instance { get =>instance; }

    public OverworldEventDatabase allEvents;
    public List<OverworldEvent> pastEvents;
    public int currentIndex;
    void Start()
    {
        if(instance != null) { Destroy(gameObject); return; }
        instance = this;
        allEvents.Setup();
        currentIndex = 0;
    }

    public OverworldEvent GetFixedEvent(string eventName)
    {
        if(eventName == "") { Debug.Log("ISSUES WITH EVENT: NAME IS NONE"); }
        return allEvents.Get(eventName);
    }


    public OverworldEvent GetARandomEvent()
    {
        List<OverworldEvent> candidates = allEvents.GetEvents(OverworldEventDatabase.EVENT_TYPE.RANDOM_UNIQUE).events.Except(pastEvents).ToList();
        candidates.AddRange(allEvents.GetEvents(OverworldEventDatabase.EVENT_TYPE.RANDOM_REPEATABLE).events);
        OverworldEvent e = Shuffle(candidates).First();
        pastEvents.Add(e);
        return e;
    }
    public OverworldEvent GetNextEvent()
    {
        return allEvents.Get(currentIndex);
    }

    public static List<OverworldEvent> Shuffle(List<OverworldEvent> iterable)
    {
        int n = iterable.Count;
        while (n > 1)
        {
            n--;
            int k = Utils.rdx.Next(n + 1);
            OverworldEvent value = iterable[k];
            iterable[k] = iterable[n];
            iterable[n] = value;
        }
        return iterable;
    }
}