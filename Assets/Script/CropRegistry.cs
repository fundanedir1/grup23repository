using System.Collections.Generic;
using UnityEngine;

/// <summary> Scene’de ekili (Growing/Ripe) tüm TilePlot’ları kaydeder. </summary>
public static class CropRegistry
{
    private static readonly HashSet<TilePlot> plots = new();

    public static void Register(TilePlot p)   => plots.Add(p);
    public static void Unregister(TilePlot p) => plots.Remove(p);

    public static TilePlot GetNearestOccupied(Vector3 pos)
    {
        TilePlot best = null;
        float bestSqr = float.MaxValue;

        foreach (var p in plots)
        {
            if (!p.HasCrop) continue;                     // boşsa geç
            float d = (p.transform.position - pos).sqrMagnitude;
            if (d < bestSqr) { best = p; bestSqr = d; }
        }
        return best;
    }
}
