using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float chasingRadious = 6f;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private EnemyHealth enemyHealth;

    private float targetDistance = Mathf.Infinity;
    private bool isProved = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead()) return; // Ölü düşman hiçbir şey yapmasın

        targetDistance = Vector3.Distance(transform.position, target.position);

        if (isProved)
        {
            if (targetDistance > chasingRadious)
                StopChasing();
            else
                DelayWithTarget();
        }
        else if (targetDistance <= chasingRadious)
        {
            isProved = true;
        }
    }

    private void DelayWithTarget()
    {
        if (targetDistance >= navMeshAgent.stoppingDistance)
            ChaseTarget();

        if (targetDistance <= navMeshAgent.stoppingDistance)
            AttackTarget();
    }

    private void AttackTarget()
    {
        animator.SetBool("attack", true);
        Debug.Log(name + " is attacking " + target.name);
    }

    private void ChaseTarget()
    {
        animator.SetBool("attack", false);
        animator.SetBool("idle", false);
        navMeshAgent.SetDestination(target.position);
    }

    private void StopChasing()
    {
        isProved = false;
        navMeshAgent.ResetPath();
        animator.SetBool("attack", false);
        animator.SetBool("idle", true);
        Debug.Log(name + " has stopped chasing " + target.name);
    }
}