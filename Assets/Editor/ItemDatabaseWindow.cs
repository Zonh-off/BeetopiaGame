using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ItemDatabaseWindow : EditorWindow
{
    private string folderPath = "Assets/Beetopia/ScriptableObjects/";
    private List<ItemSO> itemList = new List<ItemSO>();

    [MenuItem("Tools/Item Database Viewer")]
    public static void ShowWindow()
    {
        GetWindow<ItemDatabaseWindow>("Item Database Viewer");
    }

    private void OnGUI()
    {
        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

        if (GUILayout.Button("Знайти всі предмети"))
        {
            FindAllItemsInFolder();
        }

        if (itemList.Count > 0)
        {
            EditorGUILayout.LabelField("Предмети в папці:");
            foreach (var item in itemList)
            {
                if(item != null)
                    EditorGUILayout.LabelField(item.name);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Немає предметів в папці.");
        }
    }

    private void FindAllItemsInFolder()
    {
        itemList.Clear();
        string[] guids = AssetDatabase.FindAssets("t:ItemSO", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ItemSO item = AssetDatabase.LoadAssetAtPath<ItemSO>(assetPath);
            if(item != null)
                itemList.Add(item);
        }
    }
}