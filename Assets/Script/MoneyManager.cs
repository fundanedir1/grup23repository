using UnityEngine;

/// <summary>
/// Static treasury. Holds current money and broadcasts change events.
/// **Do NOT attach this script to a GameObject.**
/// </summary>
public static class MoneyManager
{
    public static int Money { get; private set; }

    /// <summary>Fires whenever Money value changes.</summary>
    public static System.Action<int> OnMoneyChanged;

    /// <summary>Add positive amount to wallet.</summary>
    public static void Add(int amount)
    {
        if (amount <= 0) return;
        Money += amount;
        OnMoneyChanged?.Invoke(Money);
        Debug.Log($"Money +{amount} → {Money}");
    }

    /// <summary>Try to spend; returns true if successful.</summary>
    public static bool Spend(int amount)
    {
        if (amount <= 0 || Money < amount) return false;
        Money -= amount;
        OnMoneyChanged?.Invoke(Money);
        Debug.Log($"Spent {amount} → {Money}");
        return true;
    }
}
