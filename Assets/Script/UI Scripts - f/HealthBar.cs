using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText; 

    private void Start()
    {
        if (healthSlider == null)
        {
            healthSlider = GetComponent<Slider>();
        }

        if (healthSlider == null)
        {
            Debug.LogError("❌ HealthBar: Slider component bulunamadı!");
        }
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            Debug.Log($"✅ Max Health set to: {maxHealth}");
        }

        if (healthText != null)
        {
            healthText.text = $"{maxHealth}/{maxHealth}";
        }
    }

    public void SetHealth(int health)
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
            Debug.Log($"🔄 Health UI updated to: {health}");
        }
        else
        {
            Debug.LogError("❌ HealthSlider null! SetHealth çalışmadı.");
        }

        if (healthText != null)
        {
            healthText.text = $"{health}/{(int)healthSlider.maxValue}";
        }
    }
}