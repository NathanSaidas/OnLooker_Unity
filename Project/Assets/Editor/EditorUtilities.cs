using UnityEngine;
using UnityEditor;

namespace Gem
{

    public static class EditorUtilities
    {
        public const string LEFT = "Left";
        public const string RIGHT = "Right";
        public const string TOP = "Top";
        public const string BOTTOM = "Bottom";

        public static T ObjectField<T>(GUIContent aContent, T aGameObject) where T : Object
        {
            return EditorGUILayout.ObjectField(aContent, aGameObject, typeof(T), true) as T;
        }
        public static T ObjectField<T>(string aContent, T aGameObject) where T : Object
        {
            return EditorGUILayout.ObjectField(aContent, aGameObject, typeof(T), true) as T;
        }

        public static GameObject gameObjectField(string aContent, GameObject aGameObject)
        {
            return (GameObject)EditorGUILayout.ObjectField(aContent, aGameObject, typeof(GameObject), true);
        }
        public static Transform transformField(string aContent, Transform aTransform)
        {
            return (Transform)EditorGUILayout.ObjectField(aContent, aTransform, typeof(Transform), true);
        }
        public static Font fontField(string aContent, Font aFont)
        {
            return (Font)EditorGUILayout.ObjectField(aContent, aFont, typeof(Font), true);
        }
        public static FontStyle fontStyleEnum(string aContent, FontStyle aStyle)
        {
            return (FontStyle)EditorGUILayout.EnumPopup(aContent, aStyle);
        }
        public static Texture textureField(string aContent, Texture aTexture)
        {
            return (Texture)EditorGUILayout.ObjectField(aContent, aTexture, typeof(Texture), true);
        }
        public static KeyCode keyCodeField(string aField, KeyCode aContent, float aSpace)
        {
            return (KeyCode)EditorGUILayout.EnumPopup(aField, aContent);
        }
        ///public static UIBoarder 
        public static UIBoarder UIBoarderField(string aContent, UIBoarder aBoarder)
        {
            EditorGUILayout.LabelField(aContent);
            EditorGUILayout.BeginHorizontal();
            aBoarder.left = EditorGUILayout.FloatField(LEFT, aBoarder.left);
            aBoarder.right = EditorGUILayout.FloatField(RIGHT, aBoarder.right);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            aBoarder.top = EditorGUILayout.FloatField(TOP, aBoarder.top);
            aBoarder.bottom = EditorGUILayout.FloatField(BOTTOM, aBoarder.bottom);
            EditorGUILayout.EndHorizontal();
            return aBoarder;
        }

    }
}