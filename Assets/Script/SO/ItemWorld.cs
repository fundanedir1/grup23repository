using UnityEngine;

/// <summary>
/// Yere düşen (pickup) nesneleri temsil eder.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ItemWorld : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;

    [SerializeField] private float selfDestroySeconds = 120f;

    private void Start()
    {
        if (selfDestroySeconds > 0f)
            Destroy(gameObject, selfDestroySeconds);
    }

    public void Initialize(ItemData data, int amt = 1)
    {
        itemData = data;
        amount = Mathf.Max(1, amt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var inv = other.GetComponent<QuickBarInventory>();
        if (inv == null) return;

        int remaining = inv.AddItem(itemData, amount);
        if (remaining <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            amount = remaining; // envanter doluysa kalan miktar güncellenir
        }
    }
}
