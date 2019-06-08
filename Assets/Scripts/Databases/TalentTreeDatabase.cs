using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TalentTreeDatabase : ScriptableObject
{
    public List<TalentTreeDatabase> children;
    public List<TalenTreeFactory> factories;

    public void Setup()
    {
        if (children != null && children.Count > 0)
        {
            List<TalenTreeFactory> litems = new List<TalenTreeFactory>();
            int idOffset = 0;
            foreach (TalentTreeDatabase database in children)
            {
                database.Setup();
                foreach (TalenTreeFactory e in database.factories)
                {
                    e.ID += idOffset;
                    litems.Add(e);
                }
                idOffset += database.factories.Count;
            }
            factories = litems;
        }
        else
        {
            foreach (TalenTreeFactory ea in factories)
            {
                ea.Setup();
            }
        }
    }
    public TalenTreeFactory Get(CardDatabase.CARDCLASS type)
    {
        return factories.Find(x => x.classType == type);
    }

    public TalenTreeFactory Get(int index)
    {
        return factories[index];
    }


}