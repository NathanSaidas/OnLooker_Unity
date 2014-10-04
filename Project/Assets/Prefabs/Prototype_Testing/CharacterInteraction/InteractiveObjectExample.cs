using UnityEngine;
using System.Collections;

namespace EndevGame
{

    public class InteractiveObjectExample : MonoBehaviour
    {
        private Interactive m_InteractiveObject = null;

        [SerializeField]
        private Color m_FirstColor = Color.red;
        [SerializeField]
        private Color m_SecondColor = Color.blue;
        /// <summary>
        /// true - -respond to use, false respond to focus
        /// </summary>
        [SerializeField]
        private bool m_RespondToUse = false;

        /// <summary>
        /// The model component to search for mesh renderes
        /// </summary>
        [SerializeField]
        private Transform m_ModelComponent = null;
        //The default material
        [SerializeField]
        private Material m_Material = null;

        private bool m_Using = false;
        private bool m_Focusing = false;

        void Start()
        {
            //Get the component and register the event listener.
            m_InteractiveObject = GetComponent<Interactive>();
            if(m_InteractiveObject == null)
            {
                m_InteractiveObject = GetComponentInChildren<Interactive>();
            }
            if (m_InteractiveObject != null)
            {
                m_InteractiveObject.register(onInteractiveCallback);
            }
            //Create a material
            m_Material = new Material(m_Material);

            //Grab all the mesh renderes in the child and set their material
            if (m_ModelComponent != null)
            {
                MeshRenderer[] meshRenderers = m_ModelComponent.GetComponentsInChildren<MeshRenderer>();

                if(meshRenderers == null)
                {
                    return;
                }
                
                IEnumerator iterator = meshRenderers.GetEnumerator();
                while(iterator.MoveNext())
                {
                    (iterator.Current as MeshRenderer).material = m_Material;
                }
            }            

        }

        void Destroy()
        {
            //Unregister the event listener.
            if(m_InteractiveObject != null)
            {
                m_InteractiveObject.unregister(onInteractiveCallback);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(m_Material == null)
            {
                return;
            }


            Color color = m_Material.color;

            if(m_RespondToUse == true)
            {
                if(m_Using == true)
                {
                    color = Color.Lerp(color, m_SecondColor, Time.deltaTime * 5.0f);
                }
                else
                {
                    color = Color.Lerp(color, m_FirstColor, Time.deltaTime * 5.0f);
                }
            }
            else
            {
                if (m_Focusing == true)
                {
                    color = Color.Lerp(color, m_SecondColor, Time.deltaTime * 5.0f);
                }
                else
                {
                    color = Color.Lerp(color, m_FirstColor, Time.deltaTime * 5.0f);
                }
            }

            m_Material.color = color;
        }

        void onInteractiveCallback(Interactive aSender, InteractiveArgs aArgs)
        {
            if(aSender == null )
            {
                return;
            }
            if(aArgs.message == "Use")
            {
                m_Using = true;
            }
            else if(aArgs.message == "StopUsing")
            {
                m_Using = false;
            }
            else if(aArgs.message == "FocusBegin")
            {
                m_Focusing = true;
            }
            else if(aArgs.message == "FocusEnd")
            {
                m_Focusing = false;
            }
        }
    }
}