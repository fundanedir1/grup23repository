using UnityEngine;

/// <summary>
/// Single purchasable entry for the in‑game shop.
/// </summary>
[CreateAssetMenu(menuName = "Shop/Entry")]
public class ShopEntry : ScriptableObject
{
    public enum Category { Seed, Item, Upgrade }

    [Header("General")]
    public string      displayName;        // Button title
    public Sprite      icon;
    public Category    category;
    [Min(1)] public int price = 5;

    [Header("Seed / Item")]
    public ItemData giveItem;
    [Min(1)] public int giveAmount = 1;

    [Header("Upgrade")]
    [Tooltip("Unique key to mark this upgrade as unlocked")]  public string upgradeKey;
    [Tooltip("Disabled prefab in scene that becomes active once purchased")]
    public GameObject unlockPrefab;

    // Handy tooltip helper
    public string GetDescription() =>
        category switch
        {
            Category.Seed    => $"x{giveAmount} seed\nPrice: {price}$",
            Category.Item    => $"x{giveAmount}\nPrice: {price}$",
            Category.Upgrade => $"Unlocks upgrade\nPrice: {price}$",
            _ => string.Empty
        };
}