using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;


public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 500f;
 

    private float maxHealth = 500f;
    

    

        void Start()
    {
        
       
        
    }

       void Update()
    {
        
  
    }
    public float GetHitPoints()
    {
        return hitPoints;
    }

    public void Heal(float amount)
    {
        hitPoints += amount;
        Debug.Log("Player healed. Current HP: " + hitPoints);
    }

    public void GetDamage(float damage)
    {
        hitPoints -= damage;
        Debug.Log("Player took damage. Current HP: " + hitPoints);

        if (hitPoints <= 0)
        {
            Debug.Log("Player is dead");
            Die();
        }
    }

      private void UpdatePostProcessingEffects()
    {
      
    }

    private void Die()
    {
    
    }
}