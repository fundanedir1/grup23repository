using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;

    public void GetDamage(float damage)
    {
        hitPoints -= damage;

        if (hitPoints <= 0)
        {
            StartCoroutine(DeathDelay());
        }
    }

    private System.Collections.IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(0.03f); 
        Destroy(gameObject);
    }
}