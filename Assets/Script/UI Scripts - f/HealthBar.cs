using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;

    [Header("Sesler")]
    [SerializeField] private AudioClip damageClip; // Can azaldiginda
    [SerializeField] private AudioClip healClip;   // Can arttiginda
    [SerializeField] private AudioClip deathClip;  // Can 0 oldugunda

    private AudioSource audioSource;
    private int previousHealth = -1;
    private bool hasDied = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Yoksa ekleniyor
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (healthSlider == null)
        {
            healthSlider = GetComponent<Slider>();
        }

        if (healthSlider == null)
        {
            Debug.LogError("❌ HealthBar: Slider component bulunamadi!");
        }

        // Baslangicta previousHealth mevcut degerle esitle
        if (healthSlider != null)
        {
            previousHealth = (int)healthSlider.value;
        }
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            previousHealth = maxHealth;
            Debug.Log($"✅ Max Health set to: {maxHealth}");
        }

        if (healthText != null)
        {
            healthText.text = $"{maxHealth}/{maxHealth}";
        }

        hasDied = false; // Can yenilenince olum durumu sifirlanir
    }

    public void SetHealth(int health)
    {
        if (healthSlider == null)
        {
            Debug.LogError("❌ HealthSlider null! SetHealth calismadi.");
            return;
        }

        int clampedHealth = Mathf.Clamp(health, 0, (int)healthSlider.maxValue);
        bool isIncrease = clampedHealth > previousHealth;
        bool isDecrease = clampedHealth < previousHealth;

        healthSlider.value = clampedHealth;
        Debug.Log($"🔄 Health UI updated to: {clampedHealth}");

        if (healthText != null)
        {
            healthText.text = $"{clampedHealth}/{(int)healthSlider.maxValue}";
        }

        // Ses caldirma kontrolu
        if (clampedHealth == 0)
        {
            if (!hasDied)
            {
                PlaySound(deathClip);
                hasDied = true;
            }
        }
        else
        {
            if (isDecrease)
            {
                PlaySound(damageClip);
            }
            else if (isIncrease)
            {
                PlaySound(healClip);
            }

            hasDied = false;
        }

        previousHealth = clampedHealth;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }
}
