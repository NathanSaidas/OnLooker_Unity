using UnityEngine;
using System.Collections;
using EndevGame;

public class CharacterComponent : EndevBehaviour 
{
    private CharacterManager m_CharacterManager = null;

    protected virtual void init()
    {
        m_CharacterManager = GetComponent<CharacterManager>();
        if(m_CharacterManager == null)
        {
            m_CharacterManager = getComponentInParent<CharacterManager>();
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    public CharacterManager manager
    {
        get { if (m_CharacterManager == null) { Debug.LogError("Missing \'Character Manager\' on " + gameObject.name + "."); } return m_CharacterManager; }
        protected set { m_CharacterManager = value; }
    }

    public CharacterAnimation characterAnimation
    {
        get { return manager == null ? null : manager.characterAnimation; }
    }
    public CharacterMotor characterMotor
    {
        get { return manager == null ? null : manager.characterMotor; }
    }
    public CharacterInteraction characterInteraction
    {
        get { return manager == null ? null : manager.characterInteraction; }
    }

    #region InputProperties
    public float forwardMotion
    {
        get { return manager == null ? 0.0f : manager.forwardMotion; }
    }
    public float sideMotion
    {
        get { return manager == null ? 0.0f : manager.sideMotion; }
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


    public Camera characterCamera
    {
        get { return manager == null ? null : manager.characterCamera; }
    }
    public OrbitCamera orbitCamera
    {
        get { return manager == null ? null : manager.orbitCamera; }
    }
    public Transform cameraTarget
    {
        get { return manager == null ? null : manager.cameraTarget; }
        set { if (manager != null) { manager.cameraTarget = value; } }
    }
    public bool ownsCamera
    {
        get { return manager == null ? false : manager.ownsCamera; }
    }
    public bool lockMovement
    {
        get { return manager == null ? false : manager.lockMovement; }
        set { if (manager != null) {manager.lockMovement = value;}}
    }
    public bool lockRotation
    {
        get { return manager == null ? false : manager.lockRotation; }
        set { if (manager != null) {manager.lockRotation = value;}}
    }
    public bool lockGravity
    {
        get { return manager == null ? false : manager.lockGravity; }
        set { if (manager != null) { manager.lockGravity = value; } }
    }
}
