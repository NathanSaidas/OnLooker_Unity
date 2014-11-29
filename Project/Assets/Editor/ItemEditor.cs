using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace Gem
{

    public class ItemEditor
    {
        [MenuItem("Tools/Create Item")]
        public static void CreateAbilityAsset()
        {
            if (!Directory.Exists(Application.dataPath + "\\Items\\"))
            {
                AssetDatabase.CreateFolder("Assets", "Items");
            }

            //Debug.Log(Application.dataPath);
            Item asset = ScriptableObject.CreateInstance<Item>();

            AssetDatabase.CreateAsset(asset, "Assets/Items/NewItem.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}