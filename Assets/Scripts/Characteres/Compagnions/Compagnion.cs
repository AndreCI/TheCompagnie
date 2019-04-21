using System;
using UnityEngine;
using UnityEngine.UI;

public class Compagnion : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    private Text jack;

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
