using UnityEngine;
using UnityEditor;

namespace OnLooker
{
    namespace UI
    {
        class OLEditorUtilities
        {
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
        }
    }
}
