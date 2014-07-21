using UnityEngine;
using UnityEditor;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {
        [CustomEditor(typeof(UILabel))]
        public class UILabelEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                UILabel inspected = (UILabel)target;
                inspected.init();

                inspected.offsetPosition = EditorGUILayout.Vector3Field("Position", inspected.offsetPosition);
                inspected.offsetRotation = EditorGUILayout.Vector3Field("Rotation", inspected.offsetRotation);
                inspected.anchorTarget = OLEditorUtilities.transformField("Anchor Target", inspected.anchorTarget);
                inspected.anchorMode = (UIAnchor)EditorGUILayout.EnumPopup("Anchor Mode", inspected.anchorMode);
                inspected.faceCamera = EditorGUILayout.Toggle("Face Camera", inspected.faceCamera);
                bool smoothTransform = EditorGUILayout.Toggle("Smooth Transform", inspected.smoothTransform);

                inspected.smoothTransform = false;
                inspected.updateTransform();
                inspected.smoothTransform = smoothTransform;

                inspected.text = EditorGUILayout.TextField("Text", inspected.text);
                inspected.backgroundTexture = OLEditorUtilities.textureField("Background Texture", inspected.backgroundTexture);

                inspected.textComponent.updateText();
                inspected.updateBackground();
            }
        }


    }//End Namespace
}//End Namespace