using UnityEngine;
using System.Collections;

namespace Gem
{

    public class UIImageParams : UIToggleParams
    {
        private float m_Width = 1.0f;
        private float m_Height = 1.0f;
        private UIBoarder m_MeshBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
        private UIBoarder m_OuterUVBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
        private UIBoarder m_InnerUVBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
        private Texture m_Texture = null;
        private Shader m_Shader = null;
        private Color m_Color = Color.white;

        public override void Clear()
        {
            base.Clear();
            m_Width = 1.0f;
            m_Height = 1.0f;
            m_MeshBoarder.Set(0.1f, 0.9f, 0.9f, 0.1f);
            m_OuterUVBoarder.Set(0.1f, 0.9f, 0.9f, 0.1f);
            m_InnerUVBoarder.Set(0.1f, 0.9f, 0.9f, 0.1f);
            m_Texture = null;
            m_Shader = Shader.Find(UIUtilities.SHADER_UNLIT_TRANSPARENT);
            m_Color = Color.white ;
        }

        public float width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }
        public float height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }
        public UIBoarder meshBoarder
        {
            get { return m_MeshBoarder; }
            set { m_MeshBoarder = value; }
        }
        public UIBoarder outerUVBoarder
        {
            get { return m_OuterUVBoarder; }
            set { m_OuterUVBoarder = value; }
        }
        public UIBoarder innerUVBoarder
        {
            get { return m_InnerUVBoarder; }
            set { m_InnerUVBoarder = value; }
        }
        public Texture texture
        {
            get { return m_Texture; }
            set { m_Texture = value; }
        }
        public Shader shader
        {
            get { return m_Shader; }
            set { m_Shader = value; }
        }
        public Color color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }
        
    }
}