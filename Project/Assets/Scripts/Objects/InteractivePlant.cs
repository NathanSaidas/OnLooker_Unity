using UnityEngine;
using System.Collections;

namespace EndevGame
{
    
    public class InteractivePlant : Interactive
    {
        //A reference to the manager
        private PlantManager m_Manager;
        //The minimum distance the player must be to use this object
        [SerializeField]
        private float m_MinDistance = 5.0f;

        private void Start()
        {
            m_Manager = GetComponentInParent<PlantManager>();
            //m_Manager.plantInteractionComponent = this;
        }

        //Gets called when the player enters the plant trigger area
        public override void onPlayerEnter(CharacterInteraction aPlayer)
        {
            
        }
        //Gets called when the player stays within the plant trigger area
        public override void onPlayerStay(CharacterInteraction aPlayer)
        {

        }
        //Gets called when the player leaves the plant trigger area
        public override void onPlayerExit(CharacterInteraction aPlayer)
        {

        }
        //Gets called when the player looks at an object with a collider who has their layer set to "Object Interaction" ie 8th Layer
        public override void onPlayerFocusEnter(CharacterInteraction aPlayer)
        {

        }
        //Gets called for every frame the player looks at an object with a collider who has their layer set to "Object Interaction" ie 8th Layer
        public override void onPlayerFocus(CharacterInteraction aPlayer)
        {

        }
        //Gets called when the player stops looking at the object with a collider who has their layer set to "Object Interaction" ie 8th Layer
        public override void onPlayerFocusExit(CharacterInteraction aPlayer)
        {
            //Get the character manager to see what were dealing with here
            CharacterManager playerManager = aPlayer.GetComponent<CharacterManager>();
            if(playerManager != null)
            {
                //If the player is using the plant (In a plant action state) then force the player to stop using the plant and unlock their movement because they are no longer looking at the plant. 
                //switch(playerManager.actionState)
                //{
                //    case ActionState.PLANT_SHRINK_LOOP:
                //    case ActionState.PLANT_SHRINK_END:
                //    case ActionState.PLANT_SHRINK_BEGIN:
                //    case ActionState.PLANT_GROW_LOOP:
                //    case ActionState.PLANT_GROW_END:
                //    case ActionState.PLANT_GROW_BEGIN:
                //        playerManager.pushActionState(ActionState.NONE);
                //        playerManager.lockMovement = false;
                //        break;
                //    default:
                //
                //        break;
                //}
            }
        }
        //Gets called when the player starts using this object.
        public override void onUse(CharacterInteraction aPlayer)
        {
            //Debug.Log(aPlayer.name + " requested use");
            aPlayer.manager.lockMovement = true;
            aPlayer.manager.rigidbody.velocity = new Vector3(0.0f, aPlayer.manager.rigidbody.velocity.y, 0.0f);
            invokeCallback(new InteractiveArgs("onUse", aPlayer));
        }
        //Gets called when the player stops using this object
        public override void onUseEnd(CharacterInteraction aPlayer)
        {
            //Debug.Log(aPlayer.name + " requested stop using");
            aPlayer.manager.lockMovement = false;
            invokeCallback(new InteractiveArgs("onUseEnd", aPlayer));
        }
        //The condition to check before the player may use this object.
        public override bool condition(Transform aPlayer, Transform aPlant)
        {
            //If the plant is a seed ignore the on use condition and tell the player they cannot use this plant
            if(manager != null)
            {
                PlantGrowth plantGrowth = manager.plantGrowthComponent;
                if(plantGrowth != null && plantGrowth.plantStage == PlantStage.SEED)
                {
                    return false;
                }
            }
            //This is an example condition which checks if the player is facing the plant
            //And is within the minimum distance from the target

            //facing > 0 = facing forward
            //facing < 0 = facing backward
            float distance = Vector3.Distance(aPlant.position, aPlayer.position);
            Vector3 direction = (aPlant.position - aPlayer.position).normalized;
            float facing = Vector3.Dot(direction, aPlayer.forward);

            if (distance <= m_MinDistance && facing > 0.0f)
            {
                return true;
            }
            return false;
        }


        //A reference to the plant manager.
        public PlantManager manager
        {
            get { return m_Manager; }
        }
    }
}