using UnityEngine;
using System.Collections.Generic;

public static class UpgradeManager
{
    private static readonly HashSet<string> unlocked = new();

    public static void Unlock(string key, GameObject prefabToActivate)
    {
        if (unlocked.Contains(key)) return;
        unlocked.Add(key);
        if (prefabToActivate) prefabToActivate.SetActive(true);
    }
}
