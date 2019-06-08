using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class LeafOverworldEffect : OverworldEffect
{
    public enum TYPE {
        HEALTH_MODIFY,
        MANA_MODIFY,
        SHARD_MODIFY,
        LEAVE,
        CHANGE_POSITION_TO_PREVIOUS,
        ADD_COMPAGNION,
        FIGHT, BOSS_FIGHT,
        TOGGLE,
        SET_THIS_AS_FIXED_EVENT,
    VOID_MODIFY}

    public TYPE type;
    public int amount;

    [NonSerialized] private OverworldEvent parentEvent;
    private int childNumber;

    public override void Setup(OverworldEvent parent, int child)
    {
        parentEvent = parent;
        childNumber = child;
    }
    public override void Perform()
    {
        switch (type)
        {
            case TYPE.HEALTH_MODIFY:
                foreach(Compagnion c in PlayerInfos.Instance.compagnions)
                {
                    if(c.CurrentHealth + amount <= 0) { amount = 1 - c.CurrentHealth ; }
                    c.CurrentHealth += amount;
                }
                return;
            case TYPE.VOID_MODIFY:
                foreach (Compagnion c in PlayerInfos.Instance.compagnions)
                {
                    c.CurrentVoidPoints += amount;
                }
                return;
            case TYPE.MANA_MODIFY:
                foreach (Compagnion c in PlayerInfos.Instance.compagnions)
                {
                    
                    c.CurrentMana += amount;
                }
                return;
            case TYPE.SHARD_MODIFY:
                PlayerInfos.Instance.CurrentShards += amount;
                return;
            case TYPE.LEAVE:
                EventWindow.Instance.gameObject.SetActive(false);
                return;
            case TYPE.CHANGE_POSITION_TO_PREVIOUS:
                PlayerInfos.Instance.currentPosition.visited = false;
                MapNode newPos = PlayerInfos.Instance.previousPosition;
                PlayerInfos.Instance.currentPosition = newPos;
                OverworldMap.Instance.UpdateNodes();
                return;

            case TYPE.ADD_COMPAGNION:
                PlayerInfos.Instance.AddCompagnion(PlayerInfos.Instance.compagnionsDatabase.Get(amount));
                return;
            case TYPE.FIGHT:
                OverworldMap.Instance.StartCombat();
                return;
            case TYPE.BOSS_FIGHT:
                PlayerInfos.Instance.readyForBoss = true;
                OverworldMap.Instance.StartCombat();
                return;
            case TYPE.TOGGLE:
                EventWindow.Instance.options[childNumber].interactable = amount>0;
                return;
            case TYPE.SET_THIS_AS_FIXED_EVENT:
                PlayerInfos.Instance.currentPosition.fixedEvent = parentEvent;
                PlayerInfos.Instance.currentPosition.type = MapNode.NODETYPE.FIXED_EVENT;
                return;
        }
        throw new NotImplementedException();
    }
}