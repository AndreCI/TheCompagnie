using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OverworldEventDatabase : ScriptableObject
{
    public enum EVENT_TYPE { GLOBAL, RANDOM_UNIQUE, RANDOM_REPEATABLE, DETERMINISTIC}
    public EVENT_TYPE databaseType;
    public List<OverworldEventDatabase> children;
    public List<OverworldEvent> events;

    public void Setup()
    {
        if (children != null && children.Count > 0)
        {
            List<OverworldEvent> litems = new List<OverworldEvent>();
            int idOffset = 0;
            foreach (OverworldEventDatabase database in children)
            {
                database.Setup();
                foreach (OverworldEvent e in database.events)
                {
                    e.ID += idOffset;
                    litems.Add(e);
                }
                idOffset += database.events.Count;
            }
            events = litems;
        }
        else
        {
            foreach (OverworldEvent ea in events)
            {
                ea.Setup(databaseType);
            }
        }
    }

    public OverworldEventDatabase GetEvents(EVENT_TYPE t)
    {
        return children.Find(x => x.databaseType == t);
    }

    public OverworldEvent Get(string name)
    {
        return GeneralUtils.Copy<OverworldEvent>(events.Find(x => x.eventName == name));
    }
    public OverworldEvent Get(int index)
    {
        return GeneralUtils.Copy<OverworldEvent>(events[index]);
    }


}