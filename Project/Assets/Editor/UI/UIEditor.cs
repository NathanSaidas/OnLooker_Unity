using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


#region CHANGE LOG
/* November,13,2014 - Nathan Hanlan, Added support for UILabel.
 * November,13,2014 - Nathan Hanlan, Removed bug where the state would change between GUI Update Call Frames
 * November,14,2014 - Nathan Hanlan, Added additional error handling in CopyEditMenu method to better handle setting parameters of multiple components
 */
#endregion 
namespace Gem
{
    

    [SerializeField]
    public class UIEditor : EditorWindow
    {
        #region INIT
        [MenuItem("Tools/UI Editor")]
        private static void Init()
        {
            UIEditor window = GetWindow<UIEditor>("UI Editor"); 
 
        }

        private void OnFocus()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }
        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        }
        #endregion

        private enum State
        {
            /// <summary>
            /// Requires a UI manager in the scene
            /// </summary>
            INITIALIZATION,
            /// <summary>
            /// Displays all the Toggles in the scene
            /// </summary>
            TOGGLE_SELECT,
            /// <summary>
            /// Displays the edit menu
            /// </summary>
            TOGGLE_EDIT,
            /// <summary>
            /// Displays the create menu
            /// </summary>
            TOGGLE_CREATE,
            /// <summary>
            /// Updates the root game object then goes back to the selection state.
            /// </summary>
            UPDATE_ROOT,

        }
        #region CONSTANTS
       

        private const string GAME_OBJECT_2D = "UI_2D";
        private const string GAME_OBJECT_3D = "UI_3D";
        private const string GAME_OBJECT_WORLD = "UI_World";


        private const string GAME_OBJECT_ROOT = "Root";
        private const string INIT_TIP = "Drag and drop the root game object for UI here.";


        private const string CREATE = "Create";
        private const string NAME = "Name";
        private const string ID = "ID";
        private const string EDIT = "Edit";
        private const string DELETE = "Delete";
        private const string SAVE = "Save";
        private const string BACK = "Back";
        private const string SEARCH = "Search:";
        private const string COPY = "Copy";
        private const string SELECTABLE = "Selectable";
        private const string RECIEVE_ACTIONS = "Recieve Actions";
        private const string UI_SPACE = "UI Space";
        private const string UI_TYPE = "UI Type";
        private const string TOGGLE_WITH_NAME_EXISTS = "Toggle with that name already exists.";
        public const string WIDTH = "Width";
        public const string HEIGHT = "Height";
        public const string MESH_BOARDER = "Mesh Boarder";
        public const string OUTER_UV_BOARDER = "Outer UV Boarder";
        public const string INNER_UV_BOARDER = "Inner UV Boarder";
        public const string DEFAULT_MATERIAL = "Default Material";
        public const string TEXTURE = "Texture";
        public const string SHADER = "Shader";
        public const string COLOR = "Color";
        public const string TEXT = "Text";
        public const string FONT = "Font";
        public const string FONT_SIZE = "Font Size";
        public const string DISABLED = "Disabled";
        public const string NORMAL = "Normal";
        public const string HOVER = "Hover";
        public const string DOWN = "Down";
        public const string ENABLED_TEXT_COLOR = "Enabled Text Color";
        public const string DISABLED_TEXT_COLOR = "Disabled Text Color";
        public const string UI_EVENT_LISTENER = "Event Listener";
        public const string BUTTON_STATE = "Button State";

        public const string LABEL = "Label";
        public const string IMAGE = "Image";
        public const string BUTTON = "Button";

        private const string TOGGLE_SELECTION = "Toggle Selection";
        private const string TOGGLE_EDITOR = "Toggle Editor";
        private const string TOGGLE_CREATOR = "Toggle Creator";


        #region DEBUG ONLY
        private const string MISSING_BUTTON = "Missing UIButton in child game object";
        private const string MISSING_LABEL = "Missing UILabel in child game object";
        private const string MISSING_IMAGE = "Missing UIImage in child game object";
        #endregion
        #endregion

        /// <summary>
        /// The current state of the menu
        /// </summary>
        private State m_State = State.INITIALIZATION;
        /// <summary>
        /// The next state to transition to for the menu
        /// </summary>
        private State m_NextState = State.INITIALIZATION;
        /// <summary>
        /// The root gameobject in the scene for the UI which will be split into 2D,3D, and world space sub objects.
        /// </summary>
        private GameObject m_UIRoot = null;
        private GameObject m_NewRoot = null;

        private GameObject m_2DUI = null;
        private GameObject m_3DUI = null;
        private GameObject m_WorldUI = null;
        private bool m_Repaint = false;

        private Vector2 m_SelectScrollPosition = Vector2.zero;
        private Vector2 m_EditScrollPosition = Vector2.zero;
        private Vector2 m_CreateScrollPosition = Vector2.zero;


        private string m_ToggleSearchField = string.Empty;
        private List<UIToggle> m_Toggles = new List<UIToggle>();
        private List<UIToggle> m_TogglesToDisplay = new List<UIToggle>();
        private List<UIToggle> m_TogglesToRemove = new List<UIToggle>();
        private UIToggle m_ToggleToEdit = null;

        private UIToggleParams m_ToggleParams = new UIToggleParams();
        private UIType m_NextUIType = UIType.IMAGE;
        

        #region GUI CONTENTs
        private GUIContent m_InitRootGameObject = new GUIContent("Root GameObject:", "The game object you want to be the root for all UI's");

        #endregion
        #region GUI LAYOUT OPTIONS
        private GUILayoutOption m_NameColumnWidth = GUILayout.Width(80.0f);
        private GUILayoutOption m_IDColumnWidth = GUILayout.Width(45.0f);
        private GUILayoutOption m_EditButtonWidth = GUILayout.Width(80.0f);
        private GUILayoutOption m_DeleteButtonWidth = GUILayout.Width(80.0f);
        #endregion
        private void OnSceneGUI(SceneView aScene)
        {
            
        }

        private void Update()
        {

            if(m_Repaint == true)
            {
                this.Repaint();
            }
            bool repaint = m_NextState != m_State;
            if(m_NextState == State.UPDATE_ROOT)
            {
                m_UIRoot = m_NewRoot;
                UpdateRoot();
            }
            m_State = m_NextState;
            m_Repaint = repaint;
            if (m_ToggleParams != null)
            {
                m_ToggleParams.uiType = m_NextUIType;
            }
        }
        private void OnGUI()
        {
            switch(m_State)
            {
                case State.INITIALIZATION:
                    InitializationGUI();
                    break;
                case State.TOGGLE_CREATE:
                    ToggleCreateGUI();
                    break;
                case State.TOGGLE_EDIT:
                    ToggleEditGUI();
                    break;
                case State.TOGGLE_SELECT:
                    ToggleSelectGUI();
                    break;
            }
        }
        private void InitializationGUI()
        {
            m_UIRoot = EditorUtilities.ObjectField<GameObject>(m_InitRootGameObject, m_UIRoot);
            if(m_UIRoot == null)
            {
                GUI.enabled = false;
            }
            if(GUILayout.Button(CREATE))
            {
                UpdateRoot();
            }
            GUI.enabled = true;
            
        }


        private void ToggleCreateGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TOGGLE_EDITOR, EditorStyles.boldLabel);
            GameObject gameObject = EditorUtilities.ObjectField<GameObject>(m_InitRootGameObject, m_UIRoot);
            if (gameObject != m_UIRoot)
            {
                m_NewRoot = gameObject;
                m_NextState = State.UPDATE_ROOT;
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button(BACK))
            {
                if (m_NextState == State.TOGGLE_CREATE)
                {
                    m_NextState = State.TOGGLE_SELECT;
                }
            }
            m_CreateScrollPosition = EditorGUILayout.BeginScrollView(m_CreateScrollPosition);

            if(m_ToggleParams != null)
            {
                m_ToggleParams.name = EditorGUILayout.TextField(NAME, m_ToggleParams.name);
                m_ToggleParams.id = EditorGUILayout.IntField(ID, m_ToggleParams.id);
                m_ToggleParams.isSelectable = EditorGUILayout.Toggle(SELECTABLE, m_ToggleParams.isSelectable);
                m_ToggleParams.recieveActions = EditorGUILayout.Toggle(RECIEVE_ACTIONS, m_ToggleParams.recieveActions);
                m_ToggleParams.uiSpace = (UISpace)EditorGUILayout.EnumPopup(UI_SPACE, m_ToggleParams.uiSpace);
                m_NextUIType = (UIType)EditorGUILayout.EnumPopup(UI_TYPE, m_ToggleParams.uiType);
                switch(m_ToggleParams.uiType)
                {
                    case UIType.IMAGE:
                        DrawUIImage();
                        break;
                    case UIType.LABEL:
                        DrawUILabel();
                        break;
                    case UIType.BUTTON:
                        DrawUIButton();
                        break;
                }
            }
            EditorGUILayout.EndScrollView();
            if(GUILayout.Button(CREATE) && m_ToggleParams.name != string.Empty)
            {
                if (GetToggle(m_ToggleParams.name) == null)
                {
                    CreateUIToggle();
                }
                else
                {
                    DebugUtils.LogError(TOGGLE_WITH_NAME_EXISTS);
                }
            }

        }
        private void ToggleEditGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TOGGLE_EDITOR, EditorStyles.boldLabel);
            GameObject gameObject = EditorUtilities.ObjectField<GameObject>(m_InitRootGameObject, m_UIRoot);
            if (gameObject != m_UIRoot)
            {
                m_NewRoot = gameObject;
                m_NextState = State.UPDATE_ROOT;
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button(BACK))
            {
                if (m_NextState == State.TOGGLE_EDIT)
                {
                    m_NextState = State.TOGGLE_SELECT;
                }
            }
            m_EditScrollPosition = EditorGUILayout.BeginScrollView(m_EditScrollPosition);

            if (m_ToggleParams != null && m_ToggleToEdit != null)
            {
                m_ToggleParams.name = EditorGUILayout.TextField(NAME, m_ToggleParams.name);
                m_ToggleParams.id = EditorGUILayout.IntField(ID, m_ToggleParams.id);
                m_ToggleParams.isSelectable = EditorGUILayout.Toggle(SELECTABLE, m_ToggleParams.isSelectable);
                m_ToggleParams.recieveActions = EditorGUILayout.Toggle(RECIEVE_ACTIONS, m_ToggleParams.recieveActions);
                m_ToggleParams.uiSpace = (UISpace)EditorGUILayout.EnumPopup(UI_SPACE, m_ToggleParams.uiSpace);
                m_NextUIType = (UIType)EditorGUILayout.EnumPopup(UI_TYPE, m_ToggleParams.uiType);
                switch(m_ToggleParams.uiType)
                {
                    case UIType.IMAGE:
                        DrawUIImage();
                        break;
                    case UIType.LABEL:
                        DrawUILabel();
                        break;
                    case UIType.BUTTON:
                        DrawUIButton();
                        break;
                }
            }
            EditorGUILayout.EndScrollView();
            if(GUILayout.Button(SAVE))
            {
                if(m_ToggleToEdit != null)
                {
                    UIToggle toggleWithName = GetToggle(m_ToggleParams.name);
                    if(toggleWithName != null && toggleWithName != m_ToggleToEdit)
                    {
                        DebugUtils.LogError(TOGGLE_WITH_NAME_EXISTS);
                    }
                    else
                    {
                        UIType previousType = m_ToggleToEdit.uiType;
                        m_ToggleToEdit.gameObject.name = m_ToggleParams.name;
                        m_ToggleToEdit.id = m_ToggleParams.id;
                        m_ToggleToEdit.selectable = m_ToggleParams.isSelectable;
                        m_ToggleToEdit.receivesActionEvents = m_ToggleParams.recieveActions;
                        m_ToggleToEdit.uiSpace = m_ToggleParams.uiSpace;
                        m_ToggleToEdit.uiType = m_ToggleParams.uiType;
                        switch(m_ToggleToEdit.uiSpace)
                        {
                            case UISpace.TWO_DIMENSIONAL:
                                if(m_2DUI != null)
                                {
                                    m_ToggleToEdit.transform.parent = m_2DUI.transform;
                                }
                                break;
                            case UISpace.THREE_DIMENSIONAL:
                                if (m_3DUI != null)
                                {
                                    m_ToggleToEdit.transform.parent = m_3DUI.transform;
                                }
                                break;
                            case UISpace.WORLD:
                                if (m_WorldUI != null)
                                {
                                    m_ToggleToEdit.transform.parent = m_WorldUI.transform;
                                }
                                break;
                        }
                        UpdateToggleByType(previousType);
                        EditorUtility.SetDirty(m_ToggleToEdit);
                    }
                }
            }

        }
        private void ToggleSelectGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TOGGLE_CREATOR, EditorStyles.boldLabel);
            GameObject gameObject = EditorUtilities.ObjectField<GameObject>(m_InitRootGameObject, m_UIRoot);
            if (gameObject != m_UIRoot)
            {
                m_NewRoot = gameObject;
                m_NextState = State.UPDATE_ROOT;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(NAME, EditorStyles.boldLabel,m_NameColumnWidth);
            EditorGUILayout.LabelField(ID,m_IDColumnWidth);
            m_ToggleSearchField = EditorGUILayout.TextField(SEARCH, m_ToggleSearchField);
            EditorGUILayout.EndHorizontal();


            m_SelectScrollPosition = EditorGUILayout.BeginScrollView(m_SelectScrollPosition);
            m_TogglesToRemove.Clear();
            m_TogglesToDisplay.Clear();


            List<UIToggle>.Enumerator displayIterator = m_Toggles.GetEnumerator();

            if(m_ToggleSearchField != string.Empty)
            {
                List<UIToggle>.Enumerator iter = m_Toggles.GetEnumerator();
                while(iter.MoveNext())
                {
                    if(iter.Current == null)
                    {
                        continue;
                    }
                    if(iter.Current.name.Contains(m_ToggleSearchField))
                    {
                        m_TogglesToDisplay.Add(iter.Current);
                    }
                }
                displayIterator = m_TogglesToDisplay.GetEnumerator();
            }



            while (displayIterator.MoveNext())
            {
                if (displayIterator.Current == null)
                {
                    continue;
                }
                DrawUIToggleSelection(displayIterator.Current);
            }

            if(m_TogglesToRemove.Count > 0)
            {
                CleanList();
            }
            
            EditorGUILayout.EndScrollView();

            if(GUILayout.Button(CREATE))
            {
                if(m_NextState == State.TOGGLE_SELECT)
                {
                    ResetCreateMenu();
                    m_NextState = State.TOGGLE_CREATE;
                }
            }
            
            
        }

        private void DrawUIToggleSelection(UIToggle aToggle)
        {
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(aToggle.name, m_NameColumnWidth);
            EditorGUILayout.LabelField(aToggle.id.ToString(), m_IDColumnWidth);
            if(GUILayout.Button(EDIT,m_EditButtonWidth))
            {
                
                if(m_NextState == State.TOGGLE_SELECT)
                {
                    m_NextState = State.TOGGLE_EDIT;
                    m_ToggleToEdit = aToggle;
                    CopyEditMenu(m_ToggleToEdit);
                }
            }
            if(GUILayout.Button(COPY,m_EditButtonWidth))
            {
                if(m_NextState == State.TOGGLE_SELECT)
                {
                    m_NextState = State.TOGGLE_CREATE;
                    CopyCreateMenu(aToggle);
                }
            }
            if(GUILayout.Button(DELETE,m_DeleteButtonWidth))
            {
                m_TogglesToRemove.Add(aToggle);
            }
            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Clears toggles from the toggles to remove list and destroys them
        /// </summary>
        private void CleanList()
        {
            List<UIToggle>.Enumerator iter = m_TogglesToRemove.GetEnumerator();
            while(iter.MoveNext())
            {
                m_Toggles.Remove(iter.Current);
                if (iter.Current == null)
                {
                    continue;
                }
                DestroyImmediate(iter.Current.gameObject); 
            }
            m_TogglesToRemove.Clear();
        }
        private void UpdateRoot()
        {
            m_2DUI = null;
            m_3DUI = null;
            m_WorldUI = null;
            m_Toggles.Clear();
            m_TogglesToRemove.Clear();
            m_TogglesToDisplay.Clear();

            ///Search for Sub GameObjects
            foreach (Transform transform in m_UIRoot.transform)
            {
                if (transform == null)
                {
                    continue;
                }
                string gameobjectName = transform.gameObject.name;
                if (gameobjectName == GAME_OBJECT_2D)
                {
                    m_2DUI = transform.gameObject;
                    if (m_2DUI != null && m_3DUI != null && m_WorldUI != null)
                    {
                        break;
                    }
                    continue;
                }
                if (gameobjectName == GAME_OBJECT_3D)
                {
                    m_3DUI = transform.gameObject;
                    if (m_2DUI != null && m_3DUI != null && m_WorldUI != null)
                    {
                        break;
                    }
                    continue;
                }
                if (gameobjectName == GAME_OBJECT_WORLD)
                {
                    m_WorldUI = transform.gameObject;
                    if (m_2DUI != null && m_3DUI != null && m_WorldUI != null)
                    {
                        break;
                    }
                    continue;
                }
            }

            if (m_2DUI == null)
            {
                m_2DUI = new GameObject(GAME_OBJECT_2D);
                m_2DUI.transform.parent = m_UIRoot.transform;
                m_2DUI.layer = UIManager.UI_2D_LAYER;
            }
            if (m_3DUI == null)
            {
                m_3DUI = new GameObject(GAME_OBJECT_3D);
                m_3DUI.transform.parent = m_UIRoot.transform;
                m_3DUI.layer = UIManager.UI_3D_LAYER;
            }
            if (m_WorldUI == null)
            {
                m_WorldUI = new GameObject(GAME_OBJECT_WORLD);
                m_WorldUI.transform.parent = m_UIRoot.transform;
                m_WorldUI.layer = UIManager.UI_WORLD_LAYER;
            }

            m_UIRoot.transform.position = Vector3.zero;
            m_UIRoot.transform.rotation = Quaternion.identity;
            m_2DUI.transform.position = Vector3.zero;
            m_2DUI.transform.rotation = Quaternion.identity;
            m_3DUI.transform.position = Vector3.zero;
            m_3DUI.transform.rotation = Quaternion.identity;
            m_WorldUI.transform.position = Vector3.zero;
            m_WorldUI.transform.rotation = Quaternion.identity;


            //Get all components
            m_Toggles.AddRange(m_UIRoot.GetComponentsInChildren<UIToggle>()); 

            m_NextState = State.TOGGLE_SELECT;
        }
        private void ResetCreateMenu()
        {
            if(m_ToggleParams != null)
            {
                m_ToggleParams.Clear();
            }
        }
        /// <summary>
        /// Copies the toggles properties to the parameters for editting.
        /// </summary>
        /// <param name="aCopyToggle"></param>
        private void CopyEditMenu(UIToggle aCopyToggle)
        {
            if(m_ToggleParams != null && aCopyToggle != null)
            {
                if(m_ToggleParams.uiType != aCopyToggle.uiType)
                {
                    CreateParamsForType(aCopyToggle.uiType);
                }
                m_ToggleParams.Clear();
                m_ToggleParams.name = aCopyToggle.gameObject.name;
                m_ToggleParams.id = aCopyToggle.id;
                m_ToggleParams.isSelectable = aCopyToggle.selectable;
                m_ToggleParams.recieveActions = aCopyToggle.receivesActionEvents;
                m_ToggleParams.uiSpace = aCopyToggle.uiSpace;
                m_ToggleParams.uiType = aCopyToggle.uiType;
                switch(m_ToggleParams.uiType)
                {
                    case UIType.IMAGE:
                        {
                            UIImageParams imageParams = m_ToggleParams as UIImageParams;
                            UIImage image = aCopyToggle.GetComponentInChildren<UIImage>();
                            if(imageParams == null || image == null)
                            {
                                DebugUtils.LogError(MISSING_IMAGE);
                                break;
                            }
                            imageParams.color = image.color;
                            imageParams.height = image.height;
                            imageParams.width = image.width;
                            imageParams.meshBoarder = image.meshBoarder;
                            imageParams.innerUVBoarder = image.innerUVBoarder;
                            imageParams.outerUVBoarder = image.innerUVBoarder;
                            imageParams.shader = image.shader;
                            imageParams.texture = image.texture;
                        }
                        break;
                    case UIType.LABEL:
                        {
                            UILabelParams labelParams = m_ToggleParams as UILabelParams;
                            UILabel label = aCopyToggle.GetComponentInChildren<UILabel>();
                            if(labelParams == null || label == null)
                            {
                                DebugUtils.LogError(MISSING_LABEL);
                                break;
                            }
                            labelParams.color = label.color;
                            labelParams.font = label.font;
                            labelParams.fontSize = label.fontSize;
                            labelParams.fontTexture = label.fontTexture;
                            labelParams.text = label.text;
                        }
                        break;
                    case UIType.BUTTON:
                        {
                            UIButtonParams buttonParams = m_ToggleParams as UIButtonParams;
                            UIButton button = aCopyToggle.GetComponentInChildren<UIButton>();
                            if (buttonParams == null || button == null)
                            {
                                DebugUtils.LogError(MISSING_BUTTON);
                                break;
                            }
                            buttonParams.disabled = button.buttonState == UIButtonState.DISABLED;
                            buttonParams.disabledTexture = button.disabledTexture;
                            buttonParams.normalTexture = button.normalTexture;
                            buttonParams.hoverTexture = button.hoverTexture;
                            buttonParams.downTexture = button.downTexture;
                            buttonParams.enabledTextColor = button.enabledTextColor;
                            buttonParams.disabledTextColor = button.disabledTextColor;
                            buttonParams.eventListener = button.eventListener;
                            
                            UILabel label = button.GetComponentInChildren<UILabel>();
                            if(label == null)
                            {
                                DebugUtils.LogError(MISSING_LABEL);
                            }
                            else
                            {
                                label.color = buttonParams.labelColor;
                                label.font = buttonParams.labelFont;
                                label.fontSize = buttonParams.labelFontSize;
                                label.fontTexture = buttonParams.labelFontTexture;
                                label.text = buttonParams.labelText;
                            }
                            UIImage image = button.GetComponentInChildren<UIImage>();
                            if(image == null)
                            {
                                DebugUtils.LogError(MISSING_IMAGE);
                            }
                            else
                            {
                                image.color = buttonParams.imageColor;
                                image.height = buttonParams.imageHeight;
                                image.width = buttonParams.imageWidth;
                                image.meshBoarder = buttonParams.imageMeshBoarder;
                                image.innerUVBoarder = buttonParams.imageInnerUVBoarder;
                                image.outerUVBoarder = buttonParams.imageInnerUVBoarder;
                                image.shader = buttonParams.imageShader;
                                image.texture = buttonParams.imageTexture;
                            }
                        }
                        break;

                }
            }
        }
        private void CopyCreateMenu(UIToggle aCopyToggle)
        {
            if(m_ToggleParams != null && aCopyToggle != null)
            {
                m_ToggleParams.Clear();
                m_ToggleParams.isSelectable = aCopyToggle.selectable;
                m_ToggleParams.recieveActions = aCopyToggle.receivesActionEvents;
                m_ToggleParams.uiSpace = aCopyToggle.uiSpace;
                m_ToggleParams.uiType = aCopyToggle.uiType;
            }
        }
        private void CreateParamsForType(UIType aType)
        {
            UIToggleParams tempParams = m_ToggleParams;
            switch(aType)
            {
                case UIType.IMAGE:
                    m_ToggleParams = new UIImageParams();
                    m_ToggleParams.uiType = UIType.IMAGE;
                    m_ToggleParams.Copy(tempParams);
                    break;
                case UIType.LABEL:
                    m_ToggleParams = new UILabelParams();
                    m_ToggleParams.uiType = UIType.LABEL;
                    m_ToggleParams.Copy(tempParams);
                    break;
                case UIType.BUTTON:
                    m_ToggleParams = new UIButtonParams();
                    m_ToggleParams.uiType = UIType.BUTTON;
                    m_ToggleParams.Copy(tempParams);
                    break;
            }
        }
        private void CreateUIToggle()
        {
            if(m_UIRoot == null || m_WorldUI == null || m_2DUI == null || m_3DUI == null)
            {
                m_NextState = State.INITIALIZATION;
                return;
            }
            Transform parent = null;
            switch(m_ToggleParams.uiSpace)
            {
                case UISpace.TWO_DIMENSIONAL:
                    parent = m_2DUI.transform;
                    break;
                case UISpace.THREE_DIMENSIONAL:
                    parent = m_3DUI.transform;
                    break;
                case UISpace.WORLD:
                    parent = m_WorldUI.transform;
                    break;
            }
            UIToggle toggle = UIUtilities.CreateUIToggle(m_ToggleParams, parent);
            switch(toggle.uiType)
            {
                case UIType.IMAGE:
                    UIUtilities.CreateUIImage(m_ToggleParams as UIImageParams, toggle);
                    break;
                case UIType.LABEL:
                    UIUtilities.CreateUILabel(m_ToggleParams as UILabelParams, toggle);
                    break;
                case UIType.BUTTON:
                    UIUtilities.CreateUIButton(m_ToggleParams as UIButtonParams, toggle);
                    break;
            }
            m_Toggles.Add(toggle);
            EditorUtility.SetDirty(toggle);
        }
        /// <summary>
        /// Gets a toggle by name.
        /// </summary>
        /// <param name="aName"></param>
        /// <returns></returns>
        private UIToggle GetToggle(string aName)
        {
            List<UIToggle>.Enumerator iter = m_Toggles.GetEnumerator();
            UIToggle toggle = null;

            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                if(iter.Current.gameObject.name == aName)
                {
                    toggle = iter.Current;
                    break;
                }
            }
          
            return toggle;
        }
        
        /// <summary>
        /// Updates the toggle based on the previous type
        /// </summary>
        /// <param name="aPreviousType"></param>
        private void UpdateToggleByType(UIType aPreviousType)
        {
            if (aPreviousType != m_ToggleToEdit.uiType)
            {
                //Step 1. Remove the previous gameobjects.
                switch (aPreviousType)
                {
                    case UIType.IMAGE:
                        {
                            UIImage image = m_ToggleToEdit.GetComponentInChildren<UIImage>();
                            if (image != null)
                            {
                                DestroyImmediate(image.gameObject);
                            }
                        }
                        break;
                    case UIType.LABEL:
                        {
                            UILabel label = m_ToggleToEdit.GetComponentInChildren<UILabel>();
                            if(label != null)
                            {
                                DestroyImmediate(label.gameObject);
                            }
                        }
                        break;
                    case UIType.BUTTON:
                        {
                            UIButton button = m_ToggleToEdit.GetComponentInChildren<UIButton>();
                            if(button != null)
                            {
                                DestroyImmediate(button.gameObject);
                            }
                        }
                        break;
                }
                //Step 2. Add the new game objects
                switch (m_ToggleToEdit.uiType)
                {
                    case UIType.IMAGE:
                        {
                            UIImageParams imageParams = m_ToggleParams as UIImageParams;
                            if (imageParams != null)
                            {
                                UIUtilities.CreateUIImage(imageParams, m_ToggleToEdit);
                            }
                        }
                        break;
                    case UIType.LABEL:
                        {
                            UILabelParams labelParams = m_ToggleParams as UILabelParams;
                            if(labelParams != null)
                            {
                                UIUtilities.CreateUILabel(labelParams, m_ToggleToEdit);
                            }
                        }
                        break;
                    case UIType.BUTTON:
                        {
                            UIButtonParams buttonParams = m_ToggleParams as UIButtonParams;
                            if(buttonParams != null)
                            {
                                UIUtilities.CreateUIButton(buttonParams, m_ToggleToEdit);
                            }
                        }
                        break;
                }
            }
            else
            {
                switch(m_ToggleToEdit.uiType)
                {
                    case UIType.IMAGE:
                        {
                            UIImage image = m_ToggleToEdit.GetComponentInChildren<UIImage>();
                            UIImageParams imageParams = m_ToggleParams as UIImageParams;
                            if(image == null || imageParams == null)
                            {
                                DebugUtils.LogError(MISSING_LABEL);
                                break;
                            }
                            image.width = imageParams.width;
                            image.height = imageParams.height;
                            image.meshBoarder = imageParams.meshBoarder;
                            image.outerUVBoarder = imageParams.outerUVBoarder;
                            image.innerUVBoarder = imageParams.innerUVBoarder;
                            image.texture = imageParams.texture;
                            image.shader = imageParams.shader;
                            image.color = imageParams.color;
                            image.material.shader = image.shader;
                            image.material.SetTexture(UIUtilities.SHADER_TEXTURE, image.texture);

                            image.GenerateMesh();
                            image.SetTexture();
                            image.SetColor();

                        }
                        break;
                    case UIType.LABEL:
                        {
                            UILabel label = m_ToggleToEdit.GetComponentInChildren<UILabel>();
                            UILabelParams labelParams = m_ToggleParams as UILabelParams;
                            if(label == null || labelParams == null)
                            {
                                DebugUtils.LogError(MISSING_LABEL);
                                break;
                            }
                            label.text = labelParams.text;
                            label.fontSize = labelParams.fontSize;
                            label.font = labelParams.font;
                            label.color = labelParams.color;
                            label.fontTexture = labelParams.fontTexture;
                            label.UpdateComponents();
                        }
                        break;
                    case UIType.BUTTON:
                        {
                            UIButton button = m_ToggleToEdit.GetComponentInChildren<UIButton>();
                            UIButtonParams buttonParams = m_ToggleParams as UIButtonParams;
                            if(button == null || buttonParams == null)
                            {
                                DebugUtils.LogError(MISSING_BUTTON);
                                break;
                            }
                            buttonParams.disabled = button.buttonState == UIButtonState.DISABLED;
                            buttonParams.disabledTexture = button.disabledTexture;
                            buttonParams.normalTexture = button.normalTexture;
                            buttonParams.hoverTexture = button.hoverTexture;
                            buttonParams.downTexture = button.downTexture;
                            buttonParams.enabledTextColor = button.enabledTextColor;
                            buttonParams.disabledTextColor = button.disabledTextColor;
                            buttonParams.eventListener = button.eventListener;

                            UILabel label = button.GetComponentInChildren<UILabel>();
                            if (label == null)
                            {
                                DebugUtils.LogError(MISSING_LABEL);
                            }
                            else
                            {
                                label.color = buttonParams.labelColor;
                                label.font = buttonParams.labelFont;
                                label.fontSize = buttonParams.labelFontSize;
                                label.fontTexture = buttonParams.labelFontTexture;
                                label.text = buttonParams.labelText;
                            }
                            UIImage image = button.GetComponentInChildren<UIImage>();
                            if (image == null)
                            {
                                DebugUtils.LogError(MISSING_IMAGE);
                            }
                            else
                            {
                                image.color = buttonParams.imageColor;
                                image.height = buttonParams.imageHeight;
                                image.width = buttonParams.imageWidth;
                                image.meshBoarder = buttonParams.imageMeshBoarder;
                                image.innerUVBoarder = buttonParams.imageInnerUVBoarder;
                                image.outerUVBoarder = buttonParams.imageInnerUVBoarder;
                                image.shader = buttonParams.imageShader;
                                image.texture = buttonParams.imageTexture;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Draws the parameters of a UIImage
        /// </summary>
        private void DrawUIImage()
        {
            UIImageParams imageParams = m_ToggleParams as UIImageParams;
            if(imageParams == null)
            {
                CreateParamsForType(m_ToggleParams.uiType);
                imageParams = m_ToggleParams as UIImageParams;
            }
            EditorGUILayout.BeginHorizontal();
            imageParams.width = EditorGUILayout.FloatField(WIDTH, imageParams.width);
            imageParams.height = EditorGUILayout.FloatField(HEIGHT, imageParams.height);
            EditorGUILayout.EndHorizontal();
            imageParams.meshBoarder = EditorUtilities.UIBoarderField(MESH_BOARDER, imageParams.meshBoarder);
            imageParams.outerUVBoarder = EditorUtilities.UIBoarderField(OUTER_UV_BOARDER, imageParams.outerUVBoarder);
            imageParams.innerUVBoarder = EditorUtilities.UIBoarderField(INNER_UV_BOARDER, imageParams.innerUVBoarder);
            imageParams.texture = EditorUtilities.ObjectField<Texture>(TEXTURE, imageParams.texture);
            imageParams.shader = EditorUtilities.ObjectField<Shader>(SHADER, imageParams.shader);
            imageParams.color = EditorGUILayout.ColorField(COLOR, imageParams.color);
        }

        /// <summary>
        /// Draws the parameters of the UILabel
        /// </summary>
        private void DrawUILabel()
        {
            UILabelParams labelParams = m_ToggleParams as UILabelParams;
            if(labelParams == null)
            {
                CreateParamsForType(m_ToggleParams.uiType);
                labelParams = m_ToggleParams as UILabelParams;
            }
            labelParams.text = EditorGUILayout.TextField(TEXT, labelParams.text);
            labelParams.fontSize = EditorGUILayout.IntField(FONT_SIZE, labelParams.fontSize);
            labelParams.font = EditorUtilities.fontField(FONT, labelParams.font);
            labelParams.color = EditorGUILayout.ColorField(COLOR, labelParams.color);
            labelParams.fontTexture = EditorUtilities.textureField(TEXTURE, labelParams.fontTexture);
        }
        
        private void DrawUIButton()
        {
            UIButtonParams buttonParams = m_ToggleParams as UIButtonParams;
            if(buttonParams == null)
            {
                CreateParamsForType(m_ToggleParams.uiType);
                buttonParams = m_ToggleParams as UIButtonParams;
            }
            buttonParams.disabled = EditorGUILayout.Toggle(DISABLED, buttonParams.disabled);
            EditorGUILayout.LabelField(BUTTON + TEXTURE);
            buttonParams.disabledTexture = EditorUtilities.textureField(DISABLED, buttonParams.disabledTexture);
            buttonParams.normalTexture = EditorUtilities.textureField(NORMAL, buttonParams.normalTexture);
            buttonParams.hoverTexture = EditorUtilities.textureField(HOVER, buttonParams.hoverTexture);
            buttonParams.downTexture = EditorUtilities.textureField(DOWN, buttonParams.downTexture);
            buttonParams.enabledTextColor = EditorGUILayout.ColorField(ENABLED_TEXT_COLOR, buttonParams.enabledTextColor);
            buttonParams.disabledTextColor = EditorGUILayout.ColorField(DISABLED_TEXT_COLOR, buttonParams.disabledTextColor);
            buttonParams.eventListener = EditorUtilities.ObjectField<UIEventListener>(UI_EVENT_LISTENER, buttonParams.eventListener);

            buttonParams.labelText = EditorGUILayout.TextField(TEXT, buttonParams.labelText);
            buttonParams.labelFontSize = EditorGUILayout.IntField(FONT_SIZE, buttonParams.labelFontSize);
            buttonParams.labelFont = EditorUtilities.fontField(FONT, buttonParams.labelFont);
            buttonParams.labelColor = EditorGUILayout.ColorField(COLOR, buttonParams.labelColor);
            buttonParams.labelFontTexture = EditorUtilities.textureField(TEXTURE, buttonParams.labelFontTexture);

            EditorGUILayout.BeginHorizontal();
            buttonParams.imageWidth = EditorGUILayout.FloatField(WIDTH, buttonParams.imageWidth);
            buttonParams.imageHeight = EditorGUILayout.FloatField(HEIGHT, buttonParams.imageHeight);
            EditorGUILayout.EndHorizontal();
            buttonParams.imageMeshBoarder = EditorUtilities.UIBoarderField(MESH_BOARDER, buttonParams.imageMeshBoarder);
            buttonParams.imageOuterUVBoarder = EditorUtilities.UIBoarderField(OUTER_UV_BOARDER, buttonParams.imageOuterUVBoarder);
            buttonParams.imageInnerUVBoarder = EditorUtilities.UIBoarderField(INNER_UV_BOARDER, buttonParams.imageInnerUVBoarder);
            buttonParams.imageTexture = EditorUtilities.ObjectField<Texture>(TEXTURE, buttonParams.imageTexture);
            buttonParams.imageShader = EditorUtilities.ObjectField<Shader>(SHADER, buttonParams.imageShader);
            buttonParams.imageColor = EditorGUILayout.ColorField(COLOR, buttonParams.imageColor);
            
        }
    }
}