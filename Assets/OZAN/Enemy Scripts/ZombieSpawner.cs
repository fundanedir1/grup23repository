using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gece başladığında dalga hâlinde zombi üreten basit spawner.
/// GameManager, Night durumuna geçerken StartNightWave() çağırır.
/// </summary>
public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    [Tooltip("Zombinin doğacağı noktalar (boş GameObject ler)")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("Çoğaltılacak zombi prefabı")]
    [SerializeField] private GameObject zombiePrefab;

    [Tooltip("Aynı dalgadaki iki spawn arasındaki saniye")]
    [SerializeField] private float spawnInterval = 0.5f;

    private int currentWave = 1;
    private Coroutine spawnRoutine;

    /// <summary>
    /// GameManager.Night aşamasında tetiklenir.
    /// Dalga büyüklüğü = currentWave * 3 gibi basit bir formül.
    /// </summary>
    public void StartNightWave()
    {
        if (zombiePrefab == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("ZombieSpawner: Prefab veya spawnPoints eksik");
            return;
        }

        int waveSize = currentWave * 3;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnWave(waveSize));
        currentWave++;
    }

    private IEnumerator SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnSingle();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnSingle()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(zombiePrefab, point.position, point.rotation);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var p in spawnPoints)
        {
            if (p != null)
                Gizmos.DrawWireSphere(p.position, 0.3f);
        }
    }
#endif
}
