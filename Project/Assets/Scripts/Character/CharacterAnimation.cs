using UnityEngine;
using System.Collections;

namespace EndevGame
{
    public enum CharacterAnimationState
    {
        NONE,
        CLIMBING,
        CLIMBING_LEDGE,
        PUSH_PULL,
        CHARACTER_MOTOR,
    }
    public class CharacterAnimation : CharacterComponent
    {
        //3
        public const string ANIMATION_IDLE = "idle";
        public const string ANIMATION_CROUCH_IDLE = "crouchIdle";
        public const string ANIMATION_CLIMB_IDLE = "climbIdle";

        ///4
        public const string ANIMATION_WALK_FORWARD = "walkForward";
        public const string ANIMATION_WALK_FORWARD_CROUCH = "walkForwardCrouch";
        public const string ANIMATION_WALK_BACKWARD = "walkBackward";
        public const string ANIMATION_WALK_BACKWARD_CROUCH = "walkBackwardCrouch";
        //3
        public const string ANIMATION_RUN_FORWARD = "runForward";
        public const string ANIMATION_RUN_BACKWARD = "runBackward";
        public const string ANIMATION_SPRINT_FORWARD = "sprintForward";
        //3
        public const string ANIMATION_JUMP = "jump";
        public const string ANIMATION_FALL = "fall";
        public const string ANIMATION_LAND = "land";
        //4
        public const string ANIMATION_CLIMB_LEFT = "climbLeft";
        public const string ANIMATION_CLIMB_RIGHT = "climbRight";
        public const string ANIMATION_CLIMB_UP = "climbUp";
        public const string ANIMATION_CLIMB_DOWN = "climbDown";
        //2
        public const string ANIMATION_PUSH = "push";
        public const string ANIMATION_PULL = "pull";

        /// <summary>
        /// The animation component within the character
        /// </summary>
#if UNITY_EDITOR
        [SerializeField]
#endif
        private Animation m_Animation = null;

        #region BodyPartAttachmentPoints
        [SerializeField]
        private Transform m_Head = null;
        [SerializeField]
        private Transform m_LeftHand = null;
        [SerializeField]
        private Transform m_RightHand = null;
        [SerializeField]
        private Transform m_LeftFoot = null;
        [SerializeField]
        private Transform m_RightFoot = null;
        [SerializeField]
        private Transform m_Origin = null;
        #endregion

        [SerializeField]
        private CharacterAnimationState m_CurrentState = CharacterAnimationState.CHARACTER_MOTOR;

        [SerializeField]
        private float m_RunVelocity = 2.0f;
        [SerializeField]
        private float m_JumpTimer = 1.0f;
        [SerializeField]
        private float m_LandTimer = 1.0f;

        private float m_CurrentJumpTime = 0.0f;
        private float m_CurrentLandTime = 0.0f;

        [SerializeField]
        private AnimationClip[] m_AnimationClips;
        // Use this for initialization
        void Start()
        {
            init();
            m_Animation = GetComponent<Animation>();
            if(m_Animation == null)
            {
                m_Animation = GetComponentInChildren<Animation>();
            }

            if(m_AnimationClips != null && m_Animation != null)
            {
                for(int i = 0; i < m_AnimationClips.Length; i++)
                {
                    if(m_AnimationClips[i] == null || m_AnimationClips[i].animationClip == null)
                    {
                        continue;
                    }
                    m_AnimationClips[i].animationClip.wrapMode = m_AnimationClips[i].wrapMode;
                    if(m_AnimationClips[i].animationClip.wrapMode == WrapMode.Clamp)
                    {
                        m_AnimationClips[i].animationClip.wrapMode = WrapMode.Once;
                    }
                    if(m_AnimationClips[i].name == "jump")
                    {
                        m_JumpTimer = m_AnimationClips[i].animationClip.length * 0.45f;
                    }
                    else if(m_AnimationClips[i].name == "land")
                    {
                        m_LandTimer = m_AnimationClips[i].animationClip.length * 0.45f;
                    }

                    m_Animation.AddClip(m_AnimationClips[i].animationClip, m_AnimationClips[i].name);
                }
            }

        }

        private void attachHead(Transform aAttachee)
        {
            if(aAttachee != null)
            {
                aAttachee.parent = m_Head;
            }
        }
        private void attachLeftHand(Transform aAttachee)
        {
            if (aAttachee != null)
            {
                aAttachee.parent = m_LeftHand;
            }
        }
        private void attachRightHand(Transform aAttachee)
        {
            if (aAttachee != null)
            {
                aAttachee.parent = m_RightHand;
            }
        }
        private void attachLeftFoot(Transform aAttachee)
        {
            if (aAttachee != null)
            {
                aAttachee.parent = m_LeftFoot;
            }
        }
        private void attachRightFoot(Transform aAttachee)
        {
            if (aAttachee != null)
            {
                aAttachee.parent = m_RightFoot;
            }
        }
        private void attachOrigin(Transform aAttachee)
        {
            if (aAttachee != null)
            {
                aAttachee.parent = m_Origin;
            }
        }

        /// <summary>
        /// Use this method to set the animation state of the character. The state must be in the motor state to use.
        /// </summary>
        /// <param name="aState"></param>
        public void setState(CharacterAnimationState aState)
        {
            if(m_CurrentState == aState)
            {
                return;
            }
            if(m_CurrentState != CharacterAnimationState.CHARACTER_MOTOR && aState != CharacterAnimationState.CHARACTER_MOTOR)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Attempting to enter a new animation state " + aState + " however the current state " + m_CurrentState + " has not been released yet.");
#endif
                return;
            }
            m_CurrentState = aState;
        }
        /// <summary>
        /// Release the state back to the CharacterMotor. Use this method release your state once your done taking control of the character.
        /// </summary>
        /// <param name="aState"></param>
        public void releaseState(CharacterAnimationState aState)
        {
            if(m_CurrentState == aState)
            {
                m_CurrentState = CharacterAnimationState.CHARACTER_MOTOR;
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            m_CurrentJumpTime -= Time.deltaTime;
            m_CurrentLandTime -= Time.deltaTime;

            switch(m_CurrentState)
            {
                case CharacterAnimationState.CHARACTER_MOTOR:
                    {
                        if (characterMotor == null)
                        {
#if UNITY_EDITOR
                            Debug.LogWarning("Missing character motor component on the character.");
#endif
                            m_CurrentState = CharacterAnimationState.NONE;
                            return;
                        }
                        characterMotor.onAnimateCharacter(this);
                    }
                    break;
                case CharacterAnimationState.CLIMBING:
                    {
                        if(characterClimbing == null)
                        {
#if UNITY_EDITOR
                            Debug.LogWarning("Missing character climbing component on the character.");
#endif
                            releaseState(CharacterAnimationState.CLIMBING);
                            return;
                        }
                        characterClimbing.onAnimateCharacter(this);

                    }
                    break;
                case CharacterAnimationState.CLIMBING_LEDGE:
                    {
                        if (characterLedgeGrab == null)
                        {
#if UNITY_EDITOR
                            Debug.LogWarning("Missing character ledge grab component on the character.");
#endif
                            releaseState(CharacterAnimationState.CLIMBING_LEDGE);
                            return;
                        }
                        characterLedgeGrab.onAnimateCharacter(this);
                    }
                    break;
                case CharacterAnimationState.PUSH_PULL:
                    {
                        //TODO: Add and implement a push pull component that enables the character to push and pull objects in the world.
                    }
                    break;
                default:

                    break;
            }

        }
        /// <summary>
        /// Resets the jump timer
        /// </summary>
        public void beginJump()
        {
            m_CurrentJumpTime = m_JumpTimer;
        }
        /// <summary>
        /// Resets the landing timer
        /// </summary>
        public void beginLand()
        {
            m_CurrentLandTime = m_LandTimer;
        }
        /// <summary>
        /// The velocity required to run.
        /// </summary>
        public float runVelocity
        {
            get { return m_RunVelocity; }
            set { m_RunVelocity = value; }
        }
        /// <summary>
        /// The total time of a jump
        /// </summary>
        public float jumpTimer
        {
            get { return m_JumpTimer; }
        }
        public float landTimer
        {
            get { return m_LandTimer; }
        }
        public bool animateJump
        {
            get { return m_CurrentJumpTime > 0.0f; }
        }
        public bool animateLand
        {
            get { return m_CurrentLandTime > 0.0f; }
        }

        public Animation animationComponent
        {
            get { return m_Animation; }
        }
    }
}