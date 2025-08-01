using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 100f;
    [SerializeField] private Animator animator;         // Animator bağlantısı
    [SerializeField] private float deathAnimDuration = 1.4f; // Ölüm animasyonu süresi (saniye cinsinden)

    private bool isDead = false;

    public void GetDamage(float damage)
    {
        if (isDead) return; // Zombi zaten öldüyse tekrar hasar alma

        hitPoints -= damage;

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("die"); // Animator'daki "die" trigger'ını çalıştır
        }

        StartCoroutine(DeathDelay());
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(deathAnimDuration); // Animasyon bitene kadar bekle
        Destroy(gameObject);
    }
}