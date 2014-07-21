using UnityEngine;
using UnityEditor;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {
        [CustomEditor(typeof(UITexture))]
        public class UITextureEditor : Editor
        {

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                UITexture inspected = (UITexture)target;

                inspected.init();

                inspected.texture = OLEditorUtilities.textureField("Texture", inspected.texture);
                inspected.color = EditorGUILayout.ColorField("Color", inspected.color);

                bool smoothTransform = inspected.smoothTransform;
                inspected.smoothTransform = false;
                inspected.updateTransform();
                inspected.smoothTransform = smoothTransform;

            }
        }
    }
}
