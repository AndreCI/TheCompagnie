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
    public int maxHealth;
    public int maxMana;
    public int maxAction;
    private int currentHealth;
    private int currentMana;
    private int currentAction;
    public Sprite combatSprite;

    public delegate void InfoUpdate();
    public event InfoUpdate NotifyUpdate;

    public virtual void Setup()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentAction = maxAction;

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
    public int speed;

    public PersistentUnitDeck persistentDeck;

    public List<CombatStatus> currentStatus;

    public CombatUnitDeck GetNewDeck()
    {
        return persistentDeck.GenerateCombatDeck();
    }


    public void TakeDamage(int amount)
    {
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
