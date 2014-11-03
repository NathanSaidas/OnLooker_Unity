using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Gem
{


    [CustomEditor(typeof(UIToggle))]
    public class UIToggleEditor : Editor
    {
        private SerializedObject m_Inspected = null;
        
        private void OnEnable()
        {
            m_Inspected = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if(inspected == null)
            {
                return;
            }

            switch(inspected.uiType)
            {
                case UIType.IMAGE:
                    DrawUIImage();
                    break;
            }
            if(GUI.changed)
            {
                EditorUtility.SetDirty(inspected);
            }
        }


        private void DrawUIImage()
        {
            UIImage image = inspected.GetComponentInChildren<UIImage>();
            EditorGUILayout.BeginHorizontal();
            image.width = EditorGUILayout.FloatField(UIEditor.WIDTH, image.width);
            image.height = EditorGUILayout.FloatField(UIEditor.HEIGHT, image.height);
            EditorGUILayout.EndHorizontal();
            image.meshBoarder = EditorUtilities.UIBoarderField(UIEditor.MESH_BOARDER, image.meshBoarder);
            image.outerUVBoarder = EditorUtilities.UIBoarderField(UIEditor.OUTER_UV_BOARDER, image.outerUVBoarder);
            image.innerUVBoarder = EditorUtilities.UIBoarderField(UIEditor.INNER_UV_BOARDER, image.innerUVBoarder);
            image.texture = EditorUtilities.ObjectField<Texture>(UIEditor.TEXTURE, image.texture);
            image.shader = EditorUtilities.ObjectField<Shader>(UIEditor.SHADER, image.shader);
            image.color = EditorGUILayout.ColorField(UIEditor.COLOR, image.color);
            if(GUI.changed)
            {
                image.GenerateMesh();
                image.SetColor();
                image.SetTexture();
                BoxCollider boxCollider = inspected.GetComponent<BoxCollider>();
                if(boxCollider != null)
                {
                    boxCollider.isTrigger = true;
                    boxCollider.size = new Vector3(image.width, image.height, 0.1f);
                }
                EditorUtility.SetDirty(image);
            }
        }

        public UIToggle inspected
        {
            get { return target as UIToggle; }
        }
    }
}