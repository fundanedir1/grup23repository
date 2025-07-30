using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    PlayerHealth target;
    [SerializeField] float damage = 30f;
    
    void Start()
    {
        target = FindAnyObjectByType<PlayerHealth>();

    }

    public void HitEvent()
    {
        if (target == null) return;
        target.GetDamage(damage);

        
        Debug.Log("Enemy hits the player");
    }
}