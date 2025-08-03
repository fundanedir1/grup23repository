using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [SerializeField] private float rotationSpeedX = 0f;
    [SerializeField] private float rotationSpeedY = 50f; 
    [SerializeField] private float rotationSpeedZ = 0f;

    [SerializeField] private int healingAmount = 20;
    [SerializeField] private float respawnTime = 5f; 

    private void Update()
    {
        
        transform.Rotate(rotationSpeedX * Time.deltaTime, rotationSpeedY * Time.deltaTime, rotationSpeedZ * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healingAmount);
                Debug.Log("Player healed by: " + healingAmount + " HP");
            }

            
            gameObject.SetActive(false);  

            
            Invoke("Respawn", respawnTime);
        }
    }

    
    void Respawn()
    {
        gameObject.SetActive(true); 
    }
}