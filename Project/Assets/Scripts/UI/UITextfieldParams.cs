using UnityEngine;
using System.Collections;

namespace Gem
{

    public class UITextfieldParams : UIToggleParams
    {
        /// Button Parameters


        private bool m_Disabled = false;
        private Texture m_DisabledTexture = null;
        private Texture m_NormalTexture = null;
        private Texture m_HoverTexture = null;
        private Texture m_DownTexture = null;
        private Color m_EnabledTextColor = Color.white;
        private Color m_DisabledTextColor = Color.gray;
        private UIEventListener m_EventListener = null;
        private int m_MaxCharacter = 0;
        /// Label Parameters


        private string m_LabelText = string.Empty;
        private int m_LabelFontSize = 100;
        private Font m_LabelFont = null;
        private Texture m_LabelFontTexture = null;
        private Color m_LabelColor = Color.white;

        ///Image Parameters
        private float m_ImageWidth = 1.0f;
        private float m_ImageHeight = 1.0f;
        private UIBoarder m_ImageMeshBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
        private UIBoarder m_ImageOuterUVBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
        private UIBoarder m_ImageInnerUVBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
        private Texture m_ImageTexture = null;
        private Shader m_ImageShader = null;
        private Color m_ImageColor = Color.white;

        public override void Clear()
        {
            base.Clear();
            m_Disabled = false;
            m_DisabledTexture = null;
            m_NormalTexture = null;
            m_HoverTexture = null;
            m_DownTexture = null;
            m_EnabledTextColor = Color.white;
            m_DisabledTextColor = Color.gray;
            m_EventListener = null;
            m_MaxCharacter = 0;
            m_LabelText = string.Empty;
            m_LabelFontSize = 100;
            m_LabelFont = null;
            m_LabelFontTexture = null;
            m_LabelColor = Color.white;
            m_ImageWidth = 1.0f;
            m_ImageHeight = 1.0f;
            m_ImageMeshBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
            m_ImageOuterUVBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
            m_ImageInnerUVBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
            m_ImageTexture = null;
            m_ImageShader = null;
            m_ImageColor = Color.white;
        }

        public bool disabled
        {
            get { return m_Disabled; }
            set { m_Disabled = value; }
        }
        public Texture disabledTexture
        {
            get { return m_DisabledTexture; }
            set { m_DisabledTexture = value; }
        }
        public Texture normalTexture
        {
            get { return m_NormalTexture; }
            set { m_NormalTexture = value; }
        }
        public Texture hoverTexture
        {
            get { return m_HoverTexture; }
            set { m_HoverTexture = value; }
        }
        public Texture downTexture
        {
            get { return m_DownTexture; }
            set { m_DownTexture = value; }
        }
        public Color enabledTextColor
        {
            get { return m_EnabledTextColor; }
            set { m_EnabledTextColor = value; }
        }
        public Color disabledTextColor
        {
            get { return m_DisabledTextColor; }
            set { m_DisabledTextColor = value; }
        }
        public UIEventListener eventListener
        {
            get { return m_EventListener; }
            set { m_EventListener = value; }
        }
        public string labelText
        {
            get { return m_LabelText; }
            set { m_LabelText = value; }
        }
        public int labelFontSize
        {
            get { return m_LabelFontSize; }
            set { m_LabelFontSize = value; }
        }
        public Font labelFont
        {
            get { return m_LabelFont; }
            set { m_LabelFont = value; }
        }
        public Texture labelFontTexture
        {
            get { return m_LabelFontTexture; }
            set { m_LabelFontTexture = value; }
        }
        public Color labelColor
        {
            get { return m_LabelColor; }
            set { m_LabelColor = value; }
        }
        public float imageWidth
        {
            get { return m_ImageWidth; }
            set { m_ImageWidth = value; }
        }
        public float imageHeight
        {
            get { return m_ImageHeight; }
            set { m_ImageHeight = value; }
        }
        public UIBoarder imageMeshBoarder
        {
            get { return m_ImageMeshBoarder; }
            set { m_ImageMeshBoarder = value; }
        }
        public UIBoarder imageOuterUVBoarder
        {
            get { return m_ImageOuterUVBoarder; }
            set { m_ImageOuterUVBoarder = value; }
        }
        public UIBoarder imageInnerUVBoarder
        {
            get { return m_ImageInnerUVBoarder; }
            set { m_ImageInnerUVBoarder = value; }
        }
        public Texture imageTexture
        {
            get { return m_ImageTexture; }
            set { m_ImageTexture = value; }
        }
        public Shader imageShader
        {
            get { return m_ImageShader; }
            set { m_ImageShader = value; }
        }
        public Color imageColor
        {
            get { return m_ImageColor; }
            set { m_ImageColor = value; }
        }
        public int maxCharacter
        {
            get { return m_MaxCharacter; }
            set { m_MaxCharacter = value; }
        }
    }
}