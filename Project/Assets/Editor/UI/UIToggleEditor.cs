using UnityEngine;
using UnityEditor;
using System.Collections;

#region CHANGE LOG
/* November,14,2014 - Nathan Hanlan, Adding support for UILabel as well as additional error handling
 * 
 */
#endregion

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
                case UIType.LABEL:
                    DrawUILabel();
                    break;
                case UIType.BUTTON:
                    DrawUIButton();
                    break;

            }
            if(GUI.changed)
            {
                EditorUtility.SetDirty(inspected);
            }
        }

        /// <summary>
        /// Draw the UI Image GUI on the UI Toggle Editor Menu
        /// </summary>
        private void DrawUIImage()
        {
            UIImage image = inspected.GetComponentInChildren<UIImage>();
            if (image != null)
            {
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
                if (GUI.changed)
                {
                    image.GenerateMesh();
                    image.SetColor();
                    image.SetTexture();
                    BoxCollider boxCollider = inspected.GetComponent<BoxCollider>();
                    if (boxCollider == null && (inspected.receivesActionEvents == true || inspected.selectable == true))
                    {
                        boxCollider = inspected.gameObject.AddComponent<BoxCollider>();
                    }
                    else if (boxCollider != null && (inspected.receivesActionEvents == false || inspected.selectable == false))
                    {
                        DestroyImmediate(boxCollider);
                    }
                    if (boxCollider != null)
                    {
                        boxCollider.isTrigger = true;
                        boxCollider.size = new Vector3(image.width, image.height, 0.1f);
                    }
                    EditorUtility.SetDirty(image);
                }
            }
        }
        /// <summary>
        /// Draw the UI Label GUI on the UI Toggle Editor Menu
        /// </summary>
        private void DrawUILabel()
        {
            UILabel label = inspected.GetComponentInChildren<UILabel>();
            if(label != null)
            {
                label.text = EditorGUILayout.TextField(UIEditor.TEXT, label.text);
                label.fontSize = EditorGUILayout.IntField(UIEditor.FONT_SIZE, label.fontSize);
                label.fontTexture = EditorUtilities.textureField(UIEditor.TEXTURE, label.fontTexture);
                label.font = EditorUtilities.fontField(UIEditor.FONT, label.font);
                label.color = EditorGUILayout.ColorField(UIEditor.COLOR, label.color);
                if(GUI.changed)
                {
                    label.UpdateComponents();
                    BoxCollider boxCollider = inspected.GetComponent<BoxCollider>();
                    if(boxCollider == null && (inspected.receivesActionEvents == true || inspected.selectable == true))
                    {
                        boxCollider = inspected.gameObject.AddComponent<BoxCollider>();
                    }
                    else if (boxCollider != null && (inspected.receivesActionEvents == false || inspected.selectable == false))
                    {
                        DestroyImmediate(boxCollider);
                    }

                    if(boxCollider != null)
                    {
                        boxCollider.isTrigger = true;
                        label.UpdateBounds(boxCollider);
                    }
                    EditorUtility.SetDirty(label);
                }
            }
        }

        private void DrawUIButton()
        {
            UIButton button = inspected.GetComponentInChildren<UIButton>();
            if(button != null)
            {
                bool enabled = button.buttonState != UIButtonState.DISABLED;
                enabled = EditorGUILayout.Toggle(UIEditor.BUTTON_STATE, enabled);
                if(enabled == true)
                {
                    button.Enable();
                }
                else
                {
                    button.Disable();
                }
                button.disabledTexture = EditorUtilities.textureField(UIEditor.DISABLED, button.disabledTexture);
                button.normalTexture = EditorUtilities.textureField(UIEditor.NORMAL, button.normalTexture);
                button.hoverTexture = EditorUtilities.textureField(UIEditor.HOVER, button.hoverTexture);
                button.downTexture = EditorUtilities.textureField(UIEditor.DOWN, button.downTexture);
                button.enabledTextColor = EditorGUILayout.ColorField(UIEditor.ENABLED_TEXT_COLOR, button.enabledTextColor);
                button.disabledTextColor = EditorGUILayout.ColorField(UIEditor.DISABLED_TEXT_COLOR, button.disabledTextColor);
                button.eventListener = EditorUtilities.ObjectField<UIEventListener>(UIEditor.UI_EVENT_LISTENER, button.eventListener);

                UILabel label = button.GetComponentInChildren<UILabel>();
                if (label != null)
                {
                    label.text = EditorGUILayout.TextField(UIEditor.TEXT, label.text);
                    label.fontSize = EditorGUILayout.IntField(UIEditor.FONT_SIZE, label.fontSize);
                    label.fontTexture = EditorUtilities.textureField(UIEditor.TEXTURE, label.fontTexture);
                    label.font = EditorUtilities.fontField(UIEditor.FONT, label.font);
                    label.color = EditorGUILayout.ColorField(UIEditor.COLOR, label.color);
                }
                UIImage image = button.GetComponentInChildren<UIImage>();
                if(image != null)
                {
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
                }

                if(GUI.changed)
                {

                    button.UpdateComponents();
                    label.UpdateComponents();
                    


                    BoxCollider boxCollider = inspected.GetComponent<BoxCollider>();
                    if (boxCollider == null && (inspected.receivesActionEvents == true || inspected.selectable == true))
                    {
                        boxCollider = inspected.gameObject.AddComponent<BoxCollider>();
                    }
                    else if (boxCollider != null && (inspected.receivesActionEvents == false || inspected.selectable == false))
                    {
                        DestroyImmediate(boxCollider);
                    }

                    if (boxCollider != null)
                    {
                        boxCollider.isTrigger = true;
                        label.UpdateBounds(boxCollider);
                        image.width = boxCollider.size.x;
                        image.height = boxCollider.size.y;
                    }
                    
                    image.GenerateMesh();
                    image.SetColor();
                    image.SetTexture();
                    EditorUtility.SetDirty(image);
                    EditorUtility.SetDirty(button);
                    EditorUtility.SetDirty(label);
                }
            }
        }
        public UIToggle inspected
        {
            get { return m_Inspected.targetObject as UIToggle; }
        }
    }
}