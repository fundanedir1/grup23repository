using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 100f;
    [SerializeField] private Animator animator;
    [SerializeField] private float deathAnimDuration = 1.4f;

    private bool isDead = false;

    public void GetDamage(float damage)
    {
        if (isDead) return;

        hitPoints -= damage;

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    public bool IsDead() // AI tarafından kontrol edilsin diye public getter
    {
        return isDead;
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger("die");

        // Collider'ları kapat
        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        // NavMeshAgent'ı kapat
        var navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
            navAgent.enabled = false;

        StartCoroutine(DeathDelay());
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(deathAnimDuration);
        Destroy(gameObject);
    }
}