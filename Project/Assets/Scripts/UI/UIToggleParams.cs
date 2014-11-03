using UnityEngine;
using System.Collections;

namespace Gem
{

    public class UIToggleParams
    {
        private string m_Name = string.Empty;
        private int m_ID = -1;
        private bool m_RecieveActions = false;
        private bool m_IsSelectable = false;
        private UISpace m_UISpace = UISpace.TWO_DIMENSIONAL;
        private UIType m_UIType = UIType.IMAGE;

        /// <summary>
        /// Resets the UIToggleParmams back to defaults
        /// </summary>
        public virtual void Clear()
        {
            m_Name = string.Empty;
            m_ID = -1;
            m_RecieveActions = false;
            m_IsSelectable = false;
            m_UISpace = UISpace.TWO_DIMENSIONAL;
        }

        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public int id
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        public bool recieveActions
        {
            get { return m_RecieveActions; }
            set { m_RecieveActions = value; }
        }
        public bool isSelectable
        {
            get { return m_IsSelectable; }
            set { m_IsSelectable = value; }
        }
        public UISpace uiSpace
        {
            get { return m_UISpace; }
            set { m_UISpace = value; }
        }
        public UIType uiType
        {
            get { return m_UIType;}
            set { m_UIType = value; }
        }
    }
}