using UnityEngine;
using System.Collections;

namespace Gem
{
    public class UILabelParams : UIToggleParams
    {
        private string m_Text = string.Empty;
        private int m_FontSize = 100;
        private Font m_Font = null;
        private Texture m_FontTexture = null;
        private Color m_Color = Color.white;

        public override void Clear()
        {
            base.Clear();
            m_Text = string.Empty;
            m_FontSize = 100;
            m_Font = null;
            m_Color = Color.white;
            m_FontTexture = null;
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
        public Font font
        {
            get { return m_Font; }
            set { m_Font = value; }
        }
        public Texture fontTexture
        {
            get { return m_FontTexture; }
            set { m_FontTexture = value; }
        }
        public Color color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }
    }
}