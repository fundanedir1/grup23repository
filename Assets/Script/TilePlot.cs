using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tarla karesi: Empty → Planting → Growing → Ripe.
/// • Player yaklaşınca uygun prompt gösterir (E tuşu).
/// • E ile ekme veya hasat başlatılır, ikisi de süreli progress bar ile yapılır.
/// • Büyüme, GameManager'ın Day → Night geçişlerinde gün sayarak işler.
/// Gereksinim: CropData'da plantingSeconds, daysToRipe, harvestSeconds, harvestAmount, harvestItem alanları.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TilePlot : MonoBehaviour
{
    private enum PlotState { Empty, Planting, Growing, Ripe, Harvesting }

    [Header("World‑Space UI")]
    [SerializeField] private Canvas   worldCanvas;
    [SerializeField] private TMP_Text promptTxt;
    [SerializeField] private Slider   progressSlider;

    private PlotState state = PlotState.Empty;
    private CropData  crop;            // ekili ekin verisi
    private int       daysGrown;       // sulanmış gün sayısı
    private Coroutine activeRoutine;
    private Collider  player;

    //────────────── Unity ──────────────
    private void Awake()
    {
        if (worldCanvas)    worldCanvas.enabled = false;
        if (progressSlider) progressSlider.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if (GameManager.Instance)
            GameManager.Instance.OnStateChanged += HandleStateChange;
    }
    private void OnDisable()
    {
        if (GameManager.Instance)
            GameManager.Instance.OnStateChanged -= HandleStateChange;
    }

    //────────────── Trigger ──────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        player = other;
        UpdatePrompt();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other == player)
        {
            player = null;
            ShowPrompt(false);
        }
    }

    //────────────── Input ──────────────
    private void Update()
    {
        if (player == null) return;
        if (!Input.GetKeyDown(KeyCode.E)) return;

        var inv = player.GetComponent<QuickBarInventory>();
        if (inv == null) return;

        switch (state)
        {
            case PlotState.Empty:
                TryPlant(inv);
                break;
            case PlotState.Ripe:
                TryHarvest(inv);
                break;
        }
    }

    //────────────── Planting ──────────────
    private void TryPlant(QuickBarInventory inv)
    {
        ItemData activeItem = inv.ActiveItem;
        if (activeItem == null || activeItem.cropToPlant == null) return;
        crop = activeItem.cropToPlant;
        if (!inv.RemoveActive(1)) return;

        state = PlotState.Planting;
        StartProgress(crop.plantingSeconds, () =>
        {
            Instantiate(crop.seedPrefab, transform.position, Quaternion.identity, transform);
            state = PlotState.Growing;
            daysGrown = 0;
            UpdatePrompt();
        });
    }

    //────────────── Growing (Day tick) ──────────────
    private void HandleStateChange(GameManager.GameState gs)
    {
        if (state != PlotState.Growing) return;
        if (gs != GameManager.GameState.Day) return; // gece bitti sabah oldu

        daysGrown++;
        if (daysGrown >= crop.daysToRipe)
        {
            // Olgun prefab oluştur, tohum modelini yok et (ilk child)
            foreach (Transform child in transform) Destroy(child.gameObject);
            Instantiate(crop.GetStagePrefab(crop.growthPrefabs.Length - 1), transform.position, Quaternion.identity, transform);
            state = PlotState.Ripe;
            UpdatePrompt();
        }
    }

    //────────────── Harvest ──────────────
    private void TryHarvest(QuickBarInventory inv)
    {
        state = PlotState.Harvesting;
        StartProgress(crop.harvestSeconds, () =>
        {
            int remaining = inv.AddItem(crop.harvestItem, crop.harvestAmount);
            if (remaining > 0)
            {
                // Envanter dolu – yerde ItemWorld bırak
                if (crop.harvestItem.worldPrefab)
                    Instantiate(crop.harvestItem.worldPrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity)
                        .GetComponent<ItemWorld>().Initialize(crop.harvestItem, remaining);
            }
            // Alanı sıfırla
            foreach (Transform child in transform) Destroy(child.gameObject);
            state = PlotState.Empty;
            crop  = null;
            UpdatePrompt();
        });
    }

    //────────────── Progress Utility ──────────────
    private void StartProgress(float seconds, System.Action onComplete)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(ProgressRoutine(seconds, onComplete));
    }
    private IEnumerator ProgressRoutine(float sec, System.Action onComplete)
    {
        if (progressSlider)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(true);
        }
        for (float t = 0f; t < sec; t += Time.deltaTime)
        {
            if (progressSlider) progressSlider.value = t / sec;
            yield return null;
        }
        if (progressSlider) progressSlider.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    //────────────── Prompt Helper ──────────────
    private void UpdatePrompt()
    {
        if (player == null) { ShowPrompt(false); return; }
        switch (state)
        {
            case PlotState.Empty:
                promptTxt.text = "E – Plant seed";
                ShowPrompt(true);
                break;
            case PlotState.Ripe:
                promptTxt.text = "E – Harvest";
                ShowPrompt(true);
                break;
            default:
                ShowPrompt(false);
                break;
        }
    }
    private void ShowPrompt(bool show)
    {
        if (worldCanvas) worldCanvas.enabled = show;
    }
}
