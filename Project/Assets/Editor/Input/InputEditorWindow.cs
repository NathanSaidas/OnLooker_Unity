using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

#region CHANGE LOG
/* October,24,2014 - Nathan Hanlan, Added constants for a strings.
 * 
 */
#endregion 
namespace Gem
{
    public class InputEditorWindow : EditorWindow
    {
        
        #region CONSTANTS
        /// All editor constants.
        /// 
        public const string PERIOD = ".";
        public const string TOOLS_INPUT_MANAGER = "Tools/Input Manager";
        public const string INPUT_MANAGER = "Input Manager";
        public const string EDITOR_INSTRUCTIONS_A = "Drag and drop a gameobject you want to be the root down below and then press the button to get started.";
        public const string ROOT = "Root: ";
        public const string EDITOR_INSTRUCTIONS_B = "It appears there is already a InputManager in the scene. Would you like to use that one instead?";
        public const string YES = "yes";
        public const string NO = "no";
        public const string SETUP = "Setup";
        public const string SAVE = "Save";
        public const string HIDE_ALL = "Hide all";
        public const string DEFAULT = "Default";
        public const string LOAD = "Load";
        public const string SHOW_ALL = "Show All";
        public const string NAME = "Name: ";
        public const string SPEED = "Speed: ";
        public const string DEVICE = "Device Type: ";
        public const string PLAYER = "Player: ";
        public const string RESET_ON_RELEASE = "Reset On release: ";
        public const string POSITIVE_KEY = "Positive Key: ";
        public const string INPUT_NAME = "Input Name: ";
        public const string MODIFIER = "Modifier: ";
        public const string NEGATIVE_KEY = "Negative Key: ";
        public const string START = "Start";
        public const string CLEAR = "Clear";
        public const string TOOLS_CUTSCENE_WINDOW = "Tools/CutScene Window";
        public const string CUTSCENE_EDITOR = "Cutscene Editor";
        public const string CUTSCENE_IS_NULL = "Cutscene being editted is a null reference";
        public const string MISSING_INSTRUCTIONS = "Missing Instructions";
        public const string MOVE_HANDLE = "Move Handle";
        public const string INSTRUCTION_CNTRL_PT_TO_EDIT_DEFAULTED = "Switch Defaulted";
        public const string BUILD = "Build";
        public const string EDIT_MOVEMENT = "Edit Movement";
        public const string EDIT_LOOK_AT = "Edit Look At";
        public const string DRAW_MENU_INSTRUCTIONS_1 = "Drag and drop the game object you want to create cutscenes under. If the field is";
        public const string DRAW_MENU_INSTRUCTIONS_2 = "left empty a default game object will be created when you press the start button.";
        public const string CUTSCENE_ROOT = "Cutscene Root";
        public const string HIDE = "Hide";
        public const string REMOVE = "Remove";
        public const string CUTSCENES = "Cutscenes";
        public const string CUTSCENE_INSTRUCTION = "Instruction: ";
        public const string INSTRUCTION_NAME = "Instruction Name";
        public const string SHOW = "Show";
        public const string STRAIGHT = "Straight";
        public const string START_POSITION = "Start Position";
        public const string END_POSITION = "End Position";
        public const string BEZIER = "Bezier";
        public const string CONTROL_POINT_A = "Control Point A";
        public const string CONTROL_POINT_B = "Control Point B";
        public const string SEGMENTS = "Segments";
        public const string LOOK_AT_POINTS = "Look at Points";
        public const string ADD = "Add";
        public const string POSITION = "Position";
        public const string START_FRAME = "Start Frame";
        public const string END_FRAME = "End Frame";
        public const string EDIT_MENU = "Edit Menu";
        public const string CUTSCENE_NAME = "Cutscene Name";
        public const string CREATE = "Create";
        public const string DELETE = "Delete";
        public const string EDIT = "Edit";
        public const string SCENE_NAME = "Scene Name";
        public const string RENAME = "Rename";
        public const string TYPE = "Type";
        public const string GET_PATH = "Get Path";
        public const string DISTANCE_CLAMP = "Distance Clamp";
        public const string TIME_DELAY = "Time Delay";
        public const string MOVE_SPEED = "Move Speed";
        public const string MOVE_MODE = "Move Mode";
        public const string LOOK_SPEED = "Look Speed";
        public const string BUILD_MENU = "Build Menu";
        public const string EDIT_MOVEMENT_MENU = "Edit Movement Menu";
        #endregion

        [MenuItem(TOOLS_INPUT_MANAGER)]
        static void Init()
        {
            GetWindow<InputEditorWindow>(INPUT_MANAGER);
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
        private bool m_Repaint = false;
        private Vector2 m_ScrollPosition = Vector2.zero;
        private string m_AxisName = string.Empty;

        List<int> m_DeleteList = new List<int>();

        private void OnInspectorUpdate()
        {
            if(m_Repaint == true)
            {
                Repaint();
                m_Repaint = false;
            }


            if (m_State == State.INIT)
            {
                if (m_StartPressed && m_Root != null)
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
            else if (m_State == State.TRANSFER_GAME_OBJECT_SETUP)
            {
                if (m_Root == null)
                {
                    m_State = State.INIT;
                    return;
                }
                if (m_YesPressed == true && m_Root != null)
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
                else if (m_NoPressed == true && m_Root != null)
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
            else if (m_State == State.SETUP)
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
                EditorGUILayout.LabelField(EDITOR_INSTRUCTIONS_A);
                m_Root = EditorUtilities.gameObjectField(ROOT, m_Root);
                if (m_Root == null)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(START) && m_Root != null)
                {
                    m_StartPressed = true;
                    m_Repaint = true;
                }
                GUI.enabled = true;
            }
            else if (m_State == State.TRANSFER_GAME_OBJECT_SETUP)
            {
                EditorGUILayout.LabelField(EDITOR_INSTRUCTIONS_B);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(YES))
                {
                    m_YesPressed = true;
                }
                else if (GUILayout.Button(NO))
                {
                    m_NoPressed = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            else if (m_State == State.SETUP)
            {
                drawSetupWindow();
            }

            if(GUI.changed && m_InputManager != null)
            {
                EditorUtility.SetDirty(m_InputManager);
            }
        }

        void drawSetupWindow()
        {
            if (m_InputManager == null || m_Root == null)
            {
                return;
            }
            EditorGUILayout.LabelField(SETUP, EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(CLEAR, GUILayout.Width(100.0f)))
            {
                m_InputManager.clear();
            }

            if (GUILayout.Button(SAVE, GUILayout.Width(100.0f)))
            {
                m_InputManager.saveEditor();
            }

            if (GUILayout.Button(HIDE_ALL, GUILayout.Width(100.0f)))
            {
                m_InputManager.hideAll();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(DEFAULT, GUILayout.Width(100.0f)))
            {
                m_InputManager.setVerdantStoryDefault();
            }
            if (GUILayout.Button(LOAD, GUILayout.Width(100.0f)))
            {
                m_InputManager.loadEditor();
            }
            if (GUILayout.Button(SHOW_ALL, GUILayout.Width(100.0f)))
            {
                m_InputManager.showAll();
            }
            EditorGUILayout.EndHorizontal();

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            List<InputAxis> axisList = m_InputManager.axisList;

            if (axisList != null && axisList.Count > 0)
            {
                for (int i = 0; i < axisList.Count; i++)
                {
                    if (drawAxis(axisList[i], i))
                    {
                        m_DeleteList.Add(i);
                    }
                }
            }

            for (int i = m_DeleteList.Count - 1; i >= 0; i--)
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
            if (GUILayout.Button(CREATE))
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
            EditorGUILayout.LabelField((aIndex + 1) + PERIOD + aAxis.name, EditorStyles.boldLabel, GUILayout.Width(190.0f));
            if (aAxis.foldOut == true && GUILayout.Button(HIDE, GUILayout.Width(100.0f)))
            {
                aAxis.foldOut = false;
            }
            else if (aAxis.foldOut == false && GUILayout.Button(SHOW, GUILayout.Width(100.0f)))
            {
                aAxis.foldOut = true;
            }
            if (GUILayout.Button(DELETE, GUILayout.Width(100.0f)))
            {
                return true;
            }
            EditorGUILayout.EndHorizontal();


            if (aAxis.foldOut == true)
            {
                float width = 400.0f;
                //Draw it...
                aAxis.name = EditorGUILayout.TextField(NAME, aAxis.name, GUILayout.Width(width));
                aAxis.speed = EditorGUILayout.FloatField(SPEED, aAxis.speed, GUILayout.Width(width));
                aAxis.deviceType = (InputDevice)EditorGUILayout.EnumPopup(DEVICE, aAxis.deviceType, GUILayout.Width(width));
                aAxis.player = (InputPlayer)EditorGUILayout.EnumPopup(PLAYER, aAxis.player, GUILayout.Width(width));
                aAxis.resetOnRelease = EditorGUILayout.Toggle(RESET_ON_RELEASE, aAxis.resetOnRelease, GUILayout.Width(width));
                EditorGUILayout.LabelField(POSITIVE_KEY, EditorStyles.boldLabel, GUILayout.Width(width));
                KeyCode modifier = KeyCode.None;
                string inputName = string.Empty;

                aAxis.getPositiveKey(out inputName, out modifier);
                inputName = EditorGUILayout.TextField(INPUT_NAME, inputName, GUILayout.Width(width));
                InputModifierKey mod = (InputModifierKey)EditorGUILayout.EnumPopup(MODIFIER, InputUtilities.parseModifierKey(modifier), GUILayout.Width(width));
                aAxis.setPositiveKey(inputName, InputUtilities.parseModifierKey(mod));

                EditorGUILayout.LabelField(NEGATIVE_KEY, EditorStyles.boldLabel, GUILayout.Width(width));
                aAxis.getNegativeKey(out inputName, out modifier);
                inputName = EditorGUILayout.TextField(INPUT_NAME, inputName, GUILayout.Width(width));
                mod = (InputModifierKey)EditorGUILayout.EnumPopup(MODIFIER, InputUtilities.parseModifierKey(modifier), GUILayout.Width(width));
                aAxis.setNegativeKey(inputName, InputUtilities.parseModifierKey(mod));
            }
            return false;
        }
    }
}