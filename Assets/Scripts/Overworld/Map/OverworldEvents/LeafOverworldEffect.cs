using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class LeafOverworldEffect : OverworldEffect
{
    public enum TYPE
    {
        HEALTH_MODIFY,
        MANA_MODIFY,
        SHARD_MODIFY,
        LEAVE,
        CHANGE_POSITION_TO_PREVIOUS,
        ADD_COMPAGNION,
        FIGHT, BOSS_FIGHT,
        TOGGLE,
        SET_THIS_AS_FIXED_EVENT,
        VOID_MODIFY,
        PLACEHOLDER_MODIFY,
        MAX_HEALTH_MODIFY,
        MAX_MANA_MODIFY
    }


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
            case TYPE.MAX_HEALTH_MODIFY:
                foreach (Compagnion c in PlayerInfos.Instance.compagnions)
                {
                    if (c.maxHealth + amount <= 0) { amount = 1 - c.maxHealth; }
                    c.maxHealth += amount;
                }
                return;
            case TYPE.MAX_MANA_MODIFY:
                foreach (Compagnion c in PlayerInfos.Instance.compagnions)
                {
                    if (c.maxMana + amount <= 0) { amount = -c.maxMana; }
                    c.maxMana += amount;
                }
                return;
            case TYPE.PLACEHOLDER_MODIFY:
                if (amount > 0)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        foreach (Compagnion c in PlayerInfos.Instance.compagnions)
                        {
                            PlayerInfos.Instance.persistentPartyDeck.AddCardSlot(c);
                        }
                    }
                }
                else {
                    for (int i = 0; i < -amount; i++)
                    {
                        foreach (Compagnion c in PlayerInfos.Instance.compagnions)
                        {
                            if (c.persistentDeck.GetCardSlots() > 0)
                            {
                                c.persistentDeck.RemoveCardSlot();
                            }
                        }
                    }
                }
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
                PlayerSettings.Instance.Unlock(PlayerInfos.Instance.compagnions.Last().availableCards);
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