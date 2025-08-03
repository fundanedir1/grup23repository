using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class PrefabObjectCounter : EditorWindow
{
    enum SortOption { ByName, ByCount };
    private SortOption sortOption = SortOption.ByCount;
    
    private List<PrefabCount> prefabCounts = new List<PrefabCount>();
    private Vector2 scrollPos;

    [MenuItem("Tools/Prefab Object Counter")]
    static void Init()
    {
        GetWindow<PrefabObjectCounter>("Prefab Counter");
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab Object Counter", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Sıralama seçenekleri
        EditorGUILayout.LabelField("Sorting Options", EditorStyles.miniBoldLabel);
        sortOption = (SortOption)EditorGUILayout.EnumPopup("Sort By:", sortOption);
        EditorGUILayout.Space();
        
        // Sayım butonu
        if (GUILayout.Button("Count Prefab Instances", GUILayout.Height(30)))
        {
            CountPrefabs();
        }
        
        // Sonuçları göster
        if (prefabCounts.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Results (Click to select):", EditorStyles.boldLabel);
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
            
            foreach (var item in prefabCounts)
            {
                // Prefab adını ve sayısını göster
                if (GUILayout.Button($"{item.PrefabName}: {item.Count}", GUILayout.Height(25)))
                {
                    SelectPrefabs(item.PrefabAssetPath);
                }
            }
            
            EditorGUILayout.EndScrollView();
            
            int totalObjects = prefabCounts.Sum(item => item.Count);
            EditorGUILayout.LabelField($"Total Prefab Instances: {totalObjects}", EditorStyles.miniLabel);
        }
    }

    void CountPrefabs()
    {
        // Tüm sahne objelerini al
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        
        // Sadece prefab köklerini al (child'ları dahil etme)
        var prefabRoots = allObjects
            .Where(obj => PrefabUtility.IsPartOfPrefabInstance(obj))
            .Select(obj => PrefabUtility.GetNearestPrefabInstanceRoot(obj))
            .Where(root => root != null)
            .Distinct() // Aynı kökü birden fazla sayma
            .ToArray();
        
        // Prefab asset yollarına göre grupla
        var grouped = prefabRoots
            .GroupBy(root => PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root))
            .Where(group => !string.IsNullOrEmpty(group.Key))
            .Select(group => new PrefabCount(
                System.IO.Path.GetFileNameWithoutExtension(group.Key),
                group.Key,
                group.Count()
            ))
            .ToList();
        
        // Seçilen kritere göre sırala
        prefabCounts = sortOption == SortOption.ByCount 
            ? grouped.OrderByDescending(item => item.Count).ToList()
            : grouped.OrderBy(item => item.PrefabName).ToList();
    }

    void SelectPrefabs(string prefabAssetPath)
    {
        // Tüm prefab köklerini seç
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        var roots = allObjects
            .Where(obj => PrefabUtility.IsPartOfPrefabInstance(obj))
            .Select(obj => PrefabUtility.GetNearestPrefabInstanceRoot(obj))
            .Where(root => root != null)
            .Distinct()
            .Where(root => PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root) == prefabAssetPath)
            .ToArray();
        
        Selection.objects = roots;
        
        if (roots.Length > 0)
        {
            EditorGUIUtility.PingObject(roots[0]);
        }
    }

    class PrefabCount
    {
        public string PrefabName;
        public string PrefabAssetPath;
        public int Count;
        
        public PrefabCount(string prefabName, string prefabAssetPath, int count)
        {
            PrefabName = prefabName;
            PrefabAssetPath = prefabAssetPath;
            Count = count;
        }
    }
}