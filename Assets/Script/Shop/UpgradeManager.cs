using System.Collections.Generic;
using UnityEngine;

public static class UpgradeManager
{
    private static readonly HashSet<string> unlocked = new();

    public static bool IsUnlocked(string key) => unlocked.Contains(key);

    public static void Unlock(string key, GameObject prefab)
    {
        if (string.IsNullOrEmpty(key) || unlocked.Contains(key)) return;
        unlocked.Add(key);

        if (!prefab) return;

        // Scene object → sadece aktifleştir
        if (prefab.scene.IsValid())
        {
            prefab.SetActive(true);
            return;
        }

        // Prefab asset → instantiate at spawn point (tag = TrapSpawn) or (0,0,0)
        Vector3  pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        var sp = GameObject.FindGameObjectWithTag("TrapSpawn");
        if (sp) { pos = sp.transform.position; rot = sp.transform.rotation; }

        Object.Instantiate(prefab, pos, rot);
    }
}
