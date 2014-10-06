using UnityEngine;
using System.Collections;

namespace EndevGame
{
    [RequireComponent(typeof(InteractivePlant))]
    public class VineBehavior : PlantComponent
    {
        private CharacterManager m_PlayerManager = null;
        private InteractivePlant m_InteractivePlant = null;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            m_InteractivePlant = GetComponent<InteractivePlant>();

            if (m_InteractivePlant == null)
            {
                Debug.Log("Interactive Plant component not found");
                return;
            }

            // Register the onInteractiveEvent function with the InteractivePlant component attached to this object
            m_InteractivePlant.register(onInteractiveEvent);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void onInteractiveEvent(Interactive aSender, InteractiveArgs aArgs)
        {
            //Invalid player
            if(aArgs.triggeringPlayer == null)
            {
                return;
            }

            if (aArgs.message == "onUse")
            {
                //Removed saving reference of the triggering player from here. ~Nathan
                onUse(aArgs);
            }
            else if (aArgs.message == "onUseEnd")
            {
                onUseEnd();
            }
        }

        /// <summary>
        /// Added InteractiveArgs as a parameter. ~Nathan
        /// </summary>
        private void onUse(InteractiveArgs aArgs)
        {
            


            if (m_PlayerManager == null)
            {
                Debug.Log("Character manager not found in: " + this.name);
                return;
            }

            //m_PlayerManager.characterRigidmovement.ClimbKickOffDirection = transform.forward * -1;
            //m_PlayerManager.pushActionState(ActionState.CLIMB_BEGIN);

        }

        private void onUseEnd()
        {
            Debug.Log("onUseEnd reached in " + this.name);
            //m_PlayerManager.pushActionState(ActionState.CLIMB_BEGIN);

            m_PlayerManager = null;
        }

        void OnDrawGizmos()
        {
            Vector3 pos = transform.position + transform.rotation * (Vector3.forward * (1.0f));

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, pos);
        }
    }
}