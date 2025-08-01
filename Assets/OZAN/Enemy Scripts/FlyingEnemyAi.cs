using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyAi : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float chasingRadius = 6f;
    [SerializeField] private float flySpeed = 5f;
    [SerializeField] private float attackDistance = 2f;

    [Header("Death Fall Settings")]
    [SerializeField] private float fallSpeed = 10f;

    float targetDistance = Mathf.Infinity;
    bool isProvoked = false;
    Animator animator;
    Rigidbody rb;
    EnemyHealth enemyHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();
        
        // Başlangıçta Rigidbody'yi kinematic yap (fizik simülasyonu olmasın)
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        // EnemyHealth scriptinden ölüm durumunu kontrol et
        if (enemyHealth != null && enemyHealth.IsDead())
        {
            HandleDeathFall();
            return;
        }

        targetDistance = Vector3.Distance(transform.position, target.position);

        if (isProvoked)
        {
            if (targetDistance > chasingRadius)
            {
                StopChasing();
            }
            else
            {
                DelayWithTarget();
            }
        }
        else if (targetDistance <= chasingRadius)
        {
            isProvoked = true;
        }
    }

    private void DelayWithTarget()
    {
        if (targetDistance >= attackDistance)
        {
            ChaseTarget();
        }
        else
        {
            AttackTarget();
        }
    }

    private void AttackTarget()
    {
        animator.SetBool("attack", true);
        Debug.Log(name + " is destroying " + target.name);
    }

    private void ChaseTarget()
    {
        animator.SetBool("attack", false);
        animator.SetBool("idle", false);

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 nextPosition = transform.position + direction * flySpeed * Time.deltaTime;
        
        // Engel kontrolü
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, flySpeed * Time.deltaTime + 0.5f))
        {
            // Engel var, yukarıdan aş
            Vector3 avoidDirection = Vector3.up;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + avoidDirection, flySpeed * Time.deltaTime);
        }
        else
        {
            // Engel yok, normal hareket
            transform.position = Vector3.MoveTowards(transform.position, target.position, flySpeed * Time.deltaTime);
        }
        
        // Hedefe doğru bak
        if (direction != Vector3.zero)
        {
            transform.LookAt(target);
        }
    }

    private void StopChasing()
    {
        isProvoked = false;
        
        animator.SetBool("attack", false);
        animator.SetBool("idle", true);
        Debug.Log(name + " has stopped chasing " + target.name);
    }

    // Ölüm durumunda yere düşme işlemi
    private void HandleDeathFall()
    {
        // İlk kez ölüm durumu tespit edildiğinde
        if (rb != null && rb.isKinematic)
        {
            // Rigidbody'yi aktive et (fizik simülasyonu başlasın)
            rb.isKinematic = false;
            rb.useGravity = true;
            
            // AI davranışlarını durdur
            isProvoked = false;
            
            Debug.Log(name + " has died and is falling!");
        }

        // Eğer Rigidbody yoksa manuel olarak düşür
        if (rb == null)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        // Yere çarptıysa duraksama efekti (opsiyonel)
        CheckGroundImpact();
    }

    // Yere çarpma kontrolü
    private void CheckGroundImpact()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f))
        {
            if (hit.collider.CompareTag("Ground") || hit.collider.name.ToLower().Contains("ground"))
            {
                // Yere çarptı, hızını azalt
                if (rb != null && rb.velocity.magnitude > 1f)
                {
                    rb.velocity *= 0.3f; // Yavaşlat
                }
            }
        }
    }

    // Debug için Gizmos
    void OnDrawGizmosSelected()
    {
        // Sadece canlıyken gizmos göster
        if (enemyHealth == null || !enemyHealth.IsDead())
        {
            // Takip alanı
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chasingRadius);
            
            // Saldırı alanı
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);
        }
    }
}