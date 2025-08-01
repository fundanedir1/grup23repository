using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 500f;
    private float maxHealth = 500f;

    [SerializeField] private HealthBar healthBar; 

    void Start()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth((int)maxHealth);
            healthBar.SetHealth((int)hitPoints);
        }
    }

    public float GetHitPoints()
    {
        return hitPoints;
    }

    public void Heal(float amount)
    {
        hitPoints = Mathf.Clamp(hitPoints + amount, 0f, maxHealth);
        Debug.Log("Player healed. Current HP: " + hitPoints);

        if (healthBar != null)
            healthBar.SetHealth((int)hitPoints);
    }

    public void GetDamage(float damage)
    {
        hitPoints -= damage;
        Debug.Log("Player took damage. Current HP: " + hitPoints);

        if (healthBar != null)
            healthBar.SetHealth((int)hitPoints);

        if (hitPoints <= 0)
        {
            Debug.Log("Player is dead");
            Die();
        }
    }

    private void Die()
    {
        SceneManager.LoadScene("GameOverScene");
    }
}