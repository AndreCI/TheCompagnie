using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : UICardDropZone
{
    public int currentHealth;
    public int maxHealth;
    public int currentMana;
    public int maxMana;
    public int currentAction;
    public int maxAction;
    public int speed;
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider actionSlider;

    private Deck deck;
    public List<CombatStatus> currentStatus;

    public abstract Deck GetDeck();

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentAction = maxAction/2 - 1;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        healthSlider.value = 1 - (float)currentHealth / (float)maxHealth;
        manaSlider.value = 1 - (float)currentMana / (float)maxMana;
        actionSlider.value = 1 - (float)currentAction / (float)maxAction;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateInfo();
        if (currentHealth <= 0)
        {
            Debug.Log("Dead");
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        UpdateInfo();
    }
}
