using UnityEngine;
using UnityEditor;

public static class EditorUtilities 
{
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


}
