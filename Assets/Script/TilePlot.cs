using System.Collections;
using UnityEngine;

/// <summary>
/// TilePlot – ekim (plant), çok aşamalı büyüme (grow) ve hasat (harvest) döngüsünü yönetir.
/// Artık dünya-kaynaklı UI yerine tek bir merkezi InteractUI kullanır.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TilePlot : MonoBehaviour
{
    private enum PlotState { Empty, Planting, Growing, Ripe, Harvesting }

    [Header("Prefab Placement")]
    [Tooltip("Prefab sahnelenirken eklenecek local offset")]
    [SerializeField] private Vector3 prefabOffset = Vector3.zero;
    [Tooltip("XZ düzleminde renderer merkezine otomatik hizalama")]
    [SerializeField] private bool autoCenterXZ = true;

    private PlotState state = PlotState.Empty;
    private CropData crop;
    private int daysGrown;
    private int currentStage;
    private int daysPerStage;
    private Coroutine routine;
    private Collider player;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnStateChanged;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Day && state == PlotState.Growing)
            AdvanceGrowth();
    }

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
            InteractUI.Instance.HidePrompt();
        }
    }

    private void Update()
    {
        if (player == null || !Input.GetKeyDown(KeyCode.E)) return;
        var inv = player.GetComponent<QuickBarInventory>();
        if (inv == null) return;

        bool isDay = GameManager.Instance == null || GameManager.Instance.CurrentState == GameManager.GameState.Day;
        if (state == PlotState.Empty && isDay)
            TryPlant(inv);
        else if (state == PlotState.Ripe)
            TryHarvest(inv);
    }

    private void UpdatePrompt()
    {
        if (player == null) return;
        bool isDay = GameManager.Instance == null || GameManager.Instance.CurrentState == GameManager.GameState.Day;
        string text = state switch
        {
            PlotState.Empty when isDay => "E - Plant",
            PlotState.Ripe => "E - Harvest",
            _ => string.Empty
        };
        if (!string.IsNullOrEmpty(text))
            InteractUI.Instance.ShowPrompt(text, transform.position);
        else
            InteractUI.Instance.HidePrompt();
    }

    private void TryPlant(QuickBarInventory inv)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Day)
            return; // gece ekim yok

        ItemData it = inv.ActiveItem;
        if (it == null || it.cropToPlant == null) return;
        if (!inv.RemoveActive(1)) return;

        crop = it.cropToPlant;
        state = PlotState.Planting;
        StartProgress(crop.plantingSeconds, () =>
        {
            SpawnStage(0);
            currentStage = 0;
            daysGrown = 0;
            daysPerStage = Mathf.Max(1, Mathf.CeilToInt((float)crop.daysToRipe / (crop.growthPrefabs.Length - 1)));
            state = PlotState.Growing;
            UpdatePrompt();
        });
    }

    private void AdvanceGrowth()
    {
        daysGrown++;
        int target = Mathf.Min(crop.growthPrefabs.Length - 1, 1 + daysGrown / daysPerStage);
        if (target != currentStage)
        {
            SpawnStage(target);
            currentStage = target;
        }
        if (currentStage == crop.growthPrefabs.Length - 1)
            state = PlotState.Ripe;
        UpdatePrompt();
    }

    private void SpawnStage(int idx)
    {
        foreach (Transform c in transform) Destroy(c.gameObject);
        var pf = crop.GetStagePrefab(idx);
        if (pf == null) return;
        var go = Instantiate(pf, transform);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localPosition = prefabOffset;
        if (autoCenterXZ && go.GetComponentInChildren<Renderer>() is Renderer r)
        {
            var wc = r.bounds.center;
            var lc = go.transform.InverseTransformPoint(wc);
            go.transform.localPosition -= new Vector3(lc.x, 0, lc.z);
        }
    }

    private void TryHarvest(QuickBarInventory inv)
    {
        state = PlotState.Harvesting;
        StartProgress(crop.harvestSeconds, () =>
        {
            int rem = inv.AddItem(crop.harvestItem, crop.harvestAmount);
            if (rem > 0 && crop.harvestItem.worldPrefab)
                Instantiate(crop.harvestItem.worldPrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity)
                    .GetComponent<ItemWorld>().Initialize(crop.harvestItem, rem);
            ResetPlot();
        });
    }

    private void ResetPlot()
    {
        foreach (Transform c in transform) Destroy(c.gameObject);
        crop = null; state = PlotState.Empty; currentStage = 0; daysGrown = 0;
        InteractUI.Instance.HidePrompt();
    }

    private void StartProgress(float sec, System.Action onComplete)
    {
        InteractUI.Instance.ShowProgress(sec);
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ProgressRoutine(sec, onComplete));
    }
    private IEnumerator ProgressRoutine(float sec, System.Action cb)
    {
        float timer = 0f;
        while (timer < sec)
        {
            timer += Time.deltaTime;
            InteractUI.Instance.SetProgress(timer / sec);
            yield return null;
        }
        InteractUI.Instance.HideProgress();
        cb?.Invoke();
    }
}
