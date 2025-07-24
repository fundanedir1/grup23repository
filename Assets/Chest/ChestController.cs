using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChestController : MonoBehaviour
{
    public Animator animator;
    public bool isOpen = false;
    public float uiDelayAfterClose = 1.5f;
    public GameObject interactUI; // "E'ye bas" yazısı
    
    [Header("Mesafe Ayarları")]
    public float interactDistance = 3f; // Kaç metre yakından etkileşim

    private bool isPlayerNear = false;
    private bool uiLocked = false;
    private Transform player;

    void Start()
    {
        // Player'ı bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player bulunamadı! Player objesine 'Player' tag'i eklenmiş mi?");
    }

    void Update()
    {
        // Player mesafe kontrolü
        CheckPlayerDistance();

        if (isPlayerNear && !uiLocked)
        {
            if (!isOpen)
                interactUI.SetActive(true); // Açmak için E'ye bas
            else
                interactUI.SetActive(false); // Açıkken yazı yok

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E tuşu basıldı!");
                ToggleChest();
            }
        }
        else
        {
            interactUI.SetActive(false);
        }
    }

    void CheckPlayerDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance <= interactDistance)
        {
            if (!isPlayerNear)
                Debug.Log("Player yaklaştı - mesafe: " + distance);
            isPlayerNear = true;
        }
        else
        {
            if (isPlayerNear)
                Debug.Log("Player uzaklaştı - mesafe: " + distance);
            isPlayerNear = false;
        }
    }

    void ToggleChest()
    {
        Debug.Log("ToggleChest çağrıldı! Önceki durum: " + isOpen);
        
        isOpen = !isOpen;
        
        Debug.Log("Yeni durum: " + isOpen);
        
        if (animator != null)
        {
            animator.SetBool("Open", isOpen);
            Debug.Log("Animator'a Open=" + isOpen + " gönderildi");
        }
        else
        {
            Debug.LogError("Animator null! Inspector'da Animator atanmış mı?");
        }

        if (!isOpen)
        {
            // Sandık kapatıldıktan sonra UI biraz geç gelsin
            StartCoroutine(ShowUIDelayed());
        }
    }

    IEnumerator ShowUIDelayed()
    {
        uiLocked = true;
        yield return new WaitForSeconds(uiDelayAfterClose);
        uiLocked = false;
    }

    // Debug için görsel gösterim
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}