using UnityEngine;
using System.Collections.Generic;

#region CHANGE LOG
/* November,13,2014 - Nathan Hanlan, Added support for a new shader for text only.
 * November,13,2014 - Nathan Hanlan, Added support for creating UILabel prefabs.
 * November,13,2014 - Nathan Hanlan, Added constants for shader string names.
 * November,14,2014 - Nathan Hanlan, Added support for creating UI Buttons
 */
#endregion

namespace Gem
{


    public class UIUtilities : MonoBehaviour
    {
        /// Shader Names

        public const string SHADER_UNLIT = "Custom/UI/UI_Unlit";
        public const string SHADER_UNLIT_TRANSPARENT = "Custom/UI/UI_Unlit_Transparent";
        public const string SHADER_DIFFUSE = "Custom/UI/UI_Diffuse";
        public const string SHADER_DIFFUSE_TRANSPARENT = "Custom/UI/UI_Diffuse_Transparent";
        public const string SHADER_BUMPED_DIFFUSE = "Custom/UI/UI_Bumped_Diffuse";
        public const string SHADER_BUMPED_DIFFUSE_TRANSPARENT = "Custom/UI/UI_Bumped_Diffuse_Transparent";
        public const string SHADER_TEXT = "Custom/UI/UI_Text";


        /// Shader Keywords

        public const string SHADER_TEXTURE = "_Texture";
        public const string SHADER_COLOR = "_Color";
        public const string SHADER_TILE_X = "_TileX";
        public const string SHADER_TILE_Y = "_TileY";

        /// <summary>
        /// Default name of the meshes
        /// </summary>
        public const string MESH_NAME = "M_UIImage";

        public static Mesh GenerateUniformPlane(string aName, float aWidth, float aHeight)
        {
            return GenerateUniformPlane(aName, aWidth, aHeight, new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f));
        }

        public static Mesh GenerateUniformPlane(string aName, float aWidth, float aHeight, UIBoarder aTextureBoarder)
        {
            return GenerateUniformPlane(aName, aWidth, aHeight, aTextureBoarder, new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f), new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f));
        }

        public static Mesh GenerateUniformPlane(string aName, float aWidth, float aHeight, UIBoarder aMeshBoarder, UIBoarder aOutterUV, UIBoarder aInnerUV)
        {
            Mesh mesh = new Mesh();
            mesh.name = aName;
            return GenerateUniformPlane(mesh, aWidth, aHeight, aMeshBoarder, aOutterUV, aInnerUV);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aName">The name of the mesh</param>
        /// <param name="aWidth">The overall width of the plane</param>
        /// <param name="aHeight">The overall height of the plane</param>
        /// <param name="aMeshBoarder">The inner boarder percent of the plane</param>
        /// <param name="aOutterUV">The outter UV percentages</param>
        /// <param name="aInnerUV"></param>
        /// <returns></returns>
        public static Mesh GenerateUniformPlane(Mesh aMesh, float aWidth, float aHeight, UIBoarder aMeshBoarder, UIBoarder aOutterUV, UIBoarder aInnerUV)
        {
            if (aMesh == null)
            {
                return null;
            }
            ///Outer edges
            UIBoarder meshInnerBoarder = new UIBoarder(-aWidth * 0.5f, aWidth * 0.5f, aHeight * 0.5f, -aHeight * 0.5f);
            ///Inner edges
            UIBoarder meshOutterBoarder = new UIBoarder(
                meshInnerBoarder.left + aWidth * aMeshBoarder.left,
                meshInnerBoarder.right - aWidth * (1.0f - aMeshBoarder.right),
                meshInnerBoarder.top - aHeight * (1.0f - aMeshBoarder.top),
                meshInnerBoarder.bottom + aHeight * aMeshBoarder.bottom);

            Mesh mesh = aMesh;

            Vector3[] vertices = new Vector3[36];
            Vector2[] texCoords = new Vector2[36];
            Color[] colors = new Color[36];

            int[] indices = new int[54];


            //top left face
            vertices[0] = new Vector3(meshInnerBoarder.left, meshInnerBoarder.top, 0.0f);
            vertices[1] = new Vector3(meshOutterBoarder.left, meshInnerBoarder.top, 0.0f);
            vertices[2] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.top, 0.0f);
            vertices[3] = new Vector3(meshInnerBoarder.left, meshOutterBoarder.top, 0.0f);

            //top middle face
            vertices[4] = new Vector3(meshOutterBoarder.left, meshInnerBoarder.top, 0.0f);
            vertices[5] = new Vector3(meshOutterBoarder.right, meshInnerBoarder.top, 0.0f);
            vertices[6] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.top, 0.0f);
            vertices[7] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.top, 0.0f);

            //top right face
            vertices[8] = new Vector3(meshOutterBoarder.right, meshInnerBoarder.top, 0.0f);
            vertices[9] = new Vector3(meshInnerBoarder.right, meshInnerBoarder.top, 0.0f);
            vertices[10] = new Vector3(meshInnerBoarder.right, meshOutterBoarder.top, 0.0f);
            vertices[11] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.top, 0.0f);

            //middle left face
            vertices[12] = new Vector3(meshInnerBoarder.left, meshOutterBoarder.top, 0.0f);
            vertices[13] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.top, 0.0f);
            vertices[14] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.bottom, 0.0f);
            vertices[15] = new Vector3(meshInnerBoarder.left, meshOutterBoarder.bottom, 0.0f);

            //middle middle face
            vertices[16] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.top, 0.0f);
            vertices[17] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.top, 0.0f);
            vertices[18] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.bottom, 0.0f);
            vertices[19] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.bottom, 0.0f);

            //middle right face
            vertices[20] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.top, 0.0f);
            vertices[21] = new Vector3(meshInnerBoarder.right, meshOutterBoarder.top, 0.0f);
            vertices[22] = new Vector3(meshInnerBoarder.right, meshOutterBoarder.bottom, 0.0f);
            vertices[23] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.bottom, 0.0f);

            //bottom left face
            vertices[24] = new Vector3(meshInnerBoarder.left, meshOutterBoarder.bottom, 0.0f);
            vertices[25] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.bottom, 0.0f);
            vertices[26] = new Vector3(meshOutterBoarder.left, meshInnerBoarder.bottom, 0.0f);
            vertices[27] = new Vector3(meshInnerBoarder.left, meshInnerBoarder.bottom, 0.0f);

            //bottom middle face
            vertices[28] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.bottom, 0.0f);
            vertices[29] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.bottom, 0.0f);
            vertices[30] = new Vector3(meshOutterBoarder.right, meshInnerBoarder.bottom, 0.0f);
            vertices[31] = new Vector3(meshOutterBoarder.left, meshInnerBoarder.bottom, 0.0f);

            //bottom right face
            vertices[32] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.bottom, 0.0f);
            vertices[33] = new Vector3(meshInnerBoarder.right, meshOutterBoarder.bottom, 0.0f);
            vertices[34] = new Vector3(meshInnerBoarder.right, meshInnerBoarder.bottom, 0.0f);
            vertices[35] = new Vector3(meshOutterBoarder.right, meshInnerBoarder.bottom, 0.0f);

            //Top left UV
            texCoords[0] = new Vector2(0.0f, 1.0f);
            texCoords[1] = new Vector2(aOutterUV.left, 1.0f);
            texCoords[2] = new Vector2(aOutterUV.left, aOutterUV.top);
            texCoords[3] = new Vector2(0.0f, aOutterUV.top);

            //Top Center UV
            texCoords[4] = new Vector2(aOutterUV.left, 1.0f);
            texCoords[5] = new Vector2(aOutterUV.right, 1.0f);
            texCoords[6] = new Vector2(aOutterUV.right, aOutterUV.top);
            texCoords[7] = new Vector2(aOutterUV.left, aOutterUV.top);

            //Top Right UV
            texCoords[8] = new Vector2(aOutterUV.right, 1.0f);
            texCoords[9] = new Vector2(1.0f, 1.0f);
            texCoords[10] = new Vector2(1.0f, aOutterUV.top);
            texCoords[11] = new Vector2(aOutterUV.right, aOutterUV.top);


            //Middle Left UV
            texCoords[12] = new Vector2(0.0f, aOutterUV.top);
            texCoords[13] = new Vector2(aOutterUV.left, aOutterUV.top);
            texCoords[14] = new Vector2(aOutterUV.left, aOutterUV.bottom);
            texCoords[15] = new Vector2(0.0f, aOutterUV.bottom);

            //Middle Center UV
            texCoords[16] = new Vector2(aInnerUV.left, aInnerUV.top);
            texCoords[17] = new Vector2(aInnerUV.right, aInnerUV.top);
            texCoords[18] = new Vector2(aInnerUV.right, aInnerUV.bottom);
            texCoords[19] = new Vector2(aInnerUV.left, aInnerUV.bottom);

            //Middle Right UV
            texCoords[20] = new Vector2(aOutterUV.right, aOutterUV.top);
            texCoords[21] = new Vector2(1.0f, aOutterUV.top);
            texCoords[22] = new Vector2(1.0f, aOutterUV.bottom);
            texCoords[23] = new Vector2(aOutterUV.right, aOutterUV.bottom);

            //Bottom Left UV
            texCoords[24] = new Vector2(0.0f, aOutterUV.bottom);
            texCoords[25] = new Vector2(aOutterUV.left, aOutterUV.bottom);
            texCoords[26] = new Vector2(aOutterUV.left, 0.0f);
            texCoords[27] = new Vector2(0.0f, 0.0f);

            //Bottom Center UV
            texCoords[28] = new Vector2(aOutterUV.left, aOutterUV.bottom);
            texCoords[29] = new Vector2(aOutterUV.right, aOutterUV.bottom);
            texCoords[30] = new Vector2(aOutterUV.right, 0.0f);
            texCoords[31] = new Vector2(aOutterUV.left, 0.0f);

            //Bottom Right UV
            texCoords[32] = new Vector2(aOutterUV.right, aOutterUV.bottom);
            texCoords[33] = new Vector2(1.0f, aOutterUV.bottom);
            texCoords[34] = new Vector2(1.0f, 0.0f);
            texCoords[35] = new Vector2(aOutterUV.right, 0.0f);

            for (int i = 0; i < 36; i++)
            {
                colors[i] = Color.white;
            }

            int face = 0;
            for (int i = 0; i < 53; i += 6)
            {
                indices[i + 0] = face + 0;
                indices[i + 1] = face + 1;
                indices[i + 2] = face + 3;

                indices[i + 3] = face + 1;
                indices[i + 4] = face + 2;
                indices[i + 5] = face + 3;

                face += 4;
            }
            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.uv = texCoords;
            mesh.uv1 = texCoords;
            mesh.uv2 = texCoords;
            mesh.triangles = indices;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        /// <summary>
        /// Returns true if the string given is a valid shader name
        /// </summary>
        /// <param name="aName"></param>
        /// <returns></returns>
        public static bool IsUIShader(string aName)
        {
            switch(aName)
            {
                case SHADER_UNLIT:
                case SHADER_UNLIT_TRANSPARENT:
                case SHADER_DIFFUSE:
                case SHADER_DIFFUSE_TRANSPARENT:
                case SHADER_BUMPED_DIFFUSE:
                case SHADER_BUMPED_DIFFUSE_TRANSPARENT:
                case SHADER_TEXT:
                    return true;
                default:
                    return false;
            }
        }

        #region PREFAB

        #region PREFAB POST FIXES
        /// <summary>
        /// Postfixes.
        /// </summary>
        private const string IMAGE_POST_FIX = "_Image";
        private const string LABEL_POST_FIX = "_Label";
        private const string BUTTON_POST_FIX = "_Button";
        #endregion


        /// <summary>
        /// Creates a UI Toggle using the parent as the root
        /// </summary>
        /// <param name="aParams">The parameters for creating the toggle</param>
        /// <param name="aParent">The root of the object</param>
        /// <returns></returns>
        public static UIToggle CreateUIToggle(UIToggleParams aParams, Transform aParent)
        {
            if (aParams == null || aParent == null)
            {
                return null;
            }

            GameObject rootGameObject = new GameObject(aParams.name);
            rootGameObject.transform.position = Vector3.zero;
            rootGameObject.transform.rotation = Quaternion.identity;
            rootGameObject.transform.parent = aParent;
            UIToggle uiToggle = rootGameObject.AddComponent<UIToggle>();
            uiToggle.id = aParams.id;
            uiToggle.selectable = aParams.isSelectable;
            uiToggle.receivesActionEvents = aParams.recieveActions;
            uiToggle.uiSpace = aParams.uiSpace;
            uiToggle.uiType = aParams.uiType;
            switch (uiToggle.uiSpace)
            {
                case UISpace.TWO_DIMENSIONAL:
                    rootGameObject.layer = UIManager.UI_2D_LAYER;
                    break;
                case UISpace.THREE_DIMENSIONAL:
                    rootGameObject.layer = UIManager.UI_3D_LAYER;
                    break;
                case UISpace.WORLD:
                    rootGameObject.layer = UIManager.UI_WORLD_LAYER;
                    break;
            }
            return uiToggle;
        }
        /// <summary>
        /// Creates a UI Image prefab at using the toggle as the root
        /// </summary>
        /// <param name="aParams">The parameters for creating the toggle</param>
        /// <param name="aToggle">The root of creation</param>
        /// <returns></returns>
        public static UIImage CreateUIImage(UIImageParams aParams, UIToggle aToggle)
        {
            if (aParams == null || aToggle == null)
            {
                return null;
            }
            GameObject rootGameObject = aToggle.gameObject;

            GameObject imageGameObject = new GameObject(aParams.name + IMAGE_POST_FIX);
            imageGameObject.transform.position = Vector3.zero;
            imageGameObject.transform.rotation = Quaternion.identity;
            imageGameObject.transform.parent = rootGameObject.transform;
            imageGameObject.layer = rootGameObject.layer;
            imageGameObject.AddComponent<MeshRenderer>();
            imageGameObject.AddComponent<MeshFilter>();
            UIImage uiImage = imageGameObject.AddComponent<UIImage>();
            uiImage.meshName = MESH_NAME;
            uiImage.width = aParams.width;
            uiImage.height = aParams.height;
            uiImage.meshBoarder = aParams.meshBoarder;
            uiImage.outerUVBoarder = aParams.outerUVBoarder;
            uiImage.innerUVBoarder = aParams.innerUVBoarder;
            uiImage.texture = aParams.texture;
            uiImage.shader = aParams.shader;
            uiImage.color = aParams.color;

            uiImage.GenerateMaterial();
            uiImage.GenerateMesh();
            uiImage.SetTexture();
            uiImage.SetColor();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(uiImage);
#endif
            return uiImage;
        }

        /// <summary>
        /// Creates the UI Label prefab using the toggle as the root.
        /// </summary>
        /// <param name="aParams">The parameters for creating</param>
        /// <param name="aToggle">The toggle to create</param>
        /// <returns></returns>
        public static UILabel CreateUILabel(UILabelParams aParams, UIToggle aToggle)
        {
            if(aParams == null || aToggle == null)
            {
                return null;
            }
            GameObject rootGameObject = aToggle.gameObject;
            GameObject labelGameObject = new GameObject(aParams.name + LABEL_POST_FIX);
            labelGameObject.transform.position = Vector3.zero;
            labelGameObject.transform.rotation = Quaternion.identity;
            labelGameObject.transform.parent = rootGameObject.transform;
            labelGameObject.layer = rootGameObject.layer;
            labelGameObject.AddComponent<MeshRenderer>();
            labelGameObject.AddComponent<TextMesh>();
            UILabel uiLabel = labelGameObject.AddComponent<UILabel>();
            uiLabel.text = aParams.text;
            uiLabel.fontSize = aParams.fontSize;
            uiLabel.font = aParams.font;
            uiLabel.color = aParams.color;
            uiLabel.fontTexture = aParams.fontTexture;
            uiLabel.UpdateComponents();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(uiLabel);
#endif
            return uiLabel;
        }
        

        public static UIButton CreateUIButton(UIButtonParams aParams, UIToggle aToggle)
        {
            if(aParams == null || aToggle == null)
            {
                return null;
            }
            GameObject rootGameObject = aToggle.gameObject;
            GameObject buttonGameObject = new GameObject(aParams.name + BUTTON_POST_FIX);
            buttonGameObject.transform.position = Vector3.zero;
            buttonGameObject.transform.rotation = Quaternion.identity;
            buttonGameObject.transform.parent = rootGameObject.transform;
            buttonGameObject.layer = rootGameObject.layer;
            UIButton uiButton = buttonGameObject.AddComponent<UIButton>();
            if (aParams.disabled == true)
            {
                uiButton.Disable();
            }
            else
            {
                uiButton.Enable();
            }
            uiButton.disabledTexture = aParams.disabledTexture;
            uiButton.normalTexture = aParams.normalTexture;
            uiButton.hoverTexture = aParams.hoverTexture;
            uiButton.downTexture = aParams.downTexture;
            uiButton.enabledTextColor = aParams.enabledTextColor;
            uiButton.disabledTextColor = aParams.disabledTextColor;
            uiButton.eventListener = aParams.eventListener;

            ///create label
            GameObject labelGameObject = new GameObject(aParams.name + LABEL_POST_FIX);
            labelGameObject.transform.position = new Vector3(0.0f, 0.0f, 0.04f);
            labelGameObject.transform.rotation = Quaternion.identity;
            labelGameObject.transform.parent = buttonGameObject.transform;
            labelGameObject.layer = buttonGameObject.layer;
            labelGameObject.AddComponent<MeshRenderer>();
            labelGameObject.AddComponent<TextMesh>();
            UILabel uiLabel = labelGameObject.AddComponent<UILabel>();
            uiLabel.text = aParams.labelText;
            uiLabel.fontSize = aParams.labelFontSize;
            uiLabel.font = aParams.labelFont;
            uiLabel.color = aParams.labelColor;
            uiLabel.fontTexture = aParams.labelFontTexture;
            

            ///create image
            GameObject imageGameObject = new GameObject(aParams.name + IMAGE_POST_FIX);
            imageGameObject.transform.position = Vector3.zero;
            imageGameObject.transform.rotation = Quaternion.identity;
            imageGameObject.transform.parent = buttonGameObject.transform;
            imageGameObject.layer = buttonGameObject.layer;
            imageGameObject.AddComponent<MeshFilter>();
            imageGameObject.AddComponent<MeshRenderer>();
            UIImage uiImage = imageGameObject.AddComponent<UIImage>();
            uiImage.meshName = MESH_NAME;
            uiImage.width = aParams.imageWidth;
            uiImage.height = aParams.imageHeight;
            uiImage.meshBoarder = aParams.imageMeshBoarder;
            uiImage.outerUVBoarder = aParams.imageOuterUVBoarder;
            uiImage.innerUVBoarder = aParams.imageInnerUVBoarder;
            uiImage.texture = aParams.imageTexture;
            uiImage.shader = aParams.imageShader;
            uiImage.color = aParams.imageColor;


            uiButton.UpdateComponents();
            uiLabel.UpdateComponents();
            uiImage.GenerateMaterial();
            uiImage.GenerateMesh();
            uiImage.SetTexture();
            uiImage.SetColor();
            aToggle.eventListener = uiButton;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(uiButton);
            UnityEditor.EditorUtility.SetDirty(uiLabel);
            UnityEditor.EditorUtility.SetDirty(uiImage);
#endif

            
            return uiButton;
        }

        #endregion
    }
}