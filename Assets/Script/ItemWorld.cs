using UnityEngine;

/// <summary>
/// Yere düşen (pickup) nesneleri temsil eden bileşen.
/// Player çarpıştığında QuickBarInventory'ye eklenir.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ItemWorld : MonoBehaviour
{
    [Header("Veri")]
    public ItemData itemData;
    public int amount = 1;

    [Header("Otomatik Yok Olma")]
    [Tooltip("0'dan büyükse bu kadar saniye sonra objeyi yok eder.")]
    [SerializeField] private float selfDestroySeconds = 120f;

    private void Start()
    {
        if (selfDestroySeconds > 0f) Destroy(gameObject, selfDestroySeconds);
    }

    /// <summary>
    /// Envanter vs. tarafından çağrılır.
    /// </summary>
    public void Initialize(ItemData data, int amt = 1)
    {
        itemData = data;
        amount = Mathf.Max(1, amt);
        // Dünya modeli ikonu prefab'da ayarlanmış olmalı.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var inv = other.GetComponent<QuickBarInventory>();
        if (inv == null) return;

        if (inv.AddItem(itemData, amount))
            Destroy(gameObject);
    }
}
