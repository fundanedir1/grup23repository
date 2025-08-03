// Assets/ScriptableObjects/SeedItemData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Farm/Seed Item")]
public class SeedItemData : ItemData   // ItemData senin zaten var
{
    [Tooltip("Bu tohumdan büyüyecek ekin verisi")]
    public CropData cropData;           // TilePlot, ekim için bunu kullanacak
}
