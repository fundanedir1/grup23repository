using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] Camera FPSCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damageAmount = 45f;
    [SerializeField] ParticleSystem shoot;
    [SerializeField] GameObject explosion;
    
    // Update is called once per frame
    void Update()
    {
        // Eğer ateş tuşuna basılmışsa VE şu an eğilmiyorsa
        if (Input.GetButtonDown("Fire1") && !Input.GetKey(KeyCode.LeftControl))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        ShootingEffect();
        Raycasting();
    }

    private void Raycasting()
    {
        RaycastHit hit;

        if (Physics.Raycast(FPSCamera.transform.position, FPSCamera.transform.forward, out hit, range))
        {
            HittEffect(hit);
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target == null) return;
            target.GetDamage(damageAmount);
        }
        else
        {
            return;
        }
    }

    private  void HittEffect(RaycastHit hit)
    {
        GameObject hitVisual = Instantiate(explosion, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(hitVisual,1);
    }
    

    private void ShootingEffect()
    {
        shoot.Play();
    }
}