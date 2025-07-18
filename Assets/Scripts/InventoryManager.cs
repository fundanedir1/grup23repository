using UnityEngine;

[System.Serializable]
public class SeedData
{
    public string seedName;
    public Sprite icon;
    public GameObject plantPrefab;
}

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private SeedData[] seeds;   // ?? Art?k Inspector�da g�r�necek
    private int selectedSeedIndex = 0;

    public void SelectSeed(int index)
    {
        selectedSeedIndex = index;
        Debug.Log("Se�ilen tohum: " + seeds[index].seedName);
    }

    public SeedData GetSelectedSeed()
    {
        return seeds[selectedSeedIndex];
    }
}
