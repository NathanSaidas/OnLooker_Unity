using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



#region CHANGE LOG
/* October,  26, 2014 - Nathan Hanlan, Added the UIManager class and implemeneted 10% of features.
 * November, 14,2014 - Nathan Hanlan, Removed the Init and DeInit methods as they were not used.
 */
#endregion

namespace Gem
{
    /// <summary>
    /// This class manages all 2D, 3D, and World UI components
    /// </summary>
    [Serializable]
    public class UIManager : MonoBehaviour
    {
        //private class UIInputState
        //{
        //    private Vector3 m_MousePosition = Vector3.zero;
        //    private bool m_MouseDown = false;
        //    private bool m_MouseUp = false;
        //    private bool m_Mouse = false;
        //    private bool m_EnterButton = false; //(A on Xbox)
        //    private bool m_NextButton = false;
        //    private bool m_Previous = false;
        //    private bool m_Up = false;
        //    private bool m_Down = false; 
        //}

        #region CONSTANTS
        /// <summary>
        /// Objects in the world considered a 2D UI, rendered in Orthoview
        /// </summary>
        public const int UI_2D_LAYER = 30;
        /// <summary>
        /// Objects in the world considered a 3D UI, rendered in Perspective View
        /// </summary>
        public const int UI_3D_LAYER = 31;
        /// <summary>
        /// Objects in the world rendered by the gameplay camera.
        /// </summary>
        public const int UI_WORLD_LAYER = 27;
        /// <summary>
        /// How much time a click event has to occur again in order for a double click to be sent.
        /// </summary>
        private const float DOUBLE_CLICK_TIME = 0.25f;

#if UNITY_EDITOR
        private const string INVALID_UI_SETTING = " Toggle has a invalid UI setting";
        private const string MISSING_CAMERA = "Missing Cameras.";
#endif
        #endregion

        #region SINGLETON
        private static UIManager s_Instance = null;
        public static UIManager instance
        {
            get { if (s_Instance == null) { CreateInstance();  } return s_Instance; }
        }
        private static void CreateInstance()
        {
            if (Game.isClosing)
            {
                return;
            }
            GameObject persistant = GameObject.Find(Game.PERSISTANT_GAME_OBJECT_NAME);
            if (persistant == null)
            {
                persistant = new GameObject(Game.PERSISTANT_GAME_OBJECT_NAME);
                persistant.transform.position = Vector3.zero;
                persistant.transform.rotation = Quaternion.identity;
            }
            s_Instance = persistant.GetComponent<UIManager>();
            if (s_Instance == null)
            {
                s_Instance = persistant.AddComponent<UIManager>();
            }
        }
        private static bool SetInstance(UIManager aManager)
        {
            if (s_Instance != null && s_Instance != aManager)
            {
                return false;
            }
            s_Instance = aManager;
            return true;
        }
        private static void DestroyInstance(UIManager aManager)
        {
            if(s_Instance == aManager)
            {
                s_Instance = null;
            }
        }
        void Start()
        {
            if(!SetInstance(this))
            {
                if(Application.isPlaying)
                {
                    Destroy(this);
                }
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        void OnDestroy()
        {
            DestroyInstance(this);
        }
        #endregion

        #region FIELDS
        /// <summary>
        /// The toggle that is currently being moused over.
        /// </summary>
        private UIToggle m_MousedOverToggle = null;
        /// <summary>
        /// The toggle that is currently selected
        /// </summary>
        private UIToggle m_SelectedToggle = null;
        /// <summary>
        /// The toggle that was last clicked.
        /// </summary>
        private UIToggle m_LastClickedToggle = null;
        /// <summary>
        /// A unique number generator for assigning ID's to UIToggles
        /// </summary>
        private UniqueNumberGenerator m_NumberGenerator = new UniqueNumberGenerator();
        /// <summary>
        /// A container for all toggles
        /// </summary>
        private List<UIToggle> m_Toggles = new List<UIToggle>();
        /// <summary>
        /// A container for only 2D toggles
        /// </summary>
        private List<UIToggle> m_2DToggles = new List<UIToggle>();
        /// <summary>
        /// A container for only 3D toggles
        /// </summary>
        private List<UIToggle> m_3DToggles = new List<UIToggle>();
        /// <summary>
        /// A container for only world toggles
        /// </summary>
        private List<UIToggle> m_WorldToggles = new List<UIToggle>();
        /// <summary>
        /// The default UI Shader to use.
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The default UI Shader to use.")]
#endif
        [SerializeField]
        private Shader m_DefaultUIShader = null;
        /// <summary>
        /// Represents the World Max Raycast Distance
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("World Max Raycast Distance")]
#endif
        [SerializeField]
        private float m_WorldDistance = 100.0f;
        /// <summary>
        /// Represents the 3D Max Raycast Distance
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("3D Max Raycast Distance")]
#endif
        [SerializeField]
        private float m_3DDistance = 15.0f;
        /// <summary>
        /// Represents the 2D Max Raycast Distance
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("2D Max Raycast Distance")]
#endif
        [SerializeField]
        private float m_2DDistance = 10.0f;
        /// <summary>
        /// The camera that renders 2D UI's
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The camera that renders 2D UI's.")]
#endif
        [SerializeField]
        private Camera m_2DCamera = null;
        /// <summary>
        /// The camera that renders 2D UI's
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The camera that renders 3D UI's")]
#endif
        [SerializeField]
        private Camera m_3DCamera = null;
        /// <summary>
        /// The camera that renders World UI's
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The camera that renders World UI's")]
#endif
        [SerializeField]
        private Camera m_WorldCamera = null;
        #endregion


        #region METHODS

        /// <summary>
        /// Checks stores the input and handles action / selection events.
        /// </summary>
        void Update()
        {
            ///Query Input
            bool enterButton = (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0));
            bool nextButton = Input.GetKeyDown(KeyCode.RightArrow);
            bool previousButton = Input.GetKeyDown(KeyCode.LeftArrow);
            //bool upButton = Input.GetKeyDown(KeyCode.UpArrow);
            //bool downButton = Input.GetKeyDown(KeyCode.DownArrow);
            Vector3 mousePosition = Input.mousePosition;

            ///Check for an action event
            if(m_SelectedToggle != null && enterButton == true && m_SelectedToggle.receivesActionEvents)
            {
                m_SelectedToggle.OnEvent(UIEvent.ON_ACTION);
            }
            ///Check for next selection event / previous selection
            if (nextButton == true && m_SelectedToggle != null)
            {
                int targetID = m_SelectedToggle.id + 1;
                if(targetID > m_NumberGenerator.largestNumber)
                {
                	targetID = 0;
                }
                UIToggle nextToggle = null;
                int iterations = 0;
                while(nextToggle == null && iterations < m_NumberGenerator.largestNumber + 1)
                {
                	nextToggle = FindToggle(targetID);
                	targetID++;
                	if(targetID > m_NumberGenerator.largestNumber)
                	{
                		targetID = 0;
                	}
                	iterations++;
                }
                if(nextToggle != null)
                {
                	m_SelectedToggle.OnEvent(UIEvent.UNSELECTED);
                	m_SelectedToggle.isSelected = false;
                	m_SelectedToggle = nextToggle;
                	m_SelectedToggle.OnEvent(UIEvent.SELECTED);
                	m_SelectedToggle.isSelected = true;
                }
            }
            else if (previousButton && m_SelectedToggle != null)
            {
				
                int targetID = m_SelectedToggle.id - 1;
                if(targetID < 0)
                {
                    targetID = m_NumberGenerator.largestNumber;
                }
                UIToggle nextToggle = null;
                int iterations = 0;
                while(nextToggle == null && iterations < m_NumberGenerator.largestNumber + 1)
                {
					nextToggle = FindToggle(targetID);
					targetID--;
					if(targetID < 0)
					{
						targetID = m_NumberGenerator.largestNumber;
					}
					iterations++;
                }
                
				if(nextToggle != null)
				{
					m_SelectedToggle.OnEvent(UIEvent.UNSELECTED);
					m_SelectedToggle.isSelected = false;
					m_SelectedToggle = nextToggle;
					m_SelectedToggle.OnEvent(UIEvent.SELECTED);
					m_SelectedToggle.isSelected = true;
				}
            }

            if (m_2DCamera == null || m_3DCamera == null || m_WorldCamera == null)
            {
#if UNITY_EDITOR
                Debug.LogError(MISSING_CAMERA);
#endif
                return;
            }

            UIToggle toggle2D = Check2D(ref mousePosition);
            UIToggle toggle3D = Check3D(ref mousePosition);
            UIToggle toggleWorld = CheckWorld(ref mousePosition);

            if (toggle2D != null && toggle2D.uiSpace != UISpace.TWO_DIMENSIONAL)
            {
#if UNITY_EDITOR
                Debug.LogError(toggle2D.name + INVALID_UI_SETTING);
#endif
                toggle2D = null;
            }
            if (toggle3D != null && toggle3D.uiSpace != UISpace.THREE_DIMENSIONAL)
            {
#if UNITY_EDITOR
                Debug.LogError(toggle3D.name + INVALID_UI_SETTING);
#endif
                toggle3D = null;
            }
            if (toggleWorld != null && toggleWorld.uiSpace != UISpace.WORLD)
            {
#if UNITY_EDITOR
                Debug.LogError(toggle3D.name + INVALID_UI_SETTING);
#endif
                toggleWorld = null;
            }

            if (toggle2D != null)
            {
                HandleToggleEvents(toggle2D);
                return;
            }
            else if (toggle3D != null)
            {
                HandleToggleEvents(toggle3D);
                return;
            }
            else if (toggleWorld != null)
            {
                HandleToggleEvents(toggleWorld);
                return;
            }
            else
            {
                HandleDefaultEvents();
            }

        }
        #region RAYCAST HELPERS
        /// <summary>
		/// Checks for mouse over 2D Toggles
		/// </summary>
		/// <returns>The d.</returns>
		/// <param name="aMousePosition">A mouse position.</param>
        private UIToggle Check2D(ref Vector3 aMousePosition)
        {
            if(m_2DToggles == null || m_2DToggles.Count <= 0)
            {
                return null;
            }
            int layerMask = 1 << UI_2D_LAYER;
            RaycastHit hit;
            Ray ray = m_2DCamera.ScreenPointToRay(aMousePosition);
            //Debug.DrawLine(ray.origin, ray.origin + ray.direction * m_2DDistance,Color.white,1.0f);
            //Debug.DrawRay(ray.origin, ray.direction, Color.white, 1.0f);
            if(!Physics.Raycast(ray,out hit,m_2DDistance,layerMask))
            {
                
                return null;
            }
            return hit.collider.GetComponent<UIToggle>();
        }
		/// <summary>
		/// Check for mouse over 3D toggles
		/// </summary>
		/// <returns>The d.</returns>
		/// <param name="aMousePosition">A mouse position.</param>
        private UIToggle Check3D(ref Vector3 aMousePosition)
        {
            if(m_3DToggles == null || m_3DToggles.Count <= 0)
            {
                return null;
            }
            int layerMask = 1 << UI_3D_LAYER;
            RaycastHit hit;
            Ray ray = m_3DCamera.ScreenPointToRay(aMousePosition);
            if(!Physics.Raycast(ray,out hit,m_3DDistance,layerMask))
            {
                return null;
            }
            return hit.collider.GetComponent<UIToggle>();
        }
        /// <summary>
        /// Check for mouse over world toggles
        /// </summary>
        /// <returns>The world.</returns>
        /// <param name="aMousePosition">A mouse position.</param>
        private UIToggle CheckWorld(ref Vector3 aMousePosition)
        {
            if (m_WorldToggles == null || m_WorldToggles.Count <= 0)
            {
                return null;
            }
            int layerMask = 1 << UI_WORLD_LAYER;
            RaycastHit hit;
            Ray ray = m_WorldCamera.ScreenPointToRay(aMousePosition);
            if (!Physics.Raycast(ray, out hit, m_WorldDistance, layerMask))
            {
                return null;
            }
            return hit.collider.GetComponent<UIToggle>();
        }
        #endregion
        /// <summary>
		/// Handles the toggle events (mouse enter,exit,hover, mouse down, click, double click etc...)
		/// </summary>
		/// <param name="aToggle">A toggle.</param>
        void HandleToggleEvents(UIToggle aToggle)
        {
            bool mouse = Input.GetMouseButton(0);
            bool mouseDown = Input.GetMouseButtonDown(0);
            bool mouseUp = Input.GetMouseButtonUp(0);

            ///Check 2D Mouse Enter / Exit / Hover Events
            if (aToggle != m_MousedOverToggle && m_MousedOverToggle != null)
            {
                m_MousedOverToggle.OnEvent(UIEvent.MOUSE_EXIT);
                m_MousedOverToggle.hasMouseOver = false;
            }
            m_MousedOverToggle = aToggle;
            if (aToggle.hasMouseOver == false)
            {
                aToggle.OnEvent(UIEvent.MOUSE_ENTER);
            }
            aToggle.hasMouseOver = true;
            aToggle.OnEvent(UIEvent.MOUSE_HOVER);

            ///Check for mouse down, click and selection events
            if (mouse)
            {
                aToggle.OnEvent(UIEvent.MOUSE_DOWN);
            }
            ///Check for mouse click and selection events
            if (mouseUp)
            {
                if (m_LastClickedToggle == aToggle)
                {
                    //Unselect
                    if (m_SelectedToggle != aToggle && m_SelectedToggle != null)
                    {
                        m_SelectedToggle.OnEvent(UIEvent.UNSELECTED);
                        m_SelectedToggle.isSelected = false;
                        m_SelectedToggle = null;
                    }
                    //Select
                    if (aToggle.isSelected == false && aToggle.selectable )
                    {
                        m_SelectedToggle = aToggle;
                        m_SelectedToggle.OnEvent(UIEvent.SELECTED);
                        m_SelectedToggle.isSelected = true;
                    }
                    //Calculate click delta then perform a click or double click event
                    float clickTime = Time.fixedTime - m_LastClickedToggle.lastClickTime;
                    if (clickTime > DOUBLE_CLICK_TIME)
                    {
                        m_LastClickedToggle.OnEvent(UIEvent.MOUSE_CLICK);
                    }
                    else
                    {
                        m_LastClickedToggle.OnEvent(UIEvent.MOUSE_DOUBLE_CLICK);
                    }
                    ///Set last click time
                    m_LastClickedToggle.lastClickTime = Time.fixedTime;
                }
            }

            ///Prepare for a click event
            if (mouseDown)
            {
                m_LastClickedToggle = aToggle;
            }
        }
		/// <summary>
		/// Handles events if a toggle event was not handled
		/// </summary>
        void HandleDefaultEvents()
        {
            ///Check 2D Mouse Enter / Exit / Hover Events
            if (m_MousedOverToggle != null)
            {
                m_MousedOverToggle.OnEvent(UIEvent.MOUSE_EXIT);
                m_MousedOverToggle.hasMouseOver = false;
            }
            m_MousedOverToggle = null;


            if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0))
            {
                if(m_SelectedToggle != null)
                {
                    m_SelectedToggle.OnEvent(UIEvent.UNSELECTED);
                    m_SelectedToggle.isSelected = false;
                    m_SelectedToggle = null;
                }
            }
        }
        #region REGISTER_HELPERS
        /// <summary>
		/// Registers the toggle with the UI Manager
		/// </summary>
		/// <param name="aToggle">The toggle to register</param>
        private void RegisterUIToggle(UIToggle aToggle)
        {
            if(aToggle == null || m_Toggles.Contains(aToggle))
            {
                return;
            }
            int id = aToggle.id;
            if(!m_NumberGenerator.Reserve(id))
            {
            	id = m_NumberGenerator.Get();
            }
            
            switch(aToggle.uiSpace)
            {
                case UISpace.TWO_DIMENSIONAL:
                    aToggle.id = id;
                    m_Toggles.Add(aToggle);
                    m_2DToggles.Add(aToggle);
                    break;
                case UISpace.THREE_DIMENSIONAL:
                    aToggle.id = id;
                    m_Toggles.Add(aToggle);
                    m_3DToggles.Add(aToggle);
                    break;
                case UISpace.WORLD:
                    aToggle.id = id;
                    m_Toggles.Add(aToggle);
                    m_WorldToggles.Add(aToggle);
                    break;
            }

        }
        /// <summary>
		/// Registers the toggle with the UI Manager
		/// </summary>
		/// <param name="aToggle">The toggle to register</param>
        public static void Register(UIToggle aToggle)
        {
            if(instance == null)
            {
                return;
            }
            instance.RegisterUIToggle(aToggle);
        }
        
        /// <summary>
        /// Unregisters the toggle from the UI Manager
        /// </summary>
        /// <param name="aToggle">The toggle to unregister.</param>
        private void UnregisterUIToggle(UIToggle aToggle)
        {
            if(aToggle == null)
            {
                return;
            }
            m_NumberGenerator.Free(aToggle.id);
            switch (aToggle.uiSpace)
            {
                case UISpace.TWO_DIMENSIONAL:
                    m_Toggles.Remove(aToggle);
                    m_2DToggles.Remove(aToggle);
                    break;
                case UISpace.THREE_DIMENSIONAL:
                    m_Toggles.Remove(aToggle);
                    m_3DToggles.Remove(aToggle);
                    break;
                case UISpace.WORLD:
                    m_Toggles.Remove(aToggle);
                    m_WorldToggles.Remove(aToggle);
                    break;
            }
        }
        /// <summary>
        /// Unregisters the toggle from the UI Manager
        /// </summary>
        /// <param name="aToggle">The toggle to unregister.</param>
        public static void Unregister(UIToggle aToggle)
        {
            if(instance == null)
            {
                return;
            }
            instance.UnregisterUIToggle(aToggle);
        }
        #endregion


        /// <summary>
        /// Finds the toggle for the given ID, toggle must be selectable.
        /// </summary>
        /// <returns>The toggle found.</returns>
        /// <param name="aID">The ID to search for.</param>
		private UIToggle FindToggle(int aID)
		{
			List<UIToggle>.Enumerator iter = m_Toggles.GetEnumerator();
			while(iter.MoveNext())
			{
				if(iter.Current == null || iter.Current.selectable == false)
				{
					continue;
				}
				if(iter.Current.id == aID)
				{
					return iter.Current;
				}
			}
			return null;
		}
        /// <summary>
        /// Finds a UI Toggle in the UI Manager by ID
        /// </summary>
        /// <param name="aID">The ID to search for</param>
        /// <returns></returns>
        public static UIToggle Find(int aID)
        {
            if(instance == null)
            {
                return null;
            }
            List<UIToggle>.Enumerator iter = instance.m_Toggles.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                if(iter.Current.id == aID)
                {
                    return iter.Current;
                }
            }
            return null;
        }
        /// <summary>
        /// Finds a UI Toggle in the UI manager by name
        /// </summary>
        /// <param name="aName">The name to search for</param>
        /// <returns></returns>
        public static UIToggle Find(string aName)
        {
            if(instance == null)
            {
                return null;
            }
            List<UIToggle>.Enumerator iter = instance.m_Toggles.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }
                if (iter.Current.gameObject.name == aName)
                {
                    return iter.Current;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Finds a toggle where the name matches and Type matches
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aName"></param>
        /// <returns></returns>
        public static T Find<T>(string aName) where T : MonoBehaviour
        {
            IEnumerator<UIToggle> enumerator = instance.m_Toggles.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current == null)
                {
                    continue;
                }
                if(enumerator.Current.name == aName)
                {
                    T comp = enumerator.Current.GetComponentInChildren<T>();
                    if(comp != null)
                    {
                        return comp;
                    }
                }
                
            }
            return null;
        }
        #endregion

        /// <summary>
        /// The default shader used by all UI
        /// </summary>
        public static Shader defaultShader
        {
            get { return instance == null ? null : instance.m_DefaultUIShader; }
        }
        /// <summary>
        /// The camera for 2D UI's
        /// </summary>
        public static Camera camera2D
        {
            get { return instance == null ? null : instance.m_2DCamera; }
        }
        /// <summary>
        /// The camera for 3D UI's
        /// </summary>
        public static Camera camera3D
        {
            get { return instance == null ? null : instance.m_3DCamera; }
        }
        /// <summary>
        /// The camera for World UI's
        /// </summary>
        public static Camera cameraWorld
        {
            get { return instance == null ? null : instance.m_WorldCamera; }
        }
        /// <summary>
        /// The currently selected toggle
        /// </summary>
        public static UIToggle selectedToggle
        {
            get { return instance == null ? null : instance.m_SelectedToggle; }
        }
        /// <summary>
        /// The currently moused over toggle.
        /// </summary>
        public static UIToggle mouseOverToggle
        {
            get { return instance == null ? null : instance.m_MousedOverToggle; }
        }
    }
}