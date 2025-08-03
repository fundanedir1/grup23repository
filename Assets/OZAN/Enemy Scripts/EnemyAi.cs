using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private bool foundPlayer = false;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // Animator kontrolü
        if (animator == null)
        {
            Debug.LogError(name + " Animator component bulunamadı!");
        }
        else
        {
            Debug.Log(name + " Animator bulundu. Controller: " + (animator.runtimeAnimatorController ? animator.runtimeAnimatorController.name : "NULL"));
        }
        
        // Başlangıçta agent'ı durdur
        if (agent != null)
        {
            agent.enabled = false;
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
        
        // NavMesh pozisyonunu düzelt
        FixNavMeshPosition();
        
        // Agent'ı aktifleştir
        if (agent != null)
        {
            agent.enabled = true;
            agent.speed = 2f;
            agent.stoppingDistance = 2f;
            agent.autoBraking = true;
        }
        
        // Başlangıç animasyonu
        SetAnimation("idle", true);
        
        Debug.Log(name + " AI initialized. Player found: " + foundPlayer);
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
    
    void FixNavMeshPosition()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log(name + " NavMesh position fixed: " + hit.position);
        }
        else
        {
            Debug.LogError(name + " NavMesh'te uygun pozisyon bulunamadı!");
        }
    }
    
    void Update()
    {
        if (!foundPlayer || player == null || agent == null || !agent.enabled)
            return;
            
        // Player mesafesi
        float distance = Vector3.Distance(transform.position, player.position);
        
        // Debug çizgisi
        Debug.DrawLine(transform.position, player.position, Color.red);
        
        // 20 metre içindeyse takip et
        if (distance < 10000f)
        {
            if (distance > agent.stoppingDistance)
            {
                // Takip et
                agent.SetDestination(player.position);
                SetAnimation("idle", false);
                SetAnimation("attack", false);
                // Walk animasyonu (varsa)
                if (HasParameter("walk"))
                    SetAnimation("walk", true);
            }
            else
            {
                // Saldır
                agent.ResetPath();
                SetAnimation("idle", false);
                SetAnimation("walk", false);
                SetAnimation("attack", true);
                Debug.Log(name + " SALDIRIYOR!");
            }
        }
        else
        {
            // Uzaksa dur
            agent.ResetPath();
            SetAnimation("attack", false);
            SetAnimation("walk", false);
            SetAnimation("idle", true);
        }
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
    
    // Gizmo ile debug
    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 20f);
        }
        
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}