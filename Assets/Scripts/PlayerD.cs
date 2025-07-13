using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerD : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    public PlayerAudioManager audioManager;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            TakeDamage(10);
        }   

    }
    void TakeDamage(int damage)
    {
        int oldHealth = currentHealth; 

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (audioManager != null && currentHealth < oldHealth)
            audioManager.OnHealthDecrease();
    }
}
