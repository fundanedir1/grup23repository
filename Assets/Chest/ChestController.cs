using UnityEngine;
using System.Collections;

public class ChestController : MonoBehaviour
{
    [Header("Sandık Ayarları")]
    [SerializeField] private int chestPrice = 1;
    [SerializeField] private GameObject[] itemPool;
    [SerializeField] private Transform[] itemSpawnPoints; // 🔁 Artık dizi oldu!
    [SerializeField] private float interactDistance = 3f;

    [Header("UI ve Görsel")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject interactUI;
    [SerializeField] private GameObject notEnoughMoneyUI;
    [SerializeField] private float moneyWarningDuration = 2f;
    [SerializeField] private float uiDelayAfterClose = 1.5f;

    private bool isPlayerNear = false;
    private bool uiLocked = false;
    private bool isOpen = false;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player tag eksik!");
    }

    void Update()
    {
        CheckPlayerDistance();

        if (isPlayerNear && !uiLocked)
        {
            bool hasEnoughMoney = MoneyManager.Money >= chestPrice;

            if (!isOpen && !notEnoughMoneyUI.activeSelf)
            {
                interactUI.SetActive(true);
            }
            else
            {
                interactUI.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryToggleChest();
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
        isPlayerNear = distance <= interactDistance;
    }

    void TryToggleChest()
    {
        if (isOpen)
        {
            CloseChest();
            return;
        }

        bool canOpen = chestPrice <= 0 || MoneyManager.Spend(chestPrice);

        if (canOpen)
        {
            OpenChest();
        }
        else
        {
            interactUI.SetActive(false);
            if (notEnoughMoneyUI != null)
            {
                notEnoughMoneyUI.SetActive(true);
                Invoke(nameof(HideMoneyWarning), moneyWarningDuration);
            }
        }
    }

    void OpenChest()
    {
        isOpen = true;

        if (animator != null)
            animator.SetBool("Open", true);

        SpawnRandomItems();
    }

    void CloseChest()
    {
        isOpen = false;

        if (animator != null)
            animator.SetBool("Open", false);

        StartCoroutine(ShowUIDelayed());
    }

    void SpawnRandomItems()
    {
        if (itemPool.Length == 0 || itemSpawnPoints.Length == 0)
        {
            Debug.LogWarning("Item havuzu ya da spawn noktaları eksik!");
            return;
        }

        foreach (Transform spawnPoint in itemSpawnPoints)
        {
            int randomIndex = Random.Range(0, itemPool.Length);
            GameObject itemToSpawn = itemPool[randomIndex];

            // Raycast yok: doğrudan pozisyona instantiate
            GameObject spawnedItem = Instantiate(itemToSpawn, spawnPoint.position, Quaternion.identity);
            Debug.Log("Sandıktan çıkan item: " + spawnedItem.name);
        }
    }


    IEnumerator ShowUIDelayed()
    {
        uiLocked = true;
        yield return new WaitForSeconds(uiDelayAfterClose);
        uiLocked = false;
    }

    void HideMoneyWarning()
    {
        if (notEnoughMoneyUI != null)
            notEnoughMoneyUI.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
