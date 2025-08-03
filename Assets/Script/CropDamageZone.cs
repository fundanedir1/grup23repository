using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Attach this to a child GameObject with a trigger collider that fully covers the TilePlot.
/// • If one or more Enemy colliders stay inside for <see cref="CropData.zombieDamageDuration"/> seconds,
///   the plot's crop is destroyed (KillCrop()).
/// • Each enemy has its own timer; first to finish kills the crop.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CropDamageZone : MonoBehaviour
{
    [Tooltip("Reference to the TilePlot this zone belongs to")]
    [SerializeField] private TilePlot plot;

    // Per‑enemy timer store
    private readonly Dictionary<Collider, float> timers = new();

    private void Awake()
    {
        if (plot == null) plot = GetComponentInParent<TilePlot>();
        if (!GetComponent<Collider>().isTrigger)
            Debug.LogWarning("CropDamageZone collider MUST be IsTrigger=TRUE", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (!plot || !plot.HasCrop) return;              // empty plot → do nothing
        timers[other] = 0f;                              // start timer for this enemy
        Debug.Log($"CropDamageZone: {other.name} entered, starting timer for {plot.CurrentCropData?.cropName}");
    }

    private void OnTriggerExit(Collider other)
    {
        timers.Remove(other);                            // leaving ⇒ reset timer
        Debug.Log($"CropDamageZone: {other.name} exited, removing timer for {plot.CurrentCropData?.cropName}");
    }

    private void Update()
    {
        if (timers.Count == 0 || !plot || !plot.HasCrop) return;

        // Copy keys to avoid collection‑modified exception
        var keys = new List<Collider>(timers.Keys);
        foreach (var col in keys)
        {
            if (!col) { timers.Remove(col); continue; }
            timers[col] += Time.deltaTime;

            // Destroy crop when any timer >= threshold
            float thresh = plot.CurrentCropData?.zombieDamageDuration ?? 10f;
            if (timers[col] >= thresh)
            {
                plot.KillCrop();           // reset & unregister automatically
                timers.Clear();            // stop further counting
                return;
            }
        }
    }
}
