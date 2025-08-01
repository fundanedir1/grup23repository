using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Entry")]
public class ShopEntry : ScriptableObject
{
    public enum Category { Seed, Item, Upgrade }

    [Header("General")]
    public string displayName;          // düğmede görünecek isim
    public Sprite icon;
    public Category category;
    [Min(1)] public int price = 5;

    [Header("Seed / Item")]
    public ItemData giveItem;
    [Min(1)] public int giveAmount = 1;

    [Header("Upgrade")]
    public string upgradeKey;           // örn. "PlotRow3"
    public GameObject unlockPrefab;     // devre dışı tarla prefabı

    // Kolaylık - kart tool-tip’i için
    public string GetDescription() =>
        category switch
        {
            Category.Seed    => $"x{giveAmount} seed\nPrice: {price}$",
            Category.Item    => $"x{giveAmount}\nPrice: {price}$",
            Category.Upgrade => $"Unlocks upgrade\nPrice: {price}$",
            _ => ""
        };
}
