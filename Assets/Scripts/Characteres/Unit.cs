using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public int currentMana;
    public int maxMana;
    public int speed;

    private Deck deck;
    private Text jack;

    public abstract Deck GetDeck();

    void Start()
    {
        jack = this.GetComponentInChildren<Text>();
        jack.text = maxHealth.ToString() + "/" + maxHealth.ToString() + "\n" + maxMana + "/" + maxMana;
    }

    public void UpdateInfo()
    {
        jack.text = currentHealth.ToString() + "/" + maxHealth.ToString() + "\n" + currentMana + "/" + maxMana;
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
