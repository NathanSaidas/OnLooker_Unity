using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;


namespace Gem
{

    //[CustomEditor(typeof(Ability))]
    public class AbilityEditor //: Editor
    {

        [MenuItem("Tools/Create Ability/Ability")]
        public static void CreateAbilityAsset()
        {
            if(!Directory.Exists(Application.dataPath + "\\Abilities\\"))
            {
                AssetDatabase.CreateFolder("Assets", "Abilities");
            }
            //Debug.Log(Application.dataPath);
            Ability asset = ScriptableObject.CreateInstance<Ability>();
            
            AssetDatabase.CreateAsset(asset,"Assets/Abilities/NewAbility.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        [MenuItem("Tools/Create Ability/Basic Attack")]
        public static void CreateBasicAttackAsset()
        {
            if (!Directory.Exists(Application.dataPath + "\\Abilities\\"))
            {
                AssetDatabase.CreateFolder("Assets", "Abilities");
            }

            //Debug.Log(Application.dataPath);
            BasicAttack asset = ScriptableObject.CreateInstance<BasicAttack>();

            AssetDatabase.CreateAsset(asset, "Assets/Abilities/NewBasicAttack.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        [MenuItem("Tools/Create Ability/Back Stab")]
        public static void CreateBackStabAsset()
        {
            if (!Directory.Exists(Application.dataPath + "\\Abilities\\"))
            {
                AssetDatabase.CreateFolder("Assets", "Abilities");
            }

            //Debug.Log(Application.dataPath);
            Backstab asset = ScriptableObject.CreateInstance<Backstab>();
            asset.name = "Back_Stab";
            AssetDatabase.CreateAsset(asset, "Assets/Abilities/NewBackstab.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        [MenuItem("Tools/Create Ability/Gravity")]
        public static void CreateGravityAsset()
        {
            if (!Directory.Exists(Application.dataPath + "\\Abilities\\"))
            {
                AssetDatabase.CreateFolder("Assets", "Abilities");
            }
            Gravity asset = ScriptableObject.CreateInstance<Gravity>();
            asset.name = "Gravity";
            AssetDatabase.CreateAsset(asset, "Assets/Abilities/NewGravity.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        [MenuItem("Tools/Create Ability/PowerSurge")]
        public static void CreatePowerSurgeAsset()
        {
            if (!Directory.Exists(Application.dataPath + "\\Abilities\\"))
            {
                AssetDatabase.CreateFolder("Assets", "Abilities");
            }
            PowerSurge asset = ScriptableObject.CreateInstance<PowerSurge>();
            asset.name = "PowerSurge";
            AssetDatabase.CreateAsset(asset, "Assets/Abilities/NewPowerSurge.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        //public override void OnInspectorGUI()
        //{
        //    Debug.Log("Inspector");
        //}
        
    }
}