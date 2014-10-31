using UnityEngine;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

#region CHANGE LOG
/* October,30,2014 - Nathan Hanlan, Adding more support for the DebugUtils.
 * October,30,2014 - Nathan Hanlan, DebugUtils now extends unity debug logging.
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// Attach this script onto a game object in the 'init_scene' or equivilent to start using it.
    /// </summary>
    public class DebugUtils : MonoBehaviour
    {
        #region DEBUG CONSTANTS
        private const string MISSING_DEBUG_UTILS = "Missing \'DebugUtils\' from the scene. Please start from the 'init_scene' or create a \'DebugUtils\' instance.";
        #endregion

        #region CONSOLE LANGUAGE
        private const string VIEW_SEND = "send";

        #region ARGUMENTS
        //4
        private const string TRUE = "true";
        private const string SEND = "send";
        private const string SHOW = "show";
        private const string HIDE = "hide";
        //5
        private const string FALSE = "false";
        private const string WATCH = "watch";
        //6
        private const string CAMERA = "camera";
        private const string PLAYER = "player";
        private const string CURSOR = "cursor";
        //7
        private const string CONSOLE = "console";

        #endregion

        #region COMMAND
        //3
        private const string COMMAND_SET = "set"; //sets x context with args
        //4
        private const string COMMAND_HELP = "help"; //shows help menu
        //4
        private const string COMMAND_LOAD = "load"; //load <levelname> #option<check point> loads the level, spawns the player at the check point
        //5
        private const string COMMAND_CLEAR = "clear"; //Clears the console.
        //6
        private const string COMMAND_RELOAD = "reload"; //reloads the current level
        //7
        private const string COMMAND_RESTART = "restart"; //restarts from the beginning of the game
        
        
        #endregion


        private const string CONSOLE_LOG = "[Log]";
        private const string CONSOLE_WARNING = "[Warning]:";
        private const string CONSOLE_ERROR = "[Error]:";
        private static readonly Color CONSOLE_LOG_COLOR = Color.white;
        private static readonly Color CONSOLE_WARNING_COLOR = Color.yellow;
        private static readonly Color CONSOLE_ERROR_COLOR = Color.red;
        #endregion

        private static DebugUtils s_Instance;
        public static DebugUtils instance
        {
            get { return s_Instance; }
        }
        /// <summary>
        /// The scroll position of the scroll view within the window
        /// </summary>
        private Vector2 m_WatchScrollPosition = Vector2.zero;
        /// <summary>
        /// The list of watches were currently 'watching'
        /// </summary>
        private List<IDebugWatch> m_Watches = new List<IDebugWatch>();
        /// <summary>
        /// The list of Gizmos to draw
        /// </summary>
        private List<IDebugGizmo> m_Gizmos = new List<IDebugGizmo>();
        /// <summary>
        /// Defines the x and y position of the window
        /// </summary>
        private Rect m_WatchArea = new Rect(0.0f, 0.0f, 200.0f, 100.0f);
        /// <summary>
        /// Determines if the watch window is shown or not 
        /// </summary>
        private bool m_ShowWatch = false;
        /// <summary>
        /// Determines if the consoel is shown or not.
        /// </summary>
        private bool m_ShowConsole = false;
        /// <summary>
        /// The current string in the console
        /// </summary>
        private string m_ConsoleString = string.Empty;
        /// <summary>
        /// The current messages in the console.
        /// </summary>
        private Queue<ConsoleMessage> m_ConsoleStrings = new Queue<ConsoleMessage>();
        /// <summary>
        /// The amount of console strings allowed
        /// </summary>
        private int m_ConsoleLogLength = 30;
        /// <summary>
        /// The scroll position for the console
        /// </summary>
        private Vector2 m_ConsoleScrollPosition = Vector2.zero;
        /// <summary>
        /// Console Area
        /// </summary>
        private Rect m_DebugConsoleArea = new Rect(0.0f, 0.0f, 400.0f, 100.0f);

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
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(gameObject);

        }
        /// <summary>
        /// Checks for user input.
        /// </summary>
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F3))
            {
                m_ShowConsole = !m_ShowConsole;
            }
        }
        /// <summary>
        /// Destroys the singleton
        /// </summary>
        void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
        #region LOGGING
        public static void Log(object aMessage)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(aMessage);
#else
            ConsoleLog(aMessage.ToString());
#endif
        }
        public static void LogError(object aMessage)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(aMessage);
#else
            ConsoleError(aMessage.ToString());
#endif
        }
        public static void LogWarning(object aMessage)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(aMessage);
#else
            ConsoleWarning(aMessage.ToString());
#endif
        }
        public static void LogException(Exception aMessage)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(aMessage);
#endif
        }
        public static void Break(object aMessage)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Break();
#endif
        }
        #endregion
        #region GIZMOS && WATCHES
        /// <summary>
        /// Adds a watch to the list for drawing. Multiple watches of the same reference cannot be added.
        /// </summary>
        /// <param name="aWatch">The watch to add.</param>
        public static void AddWatch(IDebugWatch aWatch)
        {
            if (instance == null)
            {
                LogError(MISSING_DEBUG_UTILS);
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
        public static void RemoveWatch(IDebugWatch aWatch)
        {
            if (instance == null)
            {
                LogError(MISSING_DEBUG_UTILS);
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
        public static void AddGizmo(IDebugGizmo aGizmo)
        {
            if (instance == null)
            {
                LogError(MISSING_DEBUG_UTILS);
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
        public static void RemoveGizmo(IDebugGizmo aGizmo)
        {
            if (instance == null)
            {
                LogError(MISSING_DEBUG_UTILS);
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
        public static void DrawProperty(string aSenderName, string aPropertyName, bool aValue)
        {
            DrawProperty(aSenderName, aPropertyName, (aValue == true ? TRUE : FALSE));
        }
        /// <summary>
        /// Use this function to draw the watch within the window.
        /// </summary>
        /// <param name="aSenderName">The name of the class/struct request a watch draw call</param>
        /// <param name="aPropertyName">The name of the property being watched.</param>
        /// <param name="aValue">The value of the property being watched.</param>
        public static void DrawProperty(string aSenderName, string aPropertyName, string aValue)
        {
            GUILayout.Label(aSenderName + " | " + aPropertyName + ": " + aValue);
        }
        /// <summary>
        /// Use this function to draw the watch within the window.
        /// </summary>
        /// <param name="aSenderName">The name of the class/struct request a watch draw call</param>
        /// <param name="aPropertyName">The name of the property being watched.</param>
        /// <param name="aValue">The value of the property being watched.</param>
        public static void DrawProperty(string aSenderName, string aPropertyName, object aValue)
        {
            GUILayout.Label(aSenderName + " | " + aPropertyName + ": " + aValue);
        }
        /// <summary>
        /// Creates and draws a window.
        /// </summary>
        void OnGUI()
        {
            if (m_Watches.Count > 0 && m_ShowWatch)
            {
                m_WatchArea = GUI.Window(0, m_WatchArea, WatchWindow, "Debug Watch");
            }
            if(m_ShowConsole)
            {
                OnDrawConsole();
            }

        }
        /// <summary>
        /// The callback to draw a window with all the watches. This will also cleanup any null watches from the list.
        /// </summary>
        /// <param name="aId"></param>
        void WatchWindow(int aId)
        {
            GUI.DragWindow();
            GUILayout.BeginArea(new Rect(0.0f, 20.0f, m_WatchArea.width, m_WatchArea.height));
            m_WatchScrollPosition = GUILayout.BeginScrollView(m_WatchScrollPosition);

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
        #endregion

        /// <summary>
        /// Gets called to draw the debug console
        /// </summary>
        void OnDrawConsole()
        {
            m_DebugConsoleArea.x = 0.0f;
            m_DebugConsoleArea.y = Screen.height - m_DebugConsoleArea.height;
            GUILayout.BeginArea(m_DebugConsoleArea);
            m_ConsoleScrollPosition = GUILayout.BeginScrollView(m_ConsoleScrollPosition);
            IEnumerator<ConsoleMessage> messageIter = m_ConsoleStrings.GetEnumerator();
            while(messageIter.MoveNext())
            {
                ConsoleMessage current = messageIter.Current;
                if(current.message == null || current.message.Length == 0)
                {
                    continue;
                }
                switch(current.logLevel)
                {
                    case LogLevel.ERROR:
                        GUI.contentColor = CONSOLE_ERROR_COLOR;
                        GUILayout.Label(CONSOLE_ERROR + current.message);
                        break;
                    case LogLevel.WARNING:
                        GUI.contentColor = CONSOLE_WARNING_COLOR;
                        GUILayout.Label(CONSOLE_WARNING + current.message);
                        break;
                    case LogLevel.LOG:
                        GUI.contentColor = CONSOLE_LOG_COLOR;
                        GUILayout.Label(CONSOLE_LOG + current.message);
                        break;
                    case LogLevel.USER:
                        GUI.contentColor = CONSOLE_LOG_COLOR;
                        GUILayout.Label(current.message);
                        break;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();

            m_ConsoleString = GUILayout.TextField(m_ConsoleString, GUILayout.Width(m_DebugConsoleArea.width * 0.60f));
            bool enterClicked = false;
            bool showHideConsolePressed = false;

            if(Event.current != null && Event.current.isKey)
            {
                switch(Event.current.keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        enterClicked = true;
                        break;
                    case KeyCode.F3:
                        showHideConsolePressed = true;
                        break;

                }
            }

            bool send = (enterClicked || GUILayout.Button(VIEW_SEND)) && m_ConsoleString.Length != 0;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if(showHideConsolePressed == true)
            {
                return;
            }
            ///Add the message, and parse the words for a command
            if(send)
            {
                ///Add the message into the console 
                AddMessage(m_ConsoleString);
                ///Parse the words from the message string
                List<string> words = Utilities.ParseToWords(m_ConsoleString,true);
                if(words.Count == 0)
                {
                    return;
                }
                string firstWord = words[0];
                switch(firstWord.Length)
                {
                    case 3:
                        OnHandleCommand3(words);
                        break;
                    case 4:
                        OnHandleCommand4(words);
                        break;
                    case 5:
                        OnHandleCommand5(words);
                        break;
                    case 6:
                        OnHandleCommand6(words);
                        break;
                    case 7:
                        OnHandleCommand7(words);
                        break;
                }
                m_ConsoleString = string.Empty;
            }
            
        }

        #region CONSOLE HELPERS
        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="aMessage"></param>
        private void ConsoleLog(object aMessage)
        {
            if (aMessage != null)
            {
                AddMessage(new ConsoleMessage(aMessage.ToString()));
            }
        }
        /// <summary>
        /// Logs a warning to the console.
        /// </summary>
        /// <param name="aMessage"></param>
        private void ConsoleWarning(object aMessage)
        {
            if (aMessage != null)
            {
                AddMessage(new ConsoleMessage(aMessage.ToString(),LogLevel.WARNING));
            }
        }
        /// <summary>
        /// Logs n error to the console.
        /// </summary>
        /// <param name="aMessage"></param>
        private void ConsoleError(object aMessage)
        {
            if (aMessage != null)
            {
                AddMessage(new ConsoleMessage(aMessage.ToString(), LogLevel.ERROR));
            }
        }
        /// <summary>
        /// Clears the console.
        /// </summary>
        private void ConsoleClear()
        {
            m_ConsoleStrings.Clear();
        }
        /// <summary>
        /// Adds a message into the console.
        /// </summary>
        /// <param name="aMessage"></param>
        private void AddMessage(ConsoleMessage aMessage)
        {
            if(m_ConsoleStrings.Count == m_ConsoleLogLength)
            {
                m_ConsoleStrings.Dequeue();
            }
            m_ConsoleStrings.Enqueue(aMessage);
        }
        /// <summary>
        /// Adds a user message to the console.
        /// </summary>
        /// <param name="aMessage"></param>
        private void AddMessage(string aMessage)
        {
            if(aMessage.Length != 0)
            {
                AddMessage(new ConsoleMessage(aMessage, LogLevel.USER));
            }
        }
        #endregion CONSOLE HELPERS
        #region COMMAND PARSING
        //Do Command Parsing here
        private void OnHandleCommand3(List<string> aWords)
        {
            switch(aWords[0])
            {
                case COMMAND_SET:
                    OnCommandSet(aWords);
                    break;
            }
        }
        private void OnHandleCommand4(List<string> aWords)
        {
            switch(aWords[0])
            {
                case COMMAND_HELP:
                    OnCommandHelp(aWords);
                    break;
                case COMMAND_LOAD:
                    OnCommandLoad(aWords);
                    break;
            }
        }
        private void OnHandleCommand5(List<string> aWords)
        {
            switch(aWords[0])
            {
                case COMMAND_CLEAR:
                    OnCommandClear(aWords);
                    break;
            }
        }
        private void OnHandleCommand6(List<string> aWords)
        {
            switch(aWords[0])
            {
                case COMMAND_RELOAD:
                    OnCommandReload(aWords);
                    break;
            }
        }
        private void OnHandleCommand7(List<string> aWords)
        {
            switch(aWords[0])
            {
                case COMMAND_RESTART:
                    OnCommandRestart(aWords);
                    break;
            }
        }
        #endregion
        #region COMMAND HANDLING
        ///Handle all events here.
        /// <summary>
        /// Gets called to process set commands from the console
        /// </summary>
        /// <param name="aWords"></param>
        private void OnCommandSet(List<string> aWords)
        {
            if(aWords.Count < 3)
            {
                return;
            }
            //TODO Set X argument based on the context.
            switch(aWords[1])
            {
                case CONSOLE:
                    {
                        if(aWords[2] == SHOW)
                        {
                            m_ShowConsole = true;
                        }
                        else if(aWords[2] == HIDE)
                        {
                            m_ShowConsole = false;
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// Gets called to process help commands from the console
        /// </summary>
        /// <param name="aWords"></param>
        private void OnCommandHelp(List<string> aWords)
        {
            //TODO: Display Help Menu
            ConsoleLog("Help");
        }
        /// <summary>
        /// Gets called to load a level
        /// </summary>
        /// <param name="aWords"></param>
        private void OnCommandLoad(List<string> aWords)
        {
            if(aWords.Count < 2)
            {
                return;
            }
            //TODO: Game.LoadLevel(aWords[1]);
            ConsoleLog("Load Level: " + aWords[1]);
        }
        /// <summary>
        /// Clears the console.
        /// </summary>
        /// <param name="aWords"></param>
        private void OnCommandClear(List<string> aWords)
        {
            ConsoleClear();
        }
        /// <summary>
        /// Gets called to reload the game.
        /// </summary>
        /// <param name="aWords"></param>
        private void OnCommandReload(List<string> aWords)
        {
            //TODO: Game.Reload();
            ConsoleLog("Reload");
        }
        /// <summary>
        /// Gets called to restart the game.
        /// </summary>
        /// <param name="aWords"></param>
        private void OnCommandRestart(List<string> aWords)
        {
            //TODO: Game.Restart();
            ConsoleLog("Restart");
        }

        #endregion
    }
}