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
    [SerializeField] private LayerMask groundLayerMask = -1; 

    float targetDistance = Mathf.Infinity;
    bool isProvoked = false;
    bool isDead = false;
    bool hasHitGround = false;
    
    Animator animator;
    Rigidbody rb;
    EnemyHealth enemyHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();
        
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead() && !isDead)
        {
            isDead = true;
            StartDeathFall();
            return;
        }
        
        if (isDead)
        {
            if (!hasHitGround)
            {
                ManualGroundCheck();
            }
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
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, flySpeed * Time.deltaTime + 0.5f))
        {
            Vector3 avoidDirection = Vector3.up;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + avoidDirection, flySpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, flySpeed * Time.deltaTime);
        }
        
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


    private void StartDeathFall()
    {

        isProvoked = false;
        
        if (animator != null)
        {
            animator.SetBool("attack", false);
            animator.SetBool("idle", false);
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
        Debug.Log($"COLLISION DETECTED: {collision.gameObject.name}, Tag: {collision.gameObject.tag}, Layer: {collision.gameObject.layer}");
        Debug.Log($"isDead: {isDead}, hasHitGround: {hasHitGround}");
        
        if (isDead && !hasHitGround)
        {
            Debug.Log("Death collision check passed");
            
            hasHitGround = true;
            HandleGroundHit();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TRIGGER DETECTED: {other.gameObject.name}");
        
        if (isDead && !hasHitGround)
        {
            hasHitGround = true;
            HandleGroundHit();
        }
    }
    
    private bool IsInGroundLayer(GameObject obj)
    {
        return (groundLayerMask.value & (1 << obj.layer)) > 0;
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
            Debug.Log($"Manual ground check hit: {hit.collider.name}");
            hasHitGround = true;
            
            transform.position = hit.point + Vector3.up * 0.1f;
            
            HandleGroundHit();
        }
        else
        {
            if (transform.position.y < -10f)
            {
                Debug.Log("Emergency stop - too low!");
                hasHitGround = true;
                HandleGroundHit();
            }
        }
    }
    
    private void ManualFall()
    {
        if (hasHitGround) return;
        
        RaycastHit hit;
        float rayDistance = fallSpeed * Time.deltaTime + 0.1f;
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, groundLayerMask))
        {
            transform.position = hit.point + Vector3.up * 0.1f; 
            hasHitGround = true;
            Debug.Log(name + " has hit the ground (manual)!");
            return;
        }
        
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }
    
    void OnDrawGizmosSelected()
    {
        if (!isDead)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chasingRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);
        }
        
        if (isDead && !hasHitGround)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Vector3.down * (fallSpeed * Time.deltaTime + 0.1f));
        }
    }
}