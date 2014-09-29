using UnityEngine;
using System;
using System.Collections.Generic;

public delegate void CutSceneCallback(CutsceneInstruction aCurrentInstruction);


public enum CutsceneActionMode
{
    STRAIGHT,
    BEZIER
}

[Serializable]
public class CutsceneInstruction
{

    public CutsceneInstruction(Cutscene aOwner)
    {
        //m_Action = new CutsceneStraight(Vector3.zero, Vector3.one);
        m_Owner = aOwner;
    }


    [SerializeField]
    private string m_Name = string.Empty;
    //The owner the of the ctuscene
    [SerializeField]
    private Cutscene m_Owner = null;
    //The distance required between current position and next position to move to the next point
    [SerializeField]
    private float m_DistanceClamp = 0.15f;

    [SerializeField]
    private CutsceneBezier m_BezierAction = null;
    [SerializeField]
    private CutsceneStraight m_StraightAction = null;
    [SerializeField]
    private Vector3[] m_PositionPath = null; //The positions the camera moves towards
    [SerializeField]
    private List<CutsceneLookAt> m_LookAtPath = new List<CutsceneLookAt>(); //The positoins the camera rotates toward from frame til frame.

    //The current path ie index of the position path
    [SerializeField]
    private int m_CurrentFrame = 0;

    [SerializeField]
    private float m_TotalDistance = 0.0f; //The total distance between m_Action.getPath() start and end
    [SerializeField]
    private float m_DistanceTravelled = 0.0f; //The distance travelled from start till current

    //How much time to delay on start
    [SerializeField]
    private float m_TimeDelay = 0.0f;
    //How much time has gone by during this instruction
    [SerializeField]
    private float m_CurrentTime = 0.0f;


    //Last and next position for distance calculations
    //private Vector3 m_LastPosition = Vector3.zero;
    //private Vector3 m_NextPosition = Vector3.zero;


    //The movement speed for this camera
    [SerializeField]
    private float m_MoveSpeed = 0.0f;
    //The lookat speed for this camera
    [SerializeField]
    private float m_LookSpeed = 0.0f;

    //The mode used for moving
    [SerializeField]
    private CameraMode m_MoveMode = CameraMode.EASE;

    private event CutSceneCallback m_TimeDelayFinished = null;
    private event CutSceneCallback m_FrameUpdate = null;
    private event CutSceneCallback m_GoalReached = null;

    //for the editor
    [SerializeField]
    private bool m_ShowInstruction = true;
    [SerializeField]
    private bool m_ShowLookAtPoints = true;
    [SerializeField]
    private bool m_Edit;
    [SerializeField]
    private Color m_LineColor = Color.green;
    [SerializeField]
    private Color m_ControlPointColor = Color.magenta;
    [SerializeField]
    private Color m_LookAtPointColor = Color.blue;

    [SerializeField]
    private CutsceneActionMode m_ActionMode = CutsceneActionMode.BEZIER;

    //Called every executing frame
    public void update()
    {
        //Determine if were waiting for the current time to pass the time delay
        bool onDelay = m_CurrentTime < m_TimeDelay;

        m_CurrentTime += Time.deltaTime;
        if (m_CurrentTime < m_TimeDelay)
        {
            //Cutscene on Delay
            return;
        }
        if (m_Owner == null)
        {
            Debug.LogError("\'Cutscene Instruction\' is missing a \'Cutscene\' Owner.");
        }
        else if (m_Owner.isPaused == true)
        {
            return;
        }

        if (m_PositionPath == null || m_PositionPath.Length == 0)
        {
            Debug.LogWarning("No path given to the \'Cutscene Instruction\'");
            return;
        }
        if (m_CurrentFrame >= m_PositionPath.Length)
        {
            onGoalReached();
            return;
        }


        //Calculate the target position
        if (onDelay == true)
        {
            onTimeDelayFinised();
        }

        //Valid Frame and Position path variable to work from.
        Vector3 goal = m_PositionPath[m_CurrentFrame];
        Vector3 lookAt = Vector3.zero;

        //Get the look at target
        for (int i = 0; i < m_LookAtPath.Count; i++)
        {
            if (m_CurrentFrame >= m_LookAtPath[i].startFrame && m_CurrentFrame <= m_LookAtPath[i].endFrame)
            {
                lookAt = m_LookAtPath[i].position;
                break;
            }
        }



        //Grab the affected transform
        Transform transform = CameraManager.instance.cutsceneCamera.transform;
        if (transform != null)
        {
            Vector3 begin = Vector3.zero;
            Vector3 end = Vector3.zero;
            //Get the start position
            begin = transform.position;

            //Move the transform based on Camera Mode
            switch (m_MoveMode)
            {
                case CameraMode.EASE:
                    end = Utilities.exponentialEase(transform.position, goal, m_MoveSpeed * Time.deltaTime);
                    break;
                case CameraMode.LERP:
                    end = Vector3.Lerp(transform.position, goal, m_MoveSpeed * Time.deltaTime);
                    break;
                case CameraMode.SMOOTH_DAMP:
                    {
                        Vector3 vel = new Vector3(m_MoveSpeed, m_MoveSpeed, m_MoveSpeed);
                        end = Vector3.SmoothDamp(transform.position, goal, ref vel, m_MoveSpeed * Time.deltaTime);
                    }
                    break;
                default:
                    Debug.LogWarning("Invalid Camera Mode. Camera modes accepted are \'EASE\', \'LERP\' and \'SMOOTH_DAMP\'");
                    return;
            }

            //Calculate distance for end position adjustment
            float distance = Vector3.Distance(begin, end);
            if (distance < m_DistanceClamp)
            {
                end = goal;
                onGoalReached();
                m_CurrentFrame++;
            }

            //Set the position
            m_DistanceTravelled += Vector3.Distance(begin, end);
            transform.position = end;


            Quaternion lookRotation = Quaternion.LookRotation(lookAt - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_LookSpeed * Time.deltaTime);
        }





    }



    public void reset()
    {
        switch (m_ActionMode)
        {
            case CutsceneActionMode.BEZIER:
                if (m_BezierAction != null)
                {
                    m_PositionPath = m_BezierAction.getPath();
                }
                break;
            case CutsceneActionMode.STRAIGHT:
                if (m_StraightAction != null)
                {
                    m_PositionPath = m_StraightAction.getPath();
                }
                break;
            default:
                Debug.LogWarning("Reseting the \'Cutscene Instruction\' but there is no Action Mode");
                break;
        }
        m_CurrentFrame = 0;
    }

    public void registerTimeDelayFinished(CutSceneCallback aCallback)
    {
        if (aCallback == null)
        {
            return;
        }
        m_TimeDelayFinished += aCallback;
    }
    public void unregisterTimeDelayFinished(CutSceneCallback aCallback)
    {
        if (aCallback == null)
        {
            return;
        }
        m_TimeDelayFinished -= aCallback;
    }
    public void registerFrameUpdate(CutSceneCallback aCallback)
    {
        if (aCallback == null)
        {
            return;
        }
        m_FrameUpdate += aCallback;
    }
    public void unregisterFrameUpdate(CutSceneCallback aCallback)
    {
        if (aCallback == null)
        {
            return;
        }
        m_FrameUpdate -= aCallback;
    }
    public void registerGoalReached(CutSceneCallback aCallback)
    {
        if (aCallback == null)
        {
            return;
        }
        m_GoalReached += aCallback;
    }
    public void unregisterGoalReached(CutSceneCallback aCallback)
    {
        if (aCallback == null)
        {
            return;
        }
        m_GoalReached -= aCallback;
    }

    private void onGoalReached()
    {
        if (m_GoalReached != null)
        {
            m_GoalReached.Invoke(this);
        }
    }
    private void onFrameUpdate()
    {
        if (m_FrameUpdate != null)
        {
            m_FrameUpdate.Invoke(this);
        }

    }
    private void onTimeDelayFinised()
    {
        if (m_TimeDelayFinished != null)
        {
            m_TimeDelayFinished.Invoke(this);
        }
    }
    public void addLookAtPoint()
    {
        m_LookAtPath.Add(new CutsceneLookAt());
    }
    public void clearLookAtPoints()
    {
        m_LookAtPath.Clear();
    }
    public void removeLookAtPoint(CutsceneLookAt aLookAt)
    {
        if (aLookAt != null)
        {
            m_LookAtPath.Remove(aLookAt);
        }
    }
    public Cutscene owner
    {
        get { return m_Owner; }
    }
    public Vector3 currentGoal
    {
        get { if (m_CurrentFrame >= 0 && m_CurrentFrame < m_PositionPath.Length) { return m_PositionPath[m_CurrentFrame]; } return Vector3.zero; }
    }
    public int currentFrame
    {
        get { return m_CurrentFrame; }
    }
    public float totalDistance
    {
        get { return m_TotalDistance; }
    }
    public float distanceTravelled
    {
        get { return m_DistanceTravelled; }
    }
    //public CutsceneAction action
    //{
    //    get { return m_Action; }
    //    set { m_Action = value; }
    //}
    public CutsceneBezier bezierAction
    {
        get { return m_BezierAction; }
    }
    public CutsceneStraight straightAction
    {
        get { return m_StraightAction; }
    }
    public float distanceClamp
    {
        get { return m_DistanceClamp; }
        set { m_DistanceClamp = value; }
    }
    public float timeDelay
    {
        get { return m_TimeDelay; }
        set { m_TimeDelay = value; }
    }
    public float currentTime
    {
        get { return m_CurrentTime; }
    }
    public float moveSpeed
    {
        get { return m_MoveSpeed; }
        set { m_MoveSpeed = value; }
    }
    public float lookSpeed
    {
        get { return m_LookSpeed; }
        set { m_LookSpeed = value; }
    }
    public CameraMode moveMode
    {
        get { return m_MoveMode; }
        set { m_MoveMode = value; }
    }
    public CutsceneActionMode actionMode
    {
        get { return m_ActionMode; }
    }

    public bool isFinished
    {
        get { if (m_PositionPath == null) { return true; } return m_CurrentFrame > m_PositionPath.Length; }
    }


    public string name
    {
        get { return m_Name; }
        set { m_Name = value; }
    }
    public bool edit
    {
        get { return m_Edit; }
        set { m_Edit = value; }
    }
    public bool showInstruction
    {
        get { return m_ShowInstruction; }
        set { m_ShowInstruction = value; }
    }
    public bool showLookAtPoints
    {
        get { return m_ShowLookAtPoints; }
        set { m_ShowLookAtPoints = value; }
    }
    public Color lineColor
    {
        get { return m_LineColor; }
        set { m_LineColor = value; }
    }
    public Color controlPointColor
    {
        get { return m_ControlPointColor; }
        set { m_ControlPointColor = value; }
    }
    public Color lookAtPointColor
    {
        get { return m_LookAtPointColor; }
        set { m_LookAtPointColor = value; }
    }


    public List<CutsceneLookAt> lookAtPath
    {
        get { return m_LookAtPath; }
    }
    public void setAsActionMode(CutsceneActionMode aMode)
    {
        switch (aMode)
        {
            case CutsceneActionMode.BEZIER:
                m_BezierAction = null;
                m_StraightAction = null;
                break;
            case CutsceneActionMode.STRAIGHT:
                m_StraightAction = null;
                m_BezierAction = null;
                break;
        }

        m_ActionMode = aMode;
    }

    public void gizmosDrawPath()
    {
        if (m_PositionPath != null && m_PositionPath.Length > 1)
        {
            Gizmos.color = m_LineColor;
            Gizmos.matrix = Matrix4x4.TRS(owner.transform.position, owner.transform.rotation, owner.transform.localScale);
            for (int i = 0; i < m_PositionPath.Length - 1; i++)
            {
                Gizmos.DrawLine(m_PositionPath[i], m_PositionPath[i + 1]);
            }
        }
    }
    public void getPath()
    {
        switch (m_ActionMode)
        {
            case CutsceneActionMode.BEZIER:
                m_PositionPath = m_BezierAction.getPath();
                break;
            case CutsceneActionMode.STRAIGHT:
                m_PositionPath = m_StraightAction.getPath();
                break;
        }
    }
}