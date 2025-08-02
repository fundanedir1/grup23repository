using System.Collections;
using UnityEngine;

public class MushroomPowerUp : MonoBehaviour
{
    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 10f;
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float fireRateMultiplier = 2f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem collectEffect;
    [SerializeField] private AudioSource collectSound;
    [SerializeField] private GameObject mushroomModel;
    
    private bool isCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectMushroom(other.gameObject);
        }
    }

    private void CollectMushroom(GameObject player)
    {
        isCollected = true;
        
        PlayCollectEffects();
        
        PowerUpManager powerUpManager = player.GetComponent<PowerUpManager>();
        if (powerUpManager == null)
        {
            powerUpManager = player.AddComponent<PowerUpManager>();
        }
        
        powerUpManager.ActivateSpeedBoost(speedMultiplier, effectDuration);
        
  
        if (mushroomModel != null)
            mushroomModel.SetActive(false);
        
  
        GetComponent<Collider>().enabled = false;
        
        
        Destroy(gameObject, 1f);
    }

    private void PlayCollectEffects()
    {
     
        if (collectEffect != null)
            collectEffect.Play();
        
        if (collectSound != null)
            collectSound.Play();
    }
}