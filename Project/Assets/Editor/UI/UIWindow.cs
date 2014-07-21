using UnityEngine;
using UnityEditor;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {
        public class UIWindow : EditorWindow
        {


            [MenuItem("OnLooker/UI")]
            public static void ShowWindow()
            {
                UIWindow window = EditorWindow.GetWindow<UIWindow>();
                window.title = "OnLooker UI";
            }

            void OnGUI()
            {
                GUILayout.Label("UI Properties", EditorStyles.boldLabel);

            }
        }
    }
}