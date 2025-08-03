using UnityEngine;
using UnityEngine.UI;

public class HealthRing : MonoBehaviour
{
    public Image ringHealthBar; 
    public PlayerHealth playerHealth; 
    public Gradient healthColorGradient; 

    private float maxHealth; 

    private void Start()
    {
        maxHealth = playerHealth.GetHitPoints(); 
    }

    private void Update()
    {
        
        float currentHealth = playerHealth.GetHitPoints();

        
        UpdateRingHealthBar(currentHealth);
    }

    
    void UpdateRingHealthBar(float currentHealth)
    {
        float healthRatio = currentHealth / maxHealth; 

        
        ringHealthBar.fillAmount = healthRatio;

        
        ringHealthBar.color = healthColorGradient.Evaluate(healthRatio);
    }
}