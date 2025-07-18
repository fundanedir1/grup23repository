using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float chasingRadious = 6f;

    NavMeshAgent navMeshAgent;
    float targetDistance = Mathf.Infinity;
    bool isProved = false;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        targetDistance = Vector3.Distance(transform.position, target.position);

        if (isProved)
        {
            if (targetDistance > chasingRadious) // Eğer hedef alanın dışına çıkarsa
            {
                StopChasing();
            }
            else
            {
                DelayWithTarget();
            }
        }
        else if (targetDistance <= chasingRadious)
        {
            isProved = true;
        }
    }

    private void DelayWithTarget()
    {
        if (targetDistance >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }

        if (targetDistance <= navMeshAgent.stoppingDistance)
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
       
        navMeshAgent.SetDestination(target.position);
    }

    private void StopChasing()
    {
        isProved = false; // Takibi durdur
        navMeshAgent.ResetPath(); // Hareketi durdur
        
        animator.SetBool("attack", false); // Saldırıyı durdur
        animator.SetBool("idle", true); // Idle durumuna geç
        Debug.Log(name + " has stopped chasing " + target.name);
    }
    
}