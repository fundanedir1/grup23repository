using EasyPeasyFirstPersonController;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FirstPersonController))]
public class ReviewWindowEditor : Editor
{
    private static string assetStoreUrl = "https://assetstore.unity.com/packages/slug/317073 "; 

    static ReviewWindowEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DrawReviewIcon;
    }

    private static void DrawReviewIcon(int instanceID, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null) return;

        if (go.GetComponent<FirstPersonController>() != null)
        {
            Rect iconRect = new Rect(selectionRect.xMax - 20, selectionRect.y, 16, 16);
            if (GUI.Button(iconRect, EditorGUIUtility.IconContent("Favorite", "| Rate this asset on Asset Store"), GUIStyle.none))
            {
                Application.OpenURL(assetStoreUrl);
            }
        }
    }
}