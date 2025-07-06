using UnityEngine;

/// <summary>
/// Çiftlikte ekilebilen ürünlerin verilerini tutar.
/// ScriptableObject olduğu için tasarımcı değerleri Inspector’da rahatça değiştirir.
/// </summary>
[CreateAssetMenu(menuName = "Farm/Crop Data", fileName = "NewCropData")]
public class CropData : ScriptableObject
{
    [Header("Genel Bilgiler")]
    public string cropName = "Pumpkin";
    [Tooltip("Tarlaya ekildiğinde hangi prefab kullanılacak? (Seed modeli)")]
    public GameObject seedPrefab;

    [Header("Büyüme Aşamaları")]
    [Tooltip("0: Seed, 1..n: Ara aşamalar, Son: Olgun model. Spoiled modeli isteğe bağlı son dizi elemanı olabilir.")]
    public GameObject[] growthPrefabs;

    [Tooltip("Tam olgunlaşmaya kadar gereken GÜN sayısı. (Her sulanan gün 1 gün sayılır)")]
    [Min(1)] public int daysToRipe = 3;

    [Header("Ekonomi")]
    public int sellPrice = 10;
    [Tooltip("Bu ürün craft station’da hangi tarife dönüşür?")]
    public CraftRecipe craftOutput;

    [Header("Bozulma Ayarları")]
    [Tooltip("Hasat vakti gelmişken sulanmazsa bu kadar gün sonra çürür.")]
    [Min(0)] public int spoilAfterDays = 1;

    [Header("Zombi Saldırısı")]
    [Tooltip("Zombi bu kadar saniye boyunca saldırırsa ürün çürür.")]
    public float zombieDamageDuration = 10f;

    /// <summary>
    /// Prefab dizisi güvenlik kontrolü yapar.
    /// </summary>
    public GameObject GetStagePrefab(int stageIndex)
    {
        if (growthPrefabs == null || growthPrefabs.Length == 0) return null;
        stageIndex = Mathf.Clamp(stageIndex, 0, growthPrefabs.Length - 1);
        return growthPrefabs[stageIndex];
    }
}
