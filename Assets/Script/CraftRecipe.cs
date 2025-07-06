using UnityEngine;

/// <summary>
/// Crafting sistemi için temel veri nesnesi.
/// Şimdilik yalnızca "1 mahsul + crystal" → "çıktı prefab" mantığına göre basit tutuldu.
/// Gerekirse malzeme listesi veya farklı maliyet tipleri eklenebilir.
/// </summary>
[CreateAssetMenu(menuName = "Farm/Craft Recipe", fileName = "NewCraftRecipe")]
public class CraftRecipe : ScriptableObject
{
    [Header("Girdi (Input)")]
    [Tooltip("Gereken mahsul (örn. Pumpkin)")]
    public CropData inputCrop;

    [Tooltip("Kaç adet mahsul gerekli?")]
    [Min(1)] public int cropAmount = 1;

    [Tooltip("Gereken kristal sayısı")]
    public int crystalAmount = 1;

    [Header("Çıktı (Output)")]
    [Tooltip("Üretilecek eşya prefabı (örn. PumpkinBomb)")]
    public GameObject outputPrefab;

    [Tooltip("Kaç adet üretilecek? (varsayılan 1)")]
    [Min(1)] public int outputAmount = 1;
}
