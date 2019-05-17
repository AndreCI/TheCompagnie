using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class Unit
{
    public enum UNIT_SPECIFIC_TRIGGER { ATTACKS, DAMAGE_BLOCKED, ATTACKED, DAMAGE_DEALT, DAMAGE_INSTANCE_END};
    public CardDatabase.CARDCLASS availableCards;
    public int maxHealth;
    public int maxMana;
    public int maxAction;
    public int currentHealth;
    private int currentTalentPoints = 0;
    private int currentMana;
    private int currentAction;
    public Sprite combatSprite;
    public Sprite portraitSprite;
    public string unitName;

    public delegate void InfoUpdate();
    public event InfoUpdate NotifyUpdate;

    public delegate void SpecificUpdateDelegate(UNIT_SPECIFIC_TRIGGER trigger);
    public event SpecificUpdateDelegate SpecificUpdate;

    public Leveling level;
    public int id;
    public static int currentId = 0;

    public void TriggerSpecificUpdate(UNIT_SPECIFIC_TRIGGER trigger)
    {
        SpecificUpdate?.Invoke(trigger);
    }
    public int currentBlock
    {
        get
        {
            int baseVal = 0;
            if (currentStatus != null)
            {
                foreach (CombatStatus s in currentStatus)
                {
                    if (s.status == CombatStatus.STATUS.BLOCK)
                    {
                        baseVal += s.value;
                    }
                }
            }
            return baseVal;
        }
    }

    public int currentStrength { get
        {
            int baseVal = 0;
            foreach(CombatStatus s in currentStatus)
            {
                if(s.status == CombatStatus.STATUS.BUFF_STR)
                {
                    baseVal += s.value;
                }else if(s.status == CombatStatus.STATUS.REDUCE_STR)
                {
                    baseVal -= s.value;
                //    if(baseVal < 0) { baseVal = 0; }
                }
            }return baseVal;
        } }
    public virtual Unit Setup()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentAction = maxAction;
        Unit copy = GeneralUtils.Copy<Unit>(this);
        return copy;

    }

    public int CurrentHealth {  get => currentHealth; set {
            CombatManager.Instance?.GetUnitUI(this)?.portraitInfos?.PlayNotificationText((value - currentHealth).ToString(), (value - currentHealth) < 0 ? Color.red : Color.green);

            currentHealth = value;
            if (currentHealth > maxHealth) { currentHealth = maxHealth; }
            if (currentHealth < 0) { currentHealth = 0;}
            NotifyUpdate?.Invoke();
        }
    }
    public int CurrentMana
    {
        get => currentMana; set
        {

            CombatManager.Instance?.GetUnitUI(this)?.portraitInfos?.PlayNotificationText((value - currentMana).ToString(), Color.blue);
            currentMana = value;
            if (currentMana > maxMana) { currentMana = maxMana; }
            if (currentMana < 0) { currentMana = 0; }
            NotifyUpdate?.Invoke();
        }
    }
    public int CurrentAction
    {
        get => currentAction; set
        {
            
            currentAction = value;
            if (currentAction > maxAction) { currentAction = maxAction; }
            if (currentAction < 0) { currentAction = 0; }
            NotifyUpdate?.Invoke();
        }
    }
    public int CurrentTalentPoints
    { get => currentTalentPoints; set
        {
            if(value < 0) { return; }
            currentTalentPoints = value;
            NotifyUpdate?.Invoke();
        }
    }
    public int baseSpeed;
    public int currentSpeed;

    public PersistentUnitDeck persistentDeck;

    private List<CombatStatus> currentStatus;

       public List<CombatStatus> CurrentStatus
    {
        get
        {
            if(currentStatus == null)
            {
                currentStatus = new List<CombatStatus>();
            }
            return currentStatus;
        }
        set { currentStatus = value; }
    }

    public void AddStatus(CombatStatus status)
    {
        if (currentStatus == null)
        {
            currentStatus = new List<CombatStatus>();
        }
        currentStatus.Add(status);
        NotifyUpdate?.Invoke();
    }

    public void RemoveStatus(CombatStatus status)
    {
        if (currentStatus == null)
        {
            currentStatus = new List<CombatStatus>();
        }
        else
        {
            currentStatus.Remove(status);
        }
        NotifyUpdate?.Invoke();
    }
    public CombatUnitDeck GetNewDeck()
    {
        return persistentDeck.GenerateCombatDeck();
    }


    public virtual void TakeDamage(int amount)
    {
        foreach(CombatStatus cs in CurrentStatus.FindAll(x => x.status == CombatStatus.STATUS.PARRY ||
        x.status == CombatStatus.STATUS.BLOCK))
        {
            if(cs.status == CombatStatus.STATUS.BLOCK)
            {
                cs.value -= amount;
                if(cs.value < 0) { amount = -1 * cs.value; cs.value = 0; }
                else
                {
                    TriggerSpecificUpdate(UNIT_SPECIFIC_TRIGGER.DAMAGE_BLOCKED);
                    CombatManager.Instance?.GetUnitUI(this).portraitInfos.PlayNotificationText(amount.ToString(), Color.gray);
                    amount = 0; }
                cs.CheckUpdate(forceAnimation:true);
            }

            if (cs.status == CombatStatus.STATUS.PARRY)
            {
                amount = 0;
                cs.value -= 1;
                cs.CheckUpdate(forceAnimation: true);

                CombatManager.Instance?.GetUnitUI(this).portraitInfos.PlayNotificationText("Parry", Color.grey);
            }
        }
        if (amount > 0)
        {
            TriggerSpecificUpdate(UNIT_SPECIFIC_TRIGGER.DAMAGE_DEALT);
            CurrentHealth -= amount;
            if (currentHealth <= 0)
            {
                Debug.Log("Dead");
            }
        }
        TriggerSpecificUpdate(UNIT_SPECIFIC_TRIGGER.DAMAGE_INSTANCE_END);
    }

    public void Heal(int amount)
    {
        if (amount > 0)
        {
            CurrentHealth += amount;
        }
    }

    public void GainMana(int amount)
    {
        CurrentMana += amount;
    }
    public void GainAction(int amount)
    {
        CurrentAction += amount;
    }

    public void GainXp(int amount)
    {
        level.GainXP(amount);
        NotifyUpdate?.Invoke();
    }
}
