using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public AudioClip healthDecreaseClip;
    public AudioClip healthIncreaseClip;
    public AudioClip coinCollectClip;
    public AudioClip coinDecreaseClip;

    private AudioSource audioSource;
    private float lastHealth;
    private int lastCoins;

    public float currentHealth; 
    public int Coins; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastHealth = currentHealth;
        lastCoins = Coins;
    }
    public void OnCoinCollected() 
    {
        if (coinCollectClip != null && audioSource != null)
            audioSource.PlayOneShot(coinCollectClip);
    }

    public void OnHealthDecrease()
    {
        if (healthDecreaseClip != null && audioSource != null)
            audioSource.PlayOneShot(healthDecreaseClip);
    }

    public void OnHealthIncrease()
    {
        if (healthIncreaseClip != null && audioSource != null)
            audioSource.PlayOneShot(healthIncreaseClip);
    }

    void Update()
    {
        if (currentHealth < lastHealth && healthDecreaseClip != null)
            PlaySound(healthDecreaseClip);
        else if (currentHealth > lastHealth && healthIncreaseClip != null)
            PlaySound(healthIncreaseClip);

        if (Coins > lastCoins && coinCollectClip != null)
            PlaySound(coinCollectClip);
        else if (Coins < lastCoins && coinDecreaseClip != null)
            PlaySound(coinDecreaseClip);

        lastHealth = currentHealth;
        lastCoins = Coins;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
