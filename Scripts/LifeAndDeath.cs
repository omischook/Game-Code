using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeAndDeath : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    public PlayerCombat playerCombat;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    public void TakeDamage(int damage)
    {
        if (playerCombat.blocking == false)
        {
            currentHealth -= damage;
        }
        else
        {
            currentHealth -= 0;
        }
            

        healthBar.setHealth(currentHealth);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Spikes")
        {
            TakeDamage(1);
        }
    }
}
