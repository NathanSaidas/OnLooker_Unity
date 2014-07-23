using UnityEngine;
using UnityEditor;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {

        [CustomEditor(typeof(UIImage))]
        public class UIImageEditor : Editor
        {

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                UIImage inspected = (UIImage)target;
                inspected.textureComponent = (UITexture)EditorGUILayout.ObjectField("Texture Component", inspected.textureComponent, typeof(UITexture), true);
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

                inspected.texture = OLEditorUtilities.textureField("Background Texture", inspected.texture);
                inspected.color = EditorGUILayout.ColorField("Background Color", inspected.color);
                
            }
        }
    }
}