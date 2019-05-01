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
    public CardDatabase.CARDCLASS availableCards;
    public int maxHealth;
    public int maxMana;
    public int maxAction;
    public int currentHealth;
    private int currentMana;
    private int currentAction;
    public Sprite combatSprite;
    public Sprite portraitSprite;
    public string unitName;

    public delegate void InfoUpdate();
    public event InfoUpdate NotifyUpdate;

    public Leveling level;

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
                    if(baseVal < 0) { baseVal = 0; }
                }
            }return baseVal;
        } }
    public virtual void Setup()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentAction = maxAction;
        level = new Leveling();

    }

    public int CurrentHealth {  get => currentHealth; set {
            currentHealth = value;
            if (currentHealth > maxHealth) { currentHealth = maxHealth; }
            if (currentHealth < 0) { currentHealth = 0;}
            NotifyUpdate();
        } }
    public int CurrentMana
    {
        get => currentMana; set
        {
            currentMana = value;
            if (currentMana > maxMana) { currentMana = maxMana; }
            if (currentMana < 0) { currentMana = 0; }
            NotifyUpdate();
        }
    }
    public int CurrentAction
    {
        get => currentAction; set
        {
            currentAction = value;
            if (currentAction > maxAction) { currentAction = maxAction; }
            if (currentAction < 0) { currentAction = 0; }
            NotifyUpdate();
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
    }

    public void AddStatus(CombatStatus status)
    {
        if (currentStatus == null)
        {
            currentStatus = new List<CombatStatus>();
        }
        currentStatus.Add(status);
        NotifyUpdate();
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
        NotifyUpdate();
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
            if(cs.status == CombatStatus.STATUS.PARRY)
            {
                amount = 0;
                cs.value -= 1;
                cs.CheckUpdate();
                return;
            }
            if(cs.status == CombatStatus.STATUS.BLOCK)
            {
                cs.value -= amount;
                if(cs.value < 0) { amount = -1 * cs.value; }
                else { amount = 0; }
                cs.CheckUpdate();
            }
        }
        if (amount > 0)
        {
            CurrentHealth -= amount;
            if (currentHealth <= 0)
            {
                Debug.Log("Dead");
            }
        }
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
}
