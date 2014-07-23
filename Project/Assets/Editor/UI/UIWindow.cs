using UnityEngine;
using UnityEditor;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {

        public enum UIToggleType
        {
            TEXT,
            TEXTURE,
            LABEL,
            IMAGE,
            BUTTON,
            TEXTFIELD,

        }
        public class UIWindow : EditorWindow
        {
            [SerializeField]
            private UIToggleType m_ToggleType;
            [SerializeField]
            private Transform m_Root;
            [SerializeField]
            private UIManager m_Manager;

            [SerializeField]
            private UIArguments m_Args = new UIArguments();

            [MenuItem("OnLooker/UI")]
            public static void ShowWindow()
            {
                UIWindow window = EditorWindow.GetWindow<UIWindow>();
                window.title = "OnLooker UI";
            }

            private void Update()
            {
                if (m_Root == null)
                {
                    GameObject root = GameObject.Find("UI");
                    if (root != null)
                    {
                        m_Root = root.GetComponent<Transform>();
                    }
                    else
                    {
                        root = new GameObject("UI");
                        m_Manager = root.AddComponent<UIManager>();
                    }

                }
            }

            void OnGUI()
            {
                EditorGUILayout.BeginVertical();
                GUI.enabled = false;
                OLEditorUtilities.transformField("Root", m_Root);
                EditorGUILayout.ObjectField("Manager", m_Manager, typeof(UIManager), true);
                GUI.enabled = true;

                if (m_Root != null)
                {
                    if (m_Manager == null)
                    {
                        m_Manager = m_Root.GetComponent<UIManager>();
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("UI Properties", EditorStyles.boldLabel);
                    m_ToggleType = (UIToggleType)EditorGUILayout.EnumPopup("Toggle Type", m_ToggleType);
                    EditorGUILayout.EndHorizontal();


                    switch (m_ToggleType)
                    {
                        case UIToggleType.TEXT:
                            drawUIText();
                            break;

                        case UIToggleType.TEXTURE:
                            drawUITexture();
                            break;

                        case UIToggleType.LABEL:
                            drawUILabel();
                            break;

                        case UIToggleType.IMAGE:
                            drawUIImage();
                            break;
                        case UIToggleType.BUTTON:
                            drawUIButton();
                            break;
                    }

                }


                
           


                EditorGUILayout.EndVertical();

            }

            void drawUIText()
            {
                m_Args.toggleName = EditorGUILayout.TextField("Toggle Name", m_Args.toggleName);
                m_Args.position = EditorGUILayout.Vector3Field("Position", m_Args.position);
                m_Args.rotation = EditorGUILayout.Vector3Field("Rotation", m_Args.rotation);
                m_Args.anchorMode = (UIAnchor)EditorGUILayout.EnumPopup("Anchor Mode", m_Args.anchorMode);
                m_Args.smoothTransform = EditorGUILayout.Toggle("Smooth Transform", m_Args.smoothTransform);
                m_Args.interactive = EditorGUILayout.Toggle("Interactive", m_Args.interactive);
                m_Args.trapDoubleClick = EditorGUILayout.Toggle("Trap Double Click", m_Args.trapDoubleClick);
                m_Args.text = EditorGUILayout.TextField("Text", m_Args.text);
                m_Args.fontSize = EditorGUILayout.IntField("Font Size", m_Args.fontSize);
                if (GUILayout.Button("Create"))
                {
                    m_Manager.createUIText(m_Args);
                }

                
            }
            void drawUITexture()
            {
                m_Args.toggleName = EditorGUILayout.TextField("Toggle Name", m_Args.toggleName);
                m_Args.position = EditorGUILayout.Vector3Field("Position", m_Args.position);
                m_Args.rotation = EditorGUILayout.Vector3Field("Rotation", m_Args.rotation);
                m_Args.anchorMode = (UIAnchor)EditorGUILayout.EnumPopup("Anchor Mode", m_Args.anchorMode);
                m_Args.smoothTransform = EditorGUILayout.Toggle("Smooth Transform", m_Args.smoothTransform);
                m_Args.interactive = EditorGUILayout.Toggle("Interactive", m_Args.interactive);
                m_Args.trapDoubleClick = EditorGUILayout.Toggle("Trap Double Click", m_Args.trapDoubleClick);
                m_Args.texture = OLEditorUtilities.textureField("Texture",m_Args.texture);

                if (GUILayout.Button("Create"))
                {
                    m_Manager.createUITexture(m_Args);
                }
            }

            void drawUILabel()
            {
                m_Args.toggleName = EditorGUILayout.TextField("Control Name", m_Args.toggleName);
                if (GUILayout.Button("Create"))
                {
                    UIText text;
                    UITexture texture;
                    m_Manager.createUILabel(m_Args,out text,out texture);
                }
            }

            void drawUIImage()
            {
                m_Args.toggleName = EditorGUILayout.TextField("Control Name", m_Args.toggleName);
                if (GUILayout.Button("Create"))
                {
                    UITexture texture;
                    m_Manager.createUIImage(m_Args, out texture);
                }
            }
            void drawUIButton()
            {
                m_Args.toggleName = EditorGUILayout.TextField("Control Name", m_Args.toggleName);
                if (GUILayout.Button("Create"))
                {
                    UIText text;
                    UITexture texture;
                    m_Manager.createUIButton(m_Args,out text, out texture);
                }
            }
        }
    }
}