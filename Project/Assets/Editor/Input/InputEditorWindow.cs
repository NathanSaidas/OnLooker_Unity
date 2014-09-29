using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class InputEditorWindow : EditorWindow
{
    [MenuItem("Tools/InputManager")]
    static void Init()
    {
        GetWindow<InputEditorWindow>("Input Manager");
    }

    private enum State
    {
        INIT,
        TRANSFER_GAME_OBJECT_SETUP,
        SETUP,
    }

    private GameObject m_Root = null;
    private InputManager m_InputManager = null;
    private State m_State = State.INIT;

    private bool m_StartPressed = false;
    private bool m_YesPressed = false;
    private bool m_NoPressed = false;

    private Vector2 m_ScrollPosition = Vector2.zero;
    private string m_AxisName = string.Empty;

    List<int> m_DeleteList = new List<int>();

    private void OnInspectorUpdate()
    {
        


        if(m_State == State.INIT)
        {
            if(m_StartPressed && m_Root != null)
            {
                m_StartPressed = false;
                m_InputManager = m_Root.GetComponent<InputManager>();
                if (m_InputManager == null)
                {
                    //If there is a input manager. Ask the user if they want to use it instead.
                    m_InputManager = InputManager.instance;
                    if (m_InputManager != null)
                    {
                        m_State = State.TRANSFER_GAME_OBJECT_SETUP;
                    }//Otherwise go straight into setup
                    else
                    {
                        m_InputManager = m_Root.AddComponent<InputManager>();
                        m_State = State.SETUP;
                    }
                }
                else
                {
                    m_State = State.SETUP;
                }
            }
        }
        else if(m_State == State.TRANSFER_GAME_OBJECT_SETUP)
        {
            if(m_Root == null)
            {
                m_State = State.INIT;
                return;
            }
            if(m_YesPressed == true && m_Root != null)
            {
                m_YesPressed = false;
                m_NoPressed = false;
                if (m_InputManager != null)
                {
                    Component[] components = m_Root.GetComponents<Component>();
                    if (components.Length <= 1)
                    {
                        DestroyImmediate(m_Root);
                    }
                    m_Root = m_InputManager.gameObject;
                    m_State = State.SETUP;
                }
                else
                {
                    m_State = State.INIT;
                }
            }
            else if(m_NoPressed == true && m_Root != null)
            {
                m_YesPressed = false;
                m_NoPressed = false;
                if (m_InputManager != null)
                {
                    DestroyImmediate(m_InputManager);
                    m_Root.AddComponent<InputManager>();
                    m_State = State.SETUP;
                }
                else
                {
                    m_Root.AddComponent<InputManager>();
                    m_State = State.SETUP;
                }
            }
        }
        else if(m_State == State.SETUP)
        {
            if (m_InputManager == null || m_Root == null)
            {
                m_State = State.INIT;
                return;
            }
        }

    }


    private void OnGUI()
    {
        
        if (m_State == State.INIT)
        {
            EditorGUILayout.LabelField("Drag and drop a gameobject you want to be the root down below and then press the button to get started.");
            m_Root = EditorUtilities.gameObjectField("Root: ", m_Root);
            if(m_Root == null)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Start") && m_Root != null)
            {
                m_StartPressed = true;
            }
            GUI.enabled = true;
        }
        else if(m_State == State.TRANSFER_GAME_OBJECT_SETUP)
        {
            EditorGUILayout.LabelField("It appears there is already a InputManager in the scene. Would you like to use that one instead?");
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Yes"))
            {
                m_YesPressed = true;
            }
            else if(GUILayout.Button("No"))
            {
                m_NoPressed = true;
            }
            EditorGUILayout.EndHorizontal();
        }
        else if(m_State == State.SETUP)
        {
            drawSetupWindow();
        }

    }

    void drawSetupWindow()
    {
        if(m_InputManager == null || m_Root == null)
        {
            return;
        }
        EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Clear", GUILayout.Width(100.0f)))
        {
            m_InputManager.clear();
        }
        
        if (GUILayout.Button("Save", GUILayout.Width(100.0f)))
        {
            m_InputManager.saveEditor();
        }
        
        if (GUILayout.Button("Hide All", GUILayout.Width(100.0f)))
        {
            m_InputManager.hideAll();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Default", GUILayout.Width(100.0f)))
        {
            m_InputManager.setDefault();
        }
        if (GUILayout.Button("Load", GUILayout.Width(100.0f)))
        {
            m_InputManager.loadEditor();
        }
        if (GUILayout.Button("Show All", GUILayout.Width(100.0f)))
        {
            m_InputManager.showAll();
        }
        EditorGUILayout.EndHorizontal();


        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
        List<InputAxis> axisList = m_InputManager.axisList;
        

        if(axisList != null && axisList.Count > 0)
        {
            for(int i = 0; i < axisList.Count; i++)
            {
                if(drawAxis(axisList[i], i))
                {
                    m_DeleteList.Add(i);
                }
            }
        }

        for (int i = m_DeleteList.Count - 1; i>= 0 ; i--)
        {
            int index = m_DeleteList[i];
            if (index >= 0 && index < axisList.Count)
            {
                axisList.RemoveAt(index);
            }
            m_DeleteList.RemoveAt(i);
        }

        m_DeleteList.Clear();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Create"))
        {
            m_InputManager.createInputAxis(m_AxisName);
            m_AxisName = string.Empty;
        }
        m_AxisName = EditorGUILayout.TextField(m_AxisName);

        EditorGUILayout.EndHorizontal();


        
    }
	

    private bool drawAxis(InputAxis aAxis, int aIndex)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField((aIndex + 1) + ". " + aAxis.name,EditorStyles.boldLabel,  GUILayout.Width(190.0f));
        if(aAxis.foldOut == true && GUILayout.Button("Hide", GUILayout.Width(100.0f)))
        {
            aAxis.foldOut = false;
        }
        else if(aAxis.foldOut == false && GUILayout.Button("Show", GUILayout.Width(100.0f)))
        {
            aAxis.foldOut = true;
        }
        if(GUILayout.Button("Delete", GUILayout.Width(100.0f)))
        {
            return true;
        }
        EditorGUILayout.EndHorizontal();


        if(aAxis.foldOut == true)
        {
            float width = 400.0f;
            //Draw it...
            aAxis.name = EditorGUILayout.TextField("Name:", aAxis.name, GUILayout.Width(width));
            aAxis.speed = EditorGUILayout.FloatField("Speed:", aAxis.speed, GUILayout.Width(width));
            aAxis.deviceType = (InputDevice)EditorGUILayout.EnumPopup("Device Type:", aAxis.deviceType, GUILayout.Width(width));
            aAxis.player = (InputPlayer)EditorGUILayout.EnumPopup("Player:", aAxis.player, GUILayout.Width(width));
            aAxis.resetOnRelease = EditorGUILayout.Toggle("Reset On Release:", aAxis.resetOnRelease, GUILayout.Width(width));
            EditorGUILayout.LabelField("Positive Key:", EditorStyles.boldLabel, GUILayout.Width(width));
            KeyCode modifier = KeyCode.None;
            string inputName = string.Empty;

            aAxis.getPositiveKey(out inputName,out modifier);
            inputName = EditorGUILayout.TextField("Input Name:", inputName, GUILayout.Width(width));
            InputModifierKey mod = (InputModifierKey)EditorGUILayout.EnumPopup("Modifier:", InputUtilities.parseModifierKey(modifier), GUILayout.Width(width));
            aAxis.setPositiveKey(inputName, InputUtilities.parseModifierKey(mod));

            EditorGUILayout.LabelField("Negative Key:", EditorStyles.boldLabel, GUILayout.Width(width));
            aAxis.getNegativeKey(out inputName, out modifier);
            inputName = EditorGUILayout.TextField("Input Name:", inputName, GUILayout.Width(width));
            mod = (InputModifierKey)EditorGUILayout.EnumPopup("Modifier:", InputUtilities.parseModifierKey(modifier), GUILayout.Width(width));
            aAxis.setNegativeKey(inputName, InputUtilities.parseModifierKey(mod)); 
        }

        return false;
    }
}
