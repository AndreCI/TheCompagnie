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
    public int speed;

    private Deck deck;
    private Text jack;

    public abstract Deck GetDeck();

    void Start()
    {
        jack = this.GetComponentInChildren<Text>();
        jack.text = maxHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        jack.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        if (currentHealth <= 0)
        {
            Debug.Log("Dead");
        }
    }
}
