using UnityEngine;
using System.Collections;
using EndevGame;

#region
/* October,6,2014 - Nathan Hanlan - Added additional regions and comments.
* 
*/
#endregion
/// <summary>
/// The base component of all future CharacterComponents. See the Character TDD section for more information.
/// </summary>
public class CharacterComponent : EndevBehaviour 
{
    private const string CHARACTER_MANAGER_NOT_FOUND = "Missing \'Character Manager\' on ";
    private void missingCharacterManager()
    {
#if UNITY_EDITOR
        Debug.LogError(CHARACTER_MANAGER_NOT_FOUND + gameObject.name);
#endif
    }
    /// <summary>
    /// The reference to the character manager this component belongs to.
    /// </summary>
    private CharacterManager m_CharacterManager = null;

    /// <summary>
    /// A helper function which initializes and gets the manager. (It searches the sibling components first then parent components after that if it fails to find it.)
    /// </summary>
    protected virtual void init()
    {
        m_CharacterManager = GetComponent<CharacterManager>();
        if(m_CharacterManager == null)
        {
            m_CharacterManager = getComponentInParent<CharacterManager>();
        }
#if UNITY_EDITOR
        if (m_CharacterManager == null)
        {
            missingCharacterManager();
        }
#endif
    }

    //Invokes the EndevBehaviour Update to get SlowUpdate function calls
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// The manager of the character component. Use this to get to the root of the character.
    /// </summary>
    public CharacterManager manager
    {
        get { if (m_CharacterManager == null) { missingCharacterManager(); } return m_CharacterManager; }
        protected set { m_CharacterManager = value; }
    }
    #region SiblingComponents
    /// <summary>
    /// The character animation component that belongs to the manager.
    /// </summary>
    public CharacterAnimation characterAnimation
    {
        get { return manager == null ? null : manager.characterAnimation; }
    }
    /// <summary>
    /// The character motor component that belongs to the manager.
    /// </summary>
    public CharacterMotor characterMotor
    {
        get { return manager == null ? null : manager.characterMotor; }
    }
    /// <summary>
    /// the character interaction component that belongs to the manager.
    /// </summary>
    public CharacterInteraction characterInteraction
    {
        get { return manager == null ? null : manager.characterInteraction; }
    }
    /// <summary>
    /// The character leddge grab component that belongs to the manager.
    /// </summary>
    public CharacterLedgeGrab characterLedgeGrab
    {
        get { return manager == null ? null : manager.characterLedgeGrab; }
    }
    /// <summary>
    /// The character climbing component that belongs to the manager.
    /// </summary>
    public CharacterClimbing characterClimbing
    {
        get { return manager == null ? null : manager.characterClimbing; }
    }
    #endregion

    #region InputProperties
    public float forwardMotion
    {
        get { return manager == null ? 0.0f : manager.forwardMotion; }
    }
    public float sideMotion
    {
        get { return manager == null ? 0.0f : manager.sideMotion; }
    }
    public float fixedSideMotion
    {
        get { return manager == null ? 0.0f : manager.fixedSideMotion; }
    }
    public bool jump
    {
        get { return manager == null ? false : manager.jump; }
    }
    public bool crouch
    {
        get { return manager == null ? false : manager.crouch; }
    }
    public bool action
    {
        get { return manager == null ? false : manager.action; }
    }
    public bool grow
    {
        get { return manager == null ? false : manager.grow; }
    }
    public bool shrink
    {
        get { return manager == null ? false : manager.shoot; }
    }
    public bool shoot
    {
        get { return manager == null ? false : manager.shoot; }
    }
    public bool sprint
    {
        get { return manager == null ? false : manager.sprint; }
    }
    public float projectileType
    {
        get { return manager == null ? 0.0f : manager.projectileType; }
    }
    public bool shootMode
    {
        get { return manager == null ? false : manager.shootMode; }
    }
    #endregion


    /// <summary>
    /// The camera used by the Character. (This is managed by CameraManager)
    /// </summary>
    public Camera characterCamera
    {
        get { return manager == null ? null : manager.characterCamera; }
    }
    /// <summary>
    /// The orbit camera used by the Character. (This is managed by CameraManager)
    /// </summary>
    public OrbitCamera orbitCamera
    {
        get { return manager == null ? null : manager.orbitCamera; }
    }
    /// <summary>
    /// The camera target the camera controller will use. 
    /// </summary>
    public Transform cameraTarget
    {
        get { return manager == null ? null : manager.cameraTarget; }
        set { if (manager != null) { manager.cameraTarget = value; } }
    }
    #region State Variables
    /// <summary>
    /// Returns true if the current camera is focused on the player.
    /// </summary>
    public bool ownsCamera
    {
        get { return manager == null ? false : manager.ownsCamera; }
    }
    /// <summary>
    /// Lock the players movement preventing them to move.
    /// </summary>
    public bool lockMovement
    {
        get { return manager == null ? false : manager.lockMovement; }
        set { if (manager != null) {manager.lockMovement = value;}}
    }
    /// <summary>
    /// Lock the players rotation preventing them from rotating using the CharacterMotor.
    /// </summary>
    public bool lockRotation
    {
        get { return manager == null ? false : manager.lockRotation; }
        set { if (manager != null) {manager.lockRotation = value;}}
    }
    /// <summary>
    /// Lock the players gravity forcing gravity to not be applied.
    /// </summary>
    public bool lockGravity
    {
        get { return manager == null ? false : manager.lockGravity; }
        set { if (manager != null) { manager.lockGravity = value; } }
    }
    #endregion
}
