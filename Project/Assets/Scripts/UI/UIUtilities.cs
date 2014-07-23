using UnityEngine;
using System;

namespace OnLooker
{
    namespace UI
    {
        public enum UIAnchor
        {
            NONE,
            CAMERA,
            OBJECT
        }

        [Serializable()]
        public class UIArguments
        {
            private string m_ToggleName = string.Empty;
            private Vector3 m_Position = Vector3.zero;
            private Vector3 m_Rotation = Vector3.zero;
            private UIAnchor m_AnchorMode = UIAnchor.CAMERA;
            private bool m_SmoothTransform = true;
            private bool m_Interactive = true;
            private bool m_TrapDoubleClick = false;


            private string m_Text = string.Empty;
            private int m_FontSize = 20;

            private Texture m_Texture = null;

            public string toggleName
            {
                get { return m_ToggleName; }
                set { m_ToggleName = value; }
            }
            public Vector3 position
            {
                get { return m_Position; }
                set { m_Position = value; }
            }
            public Vector3 rotation
            {
                get { return m_Rotation; }
                set { m_Rotation = value; }
            }
            public UIAnchor anchorMode
            {
                get { return m_AnchorMode; }
                set { m_AnchorMode = value; }
            }
            public bool smoothTransform
            {
                get { return m_SmoothTransform; }
                set { m_SmoothTransform = value; }
            }
            public bool interactive
            {
                get { return m_Interactive; }
                set { m_Interactive = value; }
            }
            public bool trapDoubleClick
            {
                get { return m_TrapDoubleClick; }
                set { m_TrapDoubleClick = value; }
            }

            public string text
            {
                get { return m_Text; }
                set { m_Text = value; }
            }
            public int fontSize
            {
                get { return m_FontSize; }
                set { m_FontSize = value; }
            }
            public Texture texture
            {
                get { return m_Texture; }
                set { m_Texture = value; }
            }
        }

        public class UIUtilities
        {
            public const int NO_LIMIT = -1;
        }



    }
}
