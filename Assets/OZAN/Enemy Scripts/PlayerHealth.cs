using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 500f;
    private float maxHealth = 500f;

    [SerializeField] private HealthBar healthBar; 

    void Start()
    {
        if (healthBar == null)
        {
            healthBar = FindObjectOfType<HealthBar>();
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth((int)maxHealth);
            healthBar.SetHealth((int)hitPoints);
            Debug.Log("✅ HealthBar bulundu ve ayarlandı!");
        }
        else
        {
            Debug.LogError("❌ HealthBar bulunamadı! Sahneye HealthBar ekleyin.");
        }
    }

    public float GetHitPoints()
    {
        return hitPoints;
    }

    public void Heal(float amount)
    {
        hitPoints = Mathf.Clamp(hitPoints + amount, 0f, maxHealth);
        Debug.Log("✅ Player healed. Current HP: " + hitPoints);

        if (healthBar != null)
        {
            healthBar.SetHealth((int)hitPoints);
        }
    }

    public void GetDamage(float damage)
    {
        hitPoints = Mathf.Clamp(hitPoints - damage, 0f, maxHealth);
        Debug.Log($"💥 Player took {damage} damage. Current HP: {hitPoints}");

        // UI güncelle
        if (healthBar != null)
        {
            healthBar.SetHealth((int)hitPoints);
            Debug.Log($"🔄 HealthBar güncellendi: {hitPoints}");
        }
        else
        {
            Debug.LogWarning("⚠️ HealthBar null! UI güncellenemedi.");
        }
        
        if (hitPoints <= 0)
        {
            Debug.Log("💀 Player is dead - calling Die()");
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("💀 Die() method çağrıldı");
        
        gameObject.SetActive(false);
        
        if (Application.CanStreamedLevelBeLoaded("GameOverScene"))
        {
            Debug.Log("🔄 GameOverScene yükleniyor...");
            SceneManager.LoadScene("GameOverScene");
        }
        else
        {
            Debug.LogError("❌ GameOverScene bulunamadı! Build Settings'e ekleyin.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Test için context menu
    [ContextMenu("Test Damage")]
    private void TestDamage()
    {
        GetDamage(50f);
    }

    [ContextMenu("Test Death")]
    private void TestDeath()
    {
        GetDamage(hitPoints);
    }
}