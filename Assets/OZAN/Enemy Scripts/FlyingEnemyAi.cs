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

    float targetDistance = Mathf.Infinity;
    bool isProvoked = false;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
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

    // Debug için Gizmos
    void OnDrawGizmosSelected()
    {
        // Takip alanı
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chasingRadius);
        
        // Saldırı alanı
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}