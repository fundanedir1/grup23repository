using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;
   // [SerializeField] Slider healthSlider;//

    
    

    public void GetDamage(float damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        //healthSlider.value = hitPoints;//
    }
    
}