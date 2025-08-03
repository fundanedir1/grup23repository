using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SellCrate – hasat edilmiş ItemWorld'leri gece boyunca depolar, yeni Gündüz başladığında satar.
/// Ürünler kasaya atıldığında kaybolmaz; fizik kapatılıp görünür şekilde kasada bekler.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SellCrate : MonoBehaviour
{
    [Tooltip("Optional particle / VFX when cash is paid")] public ParticleSystem cashVFX;
    [Tooltip("Cash register sound")] public AudioClip cashSFX;

    // Pending stacks
    private readonly Dictionary<ItemData, int> pendingAmounts = new();
    private readonly List<ItemWorld> storedWorlds = new(); // sahnede bekletilen objeler

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    private void OnEnable()
    {
        if (GameManager.Instance)
            GameManager.Instance.OnStateChanged += HandleDayStart;
    }
    private void OnDisable()
    {
        if (GameManager.Instance)
            GameManager.Instance.OnStateChanged -= HandleDayStart;
    }

    // ───────────────── Trigger collect ─────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out ItemWorld iw)) return;

        // sadece satılabilir mahsuller (seed veya sellPrice<=0 ise alma)
        if (iw.itemData == null || iw.itemData.cropToPlant != null) return;
        if (iw.itemData.sellPrice <= 0) return;

        AddPending(iw.itemData, iw.amount);
        StoreWorld(iw);
    }

    private void AddPending(ItemData item, int amt)
    {
        if (pendingAmounts.ContainsKey(item)) pendingAmounts[item] += amt;
        else pendingAmounts[item] = amt;
        InteractUI.Instance.ShowToast($"Stored {amt} × {item.name}");
    }

    // ItemWorld'ü kasada görünür hâlde tut
    private void StoreWorld(ItemWorld iw)
    {
        iw.transform.SetParent(transform);
        iw.transform.localPosition = Vector3.zero + Random.insideUnitSphere * 0.2f; // ufak yayılma
        if (iw.TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
        if (iw.TryGetComponent(out Collider col)) col.enabled = false; // yeniden tetiklenmesin
        storedWorlds.Add(iw);
    }

    // ───────────────── Day cycle ─────────────────
    private void HandleDayStart(GameManager.GameState gs)
    {
        if (gs != GameManager.GameState.Day) return;     // yalnızca sabah
        if (pendingAmounts.Count == 0) return;

        int total = 0;
        foreach (var kv in pendingAmounts)
            total += kv.Key.sellPrice * kv.Value;
        pendingAmounts.Clear();

        // görsel objeleri temizle
        foreach (var iw in storedWorlds)
            if (iw) Destroy(iw.gameObject);
        storedWorlds.Clear();

        MoneyManager.Add(total);
        InteractUI.Instance.ShowToast($"+$ {total}");
        if (cashVFX) cashVFX.Play();
        if (cashSFX) AudioSource.PlayClipAtPoint(cashSFX, transform.position);
    }
}
