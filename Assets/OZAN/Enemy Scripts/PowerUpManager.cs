using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private EasyPeasyFirstPersonController.FirstPersonController playerController;
    
 
    private float originalWalkSpeed;
    private float originalSprintSpeed;
    private float originalCrouchSpeed;
    
 
    private bool hasSpeedBoost = false;
    private Coroutine currentSpeedBoostCoroutine;

    private void Start()
    {
      
        playerController = GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();
        
        if (playerController == null)
        {
            playerController = EasyPeasyFirstPersonController.FirstPersonController.Instance;
        }
        
        if (playerController != null)
        {
       
            StoreOriginalValues();
            Debug.Log($"✅ PowerUpManager başlatıldı! Orijinal hızlar: Walk={originalWalkSpeed}, Sprint={originalSprintSpeed}");
        }
        else
        {
            Debug.LogError("❌ FirstPersonController bulunamadı!");
        }
    }

    private void StoreOriginalValues()
    {
        if (playerController != null)
        {
            originalWalkSpeed = playerController.walkSpeed;
            originalSprintSpeed = playerController.sprintSpeed;
            originalCrouchSpeed = playerController.crouchSpeed;
        }
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        if (playerController == null)
        {
            Debug.LogError("❌ PlayerController bulunamadı!");
            return;
        }

     
        if (hasSpeedBoost && currentSpeedBoostCoroutine != null)
        {
            StopCoroutine(currentSpeedBoostCoroutine);
            RestoreOriginalSpeeds();
            Debug.Log("🔄 Önceki speed boost iptal edildi!");
        }

     
        currentSpeedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        hasSpeedBoost = true;
        
        
        ApplySpeedBoost(multiplier);
        
      
        if (CameraShakeEffect.Instance != null)
        {
            CameraShakeEffect.Instance.StartMushroomEffect(duration);
        }
        
        Debug.Log($"🏃 Speed Boost aktif! Multiplier: {multiplier}x, Süre: {duration}s");
        
    
        yield return new WaitForSeconds(duration);
        
      
        RestoreOriginalSpeeds();
        hasSpeedBoost = false;
        currentSpeedBoostCoroutine = null;
        
        Debug.Log("✨ Speed Boost sona erdi!");
    }

    private void ApplySpeedBoost(float multiplier)
    {
        if (playerController != null)
        {
            float newWalkSpeed = originalWalkSpeed * multiplier;
            float newSprintSpeed = originalSprintSpeed * multiplier;
            float newCrouchSpeed = originalCrouchSpeed * multiplier;
            
            playerController.walkSpeed = newWalkSpeed;
            playerController.sprintSpeed = newSprintSpeed;
            playerController.crouchSpeed = newCrouchSpeed;
            
            Debug.Log($"🚀 Hızlar güncellendi! Walk: {originalWalkSpeed} → {newWalkSpeed}, Sprint: {originalSprintSpeed} → {newSprintSpeed}");
        }
    }

    private void RestoreOriginalSpeeds()
    {
        if (playerController != null)
        {
            playerController.walkSpeed = originalWalkSpeed;
            playerController.sprintSpeed = originalSprintSpeed;
            playerController.crouchSpeed = originalCrouchSpeed;
            
            Debug.Log($"🔄 Orijinal hızlar restore edildi! Walk: {originalWalkSpeed}, Sprint: {originalSprintSpeed}");
        }
    }


    [ContextMenu("Show Current Speeds")]
    public void ShowCurrentSpeeds()
    {
        if (playerController != null)
        {
            Debug.Log($"📊 Şu anki hızlar: Walk={playerController.walkSpeed}, Sprint={playerController.sprintSpeed}, Crouch={playerController.crouchSpeed}");
            Debug.Log($"📊 Orijinal hızlar: Walk={originalWalkSpeed}, Sprint={originalSprintSpeed}, Crouch={originalCrouchSpeed}");
            Debug.Log($"📊 Speed Boost aktif: {hasSpeedBoost}");
        }
    }

    [ContextMenu("Force Reset Speeds")]
    public void ForceResetSpeeds()
    {
        if (hasSpeedBoost && currentSpeedBoostCoroutine != null)
        {
            StopCoroutine(currentSpeedBoostCoroutine);
        }
        RestoreOriginalSpeeds();
        hasSpeedBoost = false;
        currentSpeedBoostCoroutine = null;
        Debug.Log("🔧 Hızlar zorla sıfırlandı!");
    }

    private void OnDestroy()
    {
        // Component destroy edilirken hızları restore et
        if (hasSpeedBoost)
        {
            RestoreOriginalSpeeds();
        }
    }
}