using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [Header("AI Ayarları")]
    [SerializeField] private float detectionRange = 70f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    
    [Header("Sağlık")]
    [SerializeField] private float health = 100f;

    private Transform player;
    private NavMeshAgent agent;
    private bool isAttacking = false;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        // Oyuncuyu bul (Player tag'i olan GameObject)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // NavMeshAgent component'ini al
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        // NavMeshAgent ayarları
        agent.speed = 3.5f;
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;
        agent.stoppingDistance = attackRange;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Oyuncu menzilde mi?
        if (distanceToPlayer <= detectionRange)
        {
            // Saldırı menzilinde mi?
            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                AttackPlayer();
            }
            else
            {
                // Oyuncuya doğru hareket et
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        else
        {
            // Oyuncu menzil dışında - dur
            agent.isStopped = true;
        }
    }

    private void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        // Oyuncuya doğru bak
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        // Burada saldırı animasyonu çalabilir
        Debug.Log("Zombi saldırıyor!");

        // Oyuncuya hasar ver - şimdilik sadece debug
        Debug.Log("Oyuncuya 20 hasar verildi!");
        
        // PlayerController sınıfın olduğunda bu kısmı aktif et:
        /*
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(20f);
        }
        */

        // Saldırı bitişi
        StartCoroutine(EndAttack());
    }

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        
        // NavMeshAgent'i durdur
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
        
        // Ölüm animasyonu vs. burada çalabilir
        Debug.Log("Zombi öldü!");
        
        // Zombiyi yok et
        Destroy(gameObject, 2f);
    }

    // Gizmos - Editor'da menzili göster
    void OnDrawGizmosSelected()
    {
        // Algılama menzili
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Saldırı menzili
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}