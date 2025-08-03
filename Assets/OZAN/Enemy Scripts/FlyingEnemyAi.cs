using UnityEngine;
using System.Collections;

public class SimpleFlyingAI : MonoBehaviour
{
    [Header("Flight Settings")]
    [SerializeField] private float flySpeed = 5f;
    [SerializeField] private float hoverHeight = 2f;
    
    [Header("Death Fall Settings")]
    [SerializeField] private float fallSpeed = 10f;
    [SerializeField] private LayerMask groundLayerMask = -1;
    
    private Transform player;
    private Animator animator;
    private Rigidbody rb;
    private EnemyHealth enemyHealth;
    private bool foundPlayer = false;
    private bool isDead = false;
    private bool hasHitGround = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();
        
        // Animator kontrolü
        if (animator == null)
        {
            Debug.LogError(name + " Animator component bulunamadı!");
        }
        else
        {
            Debug.Log(name + " Animator bulundu. Controller: " + (animator.runtimeAnimatorController ? animator.runtimeAnimatorController.name : "NULL"));
        }
        
        // Rigidbody ayarları
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        // Coroutine ile başlat
        StartCoroutine(InitializeAI());
    }
    
    IEnumerator InitializeAI()
    {
        // Biraz bekle
        yield return new WaitForSeconds(0.1f);
        
        // Player'ı bul
        FindPlayer();
        
        // Başlangıç animasyonu
        SetAnimation("idle", true);
        
        Debug.Log(name + " Flying AI initialized. Player found: " + foundPlayer);
    }
    
    void FindPlayer()
    {
        // Önce tag ile bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObj != null)
        {
            player = playerObj.transform;
            foundPlayer = true;
            Debug.Log(name + " found player by tag: " + player.name);
            return;
        }
        
        // Tag ile bulamazsa isim ile bul
        playerObj = GameObject.Find("FirstPersonController");
        if (playerObj != null)
        {
            player = playerObj.transform;
            foundPlayer = true;
            Debug.Log(name + " found player by name: " + player.name);
            return;
        }
        
        Debug.LogError(name + " PLAYER BULUNAMADI! FirstPersonController'a Player tag'i ekle!");
    }
    
    void Update()
    {
        // Ölüm kontrolü
        if (enemyHealth != null && enemyHealth.IsDead() && !isDead)
        {
            isDead = true;
            StartDeathFall();
            return;
        }
        
        // Ölüyse sadece düşme kontrolü
        if (isDead)
        {
            if (!hasHitGround)
            {
                ManualGroundCheck();
            }
            return;
        }
        
        if (!foundPlayer || player == null)
            return;
            
        // Player mesafesi
        float distance = Vector3.Distance(transform.position, player.position);
        
        // Debug çizgisi
        Debug.DrawLine(transform.position, player.position, Color.red);
        
        // 20 metre içindeyse takip et (zombie ile aynı)
        if (distance < 10000f)
        {
            if (distance > 3f) // Attack distance
            {
                // Takip et - FLY
                ChaseTarget();
            }
            else
            {
                // Saldır - ATTACK
                AttackTarget();
            }
        }
        else
        {
            // Uzaksa dur - IDLE
            ReturnToIdle();
        }
    }
    
    private void ChaseTarget()
    {
        SetAnimation("idle", false);
        SetAnimation("attack", false);
        
        // Player'ın üstüne uç
        Vector3 targetPosition = player.position + Vector3.up * hoverHeight;
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // Hareket et
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, flySpeed * Time.deltaTime);
        
        // Player'a bak
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
        }
        
        Debug.Log(name + " FLYING to player");
    }
    
    private void AttackTarget()
    {
        SetAnimation("idle", false);
        SetAnimation("attack", true);
        
        // Player'a bak
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
        }
        
        Debug.Log(name + " ATTACKING player!");
    }
    
    private void ReturnToIdle()
    {
        SetAnimation("attack", false);
        SetAnimation("idle", true);
        Debug.Log(name + " IDLE - hovering");
    }
    
    void SetAnimation(string parameterName, bool value)
    {
        if (animator != null && HasParameter(parameterName))
        {
            animator.SetBool(parameterName, value);
            Debug.Log(name + " Animation: " + parameterName + " = " + value);
        }
    }
    
    bool HasParameter(string parameterName)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parameterName)
                return true;
        }
        return false;
    }
    
    private void StartDeathFall()
    {
        if (animator != null)
        {
            SetAnimation("attack", false);
            SetAnimation("idle", false);
        }
        
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = false;
            }
        }
        
        Debug.Log(name + " has died and is falling!");
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (isDead && !hasHitGround)
        {
            hasHitGround = true;
            HandleGroundHit();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (isDead && !hasHitGround)
        {
            hasHitGround = true;
            HandleGroundHit();
        }
    }
    
    private void HandleGroundHit()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        Debug.Log(name + " has hit the ground!");
    }
    
    private void ManualGroundCheck()
    {
        RaycastHit hit;
        float checkDistance = 0.5f;
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance))
        {
            hasHitGround = true;
            transform.position = hit.point + Vector3.up * 0.1f;
            HandleGroundHit();
        }
        else
        {
            if (transform.position.y < -10f)
            {
                hasHitGround = true;
                HandleGroundHit();
            }
        }
    }
    
    // Debug Gizmos
    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 20f); // Detection range
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 3f); // Attack range
        }
    }
}