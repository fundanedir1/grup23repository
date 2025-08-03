using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds one wave entry – prefab & amount.
/// </summary>
[System.Serializable]
public class ZombieBatch
{
    public GameObject prefab;
    [Min(1)] public int amount = 1;
}

/// <summary>
/// ScriptableObject that defines a single night wave.
/// </summary>
[CreateAssetMenu(menuName = "Game/Wave Config", fileName = "WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [Tooltip("Batches that will be spawned sequentially. Order matters.")]
    public ZombieBatch[] batches;
}