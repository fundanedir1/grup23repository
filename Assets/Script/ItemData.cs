using UnityEngine;

/// <summary>
/// Oyuncunun envanterinde taşıyabileceği her nesne için temel veri.
/// Bu ScriptableObject, ikon, prefab, stack limiti gibi bilgileri barındırır.
/// </summary>
[CreateAssetMenu(menuName = "Items/Item Data", fileName = "NewItemData")]
public class ItemData : ScriptableObject
{
    [Header("Genel Bilgiler")]
    public string itemName = "New Item";
    public Sprite icon; // Hotbar UI'da gösterilecek 2D ikon

    [Header("Dünya Prefabı")]
    [Tooltip("Yere atıldığında instantiate edilecek prefab (null ise generic prefab kullanılır)")]
    public GameObject worldPrefab;

    [Header("Stack")]
    [Min(1)] public int stackMax = 99; // Aynı yuvada taşınabilecek maksimum miktar

    [Header("Tohum Ayarları")]
    [Tooltip("Bu nesne tohum ise true yapın – Plot.Plant sırasında kontrol edilir")]
    public bool isSeed = false;

    [Tooltip("Tohumsa hangi CropData'yı eker? (Pumpkin, Corn vs.)")]
    public CropData cropRef; // isSeed=false iken boş bırakın
    
    [Header("Tohum Ayarları")]
    [Tooltip("Bu alan NULL değilse; item tohum görevi görür ve bu CropData ekilir.")]
    public CropData cropToPlant;
    
}
