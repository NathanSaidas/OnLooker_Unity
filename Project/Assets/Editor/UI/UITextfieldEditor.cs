using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace OnLooker
{
    namespace UI
    {
        [CustomEditor(typeof(UITextfield))]
        public class UITextfieldEditor : Editor
        {

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                UITextfield inspected = (UITextfield)target;

                inspected.textComponent = (UIText)EditorGUILayout.ObjectField("Text Component", inspected.textComponent, typeof(UIText), true);
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

                inspected.text = EditorGUILayout.TextField("Text", inspected.text);
                inspected.backgroundTexture = OLEditorUtilities.textureField("Background Texture", inspected.backgroundTexture);
                inspected.backgroundColor = EditorGUILayout.ColorField("Background Color", inspected.backgroundColor);
                inspected.textComponent.updateText();
                inspected.updateBackground();
            }
        }
    }
}