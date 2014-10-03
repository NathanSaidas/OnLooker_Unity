using UnityEngine;
using System.Collections.Generic;


    /// <summary>
    /// Attach this script onto a game object in the 'init_scene' or equivilent to start using it.
    /// </summary>
    public class DebugUtils : MonoBehaviour
    {
        private static DebugUtils s_Instance;
        public static DebugUtils instance
        {
            get { return s_Instance; }
        }

        /// <summary>
        /// Defines the x and y position of the window
        /// </summary>
        [SerializeField]
        private Rect m_WatchArea = new Rect(0.0f, 0.0f, 100.0f, 100.0f);

        /// <summary>
        /// The scroll position of the scroll view within the window
        /// </summary>
        private Vector2 m_ScrollPosition = Vector2.zero;
        /// <summary>
        /// The list of watches were currently 'watching'
        /// </summary>
        private List<IDebugWatch> m_Watches = new List<IDebugWatch>();
        /// <summary>
        /// The list of Gizmos to draw
        /// </summary>
        private List<IDebugGizmo> m_Gizmos = new List<IDebugGizmo>();

        /// <summary>
        /// Initializes the singleton
        /// </summary>
        void Start()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else
            {
                Debug.LogWarning("Attempting to create more than one DebugUtils. Deleting this one. Possible empty gameobject.");
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(gameObject);
            
        }
        /// <summary>
        /// Destroys the singleton
        /// </summary>
        void OnDestroy()
        {
            Debug.LogWarning("Destroy DebugUtils");
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }

        /// <summary>
        /// Adds a watch to the list for drawing. Multiple watches of the same reference cannot be added.
        /// </summary>
        /// <param name="aWatch">The watch to add.</param>
        public static void addWatch(IDebugWatch aWatch)
        {
            if (instance == null)
            {
                Debug.LogError("Missing \'DebugUtils\' from the scene. Please start from the 'init_scene' or create a \'DebugUtils\' instance.");
                return;
            }
            if (aWatch != null && instance.m_Watches.Contains(aWatch) == false)
            {
                instance.m_Watches.Add(aWatch);
            }
        }
        /// <summary>
        /// Removes the watch from the list.
        /// </summary>
        /// <param name="aWatch">The triggering watch.</param>
        public static void removeWatch(IDebugWatch aWatch)
        {
            if (instance == null)
            {
                Debug.LogError("Missing \'DebugUtils\' from the scene. Please start from the 'init_scene' or create a \'DebugUtils\' instance.");
                return;
            }
            if (aWatch != null)
            {
                instance.m_Watches.Remove(aWatch);
            }
        }
        /// <summary>
        /// Adds a gizmo to the list. Multiple gizmos with the same reference cannot be added.
        /// </summary>
        /// <param name="aGizmo"></param>
        public static void addGizmo(IDebugGizmo aGizmo)
        {
            if (instance == null)
            {
                Debug.LogError("Missing \'DebugUtils\' from the scene. Please start from the 'init_scene' or create a \'DebugUtils\' instance.");
                return;
            }
            if (aGizmo != null && instance.m_Gizmos.Contains(aGizmo) == false)
            {
                instance.m_Gizmos.Add(aGizmo);
            }
        }
        /// <summary>
        /// Removes a gizmo from the list.
        /// </summary>
        /// <param name="aGizmo"></param>
        public static void removeGizmo(IDebugGizmo aGizmo)
        {
            if (instance == null)
            {
                Debug.LogError("Missing \'DebugUtils\' from the scene. Please start from the 'init_scene' or create a \'DebugUtils\' instance.");
                return;
            }
            if (aGizmo != null)
            {
                instance.m_Gizmos.Remove(aGizmo);
            }
        }

        /// <summary>
        /// Use this function to draw the watch within the window.
        /// </summary>
        /// <param name="aSenderName">The name of the class/struct request a watch draw call</param>
        /// <param name="aPropertyName">The name of the property being watched.</param>
        /// <param name="aValue">The value of the property being watched.</param>
        public static void drawWatch(string aSenderName, string aPropertyName, bool aValue)
        {
            drawWatch(aSenderName, aPropertyName, (aValue == true ? "TRUE" : "FALSE"));
        }
        /// <summary>
        /// Use this function to draw the watch within the window.
        /// </summary>
        /// <param name="aSenderName">The name of the class/struct request a watch draw call</param>
        /// <param name="aPropertyName">The name of the property being watched.</param>
        /// <param name="aValue">The value of the property being watched.</param>
        public static void drawWatch(string aSenderName, string aPropertyName, string aValue)
        {
            GUILayout.Label(aSenderName + " | " + aPropertyName + ": " + aValue);
        }
        /// <summary>
        /// Use this function to draw the watch within the window.
        /// </summary>
        /// <param name="aSenderName">The name of the class/struct request a watch draw call</param>
        /// <param name="aPropertyName">The name of the property being watched.</param>
        /// <param name="aValue">The value of the property being watched.</param>
        public static void drawWatch(string aSenderName, string aPropertyName, object aValue)
        {
            GUILayout.Label(aSenderName + " | " + aPropertyName + ": " + aValue);
        }
        /// <summary>
        /// Creates and draws a window.
        /// </summary>
        void OnGUI()
        {
            if (m_Watches.Count > 0)
            {
                m_WatchArea = GUI.Window(0, m_WatchArea, watchWindow, "Debug Watch");
            }
        }


        /// <summary>
        /// The callback to draw a window with all the watches. This will also cleanup any null watches from the list.
        /// </summary>
        /// <param name="aId"></param>
        void watchWindow(int aId)
        {
            GUI.DragWindow();
            GUILayout.BeginArea(new Rect(0.0f, 20.0f, m_WatchArea.width, m_WatchArea.height));
            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            List<IDebugWatch>.Enumerator iterator = m_Watches.GetEnumerator();

            while (iterator.MoveNext())
            {
                if (iterator.Current == null)
                {
                    m_Watches.Remove(iterator.Current);
                    continue;
                }
                iterator.Current.onReport();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        /// <summary>
        /// Draws all the gizmos.
        /// </summary>
        void OnDrawGizmos()
        {
            List<IDebugGizmo>.Enumerator iterator = m_Gizmos.GetEnumerator();

            while (iterator.MoveNext())
            {
                if (iterator.Current == null)
                {
                    m_Gizmos.Remove(iterator.Current);
                    continue;
                }
                iterator.Current.onDebugDraw();
            }
        }

    }
