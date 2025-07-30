using UnityEngine;

namespace EasyPeasyFirstPersonController
{
    public class WeaponBob : MonoBehaviour
    {
        [Header("Weapon Sway Settings")]
        [Range(0f, 0.1f)] public float swayAmount = 0.05f;
        [Range(0f, 20f)] public float swaySpeed = 14f;
        [Range(0f, 3f)] public float sprintSwayMultiplier = 1.2f;
        [Range(0f, 10f)] public float recoilReturnSpeed = 8f;
        
        [Header("References")]
        public FirstPersonController playerController;
        
        private Vector3 originalPosition;
        private Vector3 currentSwayOffset;
        private Vector3 recoil = Vector3.zero;
        private float swayTimer;
        private Vector2 moveInput;
        
        private void Start()
        {
            // Silahın orijinal pozisyonunu kaydet
            originalPosition = transform.localPosition;
            
            // Eğer FirstPersonController referansı verilmemişse otomatik bul
            if (playerController == null)
            {
                playerController = FindObjectOfType<FirstPersonController>();
            }
        }
        
        private void Update()
        {
            if (playerController == null) return;
            
            HandleWeaponSway();
        }
        
        private void HandleWeaponSway()
        {
            // Movement input'u al (FirstPersonController'dan)
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");
            
            // Karakter controller'dan horizontal velocity'yi hesapla
            CharacterController characterController = playerController.GetComponent<CharacterController>();
            Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
            bool isMovingEnough = horizontalVelocity.magnitude > 0.1f;
            
            // Head bob hesaplaması (FirstPersonController'daki gibi)
            float targetSwayOffset = isMovingEnough ? Mathf.Sin(swayTimer) * swayAmount : 0f;
            currentSwayOffset.y = Mathf.Lerp(currentSwayOffset.y, targetSwayOffset, Time.deltaTime * swaySpeed);
            
            // Eğer yerde değilse, sliding yapıyorsa veya crouching yapıyorsa sway'i sıfırla
            bool isGrounded = Physics.CheckSphere(playerController.groundCheck.position, playerController.groundDistance, playerController.groundMask);
            
            if (!isGrounded || playerController.isSliding || playerController.isCrouching)
            {
                swayTimer = 0f;
                currentSwayOffset = Vector3.Lerp(currentSwayOffset, Vector3.zero, Time.deltaTime * 10f);
                recoil = Vector3.zero;
                
                // Recoil'i yumuşak bir şekilde sıfırla
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(recoil), recoilReturnSpeed * Time.deltaTime);
                transform.localPosition = originalPosition + currentSwayOffset;
                return;
            }
            
            if (isMovingEnough)
            {
                // Sprint durumunda sway hızını artır
                float currentSwaySpeed = swaySpeed * (playerController.isSprinting ? sprintSwayMultiplier : 1f);
                swayTimer += Time.deltaTime * currentSwaySpeed;
                
                // Yatay hareket için recoil efekti (sağa-sola hareket)
                recoil.z = moveInput.x * -2f;
                
                // Silahın pozisyonunu güncelle
                transform.localPosition = originalPosition + currentSwayOffset;
            }
            else
            {
                swayTimer = 0f;
                currentSwayOffset = Vector3.Lerp(currentSwayOffset, Vector3.zero, Time.deltaTime * 10f);
                recoil = Vector3.zero;
                
                transform.localPosition = originalPosition + currentSwayOffset;
            }
            
            // Recoil rotasyonunu uygula
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(recoil), recoilReturnSpeed * Time.deltaTime);
        }
        
        // Dışarıdan recoil eklemek için (ateş etme durumunda)
        public void AddRecoil(Vector3 recoilAmount)
        {
            recoil += recoilAmount;
        }
        
        // Silahın orijinal pozisyonunu yeniden ayarla (silah değiştirirken kullanılabilir)
        public void ResetPosition()
        {
            originalPosition = transform.localPosition;
            currentSwayOffset = Vector3.zero;
            recoil = Vector3.zero;
            swayTimer = 0f;
        }
    }
}