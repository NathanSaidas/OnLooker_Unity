﻿using UnityEngine;
using UnityEditor;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {
        [CustomEditor(typeof(UIText))]
        public class UITextEditor : Editor
        {

            public override void OnInspectorGUI()
            {

                

                DrawDefaultInspector();

                UIText inspected = (UIText)target;
                inspected.init();

                

                inspected.font = OLEditorUtilities.fontField("Font", inspected.font);
                inspected.fontStyle = OLEditorUtilities.fontStyleEnum("Font Style", inspected.fontStyle);
                inspected.fontSize = EditorGUILayout.IntField("Font Size", inspected.fontSize);
                inspected.fontColor = EditorGUILayout.ColorField("Font Color", inspected.fontColor);
                inspected.text = EditorGUILayout.TextField("Text", inspected.text);

                bool smoothTransform = inspected.smoothTransform;
                inspected.smoothTransform = false;
                inspected.updateTransform();
                inspected.smoothTransform = smoothTransform;


                if (inspected.isInteractive == true)
                {
                    inspected.gameObject.layer = UIManager.UI_LAYER;
                }
                inspected.updateText();
            }
        }
    }
}