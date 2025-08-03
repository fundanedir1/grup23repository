using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [Tooltip("Wave configs in chronological order (size = 10)")]
    public WaveConfig[] waves = new WaveConfig[10];

    [Tooltip("Reference to the spawner(s) that will actually instantiate")]
    public ZombieSpawner spawner;

    [Tooltip("Delay between batches within same wave")] public float batchDelay = 1f;

    [SerializeField] private string creditsSceneName = "Credits";

    private int currentWaveIndex = -1; // start before first wave

    private void OnEnable()
    {
        GameManager.Instance.OnStateChanged += HandleGameState;
    }
    private void OnDisable()
    {
        if (GameManager.Instance)
            GameManager.Instance.OnStateChanged -= HandleGameState;
    }

    private void HandleGameState(GameManager.GameState gs)
    {
        if (gs != GameManager.GameState.Night) return;
        StartCoroutine(NightRoutine());
    }

    private IEnumerator NightRoutine()
    {
        currentWaveIndex++;
        if (currentWaveIndex >= waves.Length)
        {
            // All waves done => end game
            GameManager.Instance.TriggerGameOver();
            SceneManager.LoadScene(creditsSceneName);
            yield break;
        }

        // Update UI
        WaveUI.UpdateWave(currentWaveIndex + 1, waves.Length);

        var cfg = waves[currentWaveIndex];
        foreach (var batch in cfg.batches)
        {
            spawner.SpawnBatch(batch.prefab, batch.amount);
            yield return new WaitForSeconds(batchDelay);
        }
    }
}