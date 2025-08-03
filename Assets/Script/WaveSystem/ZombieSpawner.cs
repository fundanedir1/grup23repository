using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gece başladığında dalga hâlinde zombi üreten basit spawner.
/// GameManager, Night durumuna geçerken StartNightWave() çağırır.
/// </summary>
public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private float spawnInterval = 0.3f;

    public void SpawnBatch(GameObject prefab, int amount)
    {
        StartCoroutine(SpawnRoutine(prefab, amount));
    }

    private IEnumerator SpawnRoutine(GameObject prefab, int amount)
    {
        if (prefab == null || spawnPoints.Count == 0) yield break;
        for (int i = 0; i < amount; i++)
        {
            var p = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Instantiate(prefab, p.position, p.rotation);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

