using UnityEngine;
using System;
using System.Collections.Generic;

namespace EndevGame
{

    public delegate void CutSceneStateChangeCallback(Cutscene aCutScene);

    [Serializable]
    public class Cutscene : MonoBehaviour
    {
        [SerializeField]
        private bool m_DrawPath;
        //Editor Field..
        [SerializeField]
        private bool m_ShowCutScene;
        private enum State
        {
            STOPPED,
            PLAYING,
            PAUSED
        }
        private State m_State = State.STOPPED;
        //[SerializeField]
        //private Transform m_Parent = null;
        [SerializeField]
        private string m_CutsceneName;
        [SerializeField]
        private string m_SceneName;

        [SerializeField]
        private List<CutsceneInstruction> m_Instructions = new List<CutsceneInstruction>();
        [SerializeField]
        private int m_CurrentInstruction = 0;

        private event CutSceneStateChangeCallback m_ChangeOfState = null;


        private void Start()
        {

        }
        private void OnEnable()
        {

        }
        private void OnDisable()
        {

        }
        private void OnDestroy()
        {
            CameraManager.instance.unregisterCutscene(this);
        }

        private void Update()
        {
            CameraManager.instance.registerCutscene(this);
        }

        //Called every frame the cutscene is playing
        public void update()
        {

            switch (m_State)
            {
                case State.PLAYING:
                    if (m_Instructions.Count > m_CurrentInstruction)
                    {
                        m_Instructions[m_CurrentInstruction].update();
                        if (m_Instructions[m_CurrentInstruction].isFinished == true)
                        {
                            m_CurrentInstruction++;
                            if (m_CurrentInstruction >= m_Instructions.Count)
                            {
                                Debug.Log("Finished");
                                stop();
                            }

                        }
                    }
                    break;
            }
        }


        public void play()
        {
            Debug.Log("Play");
            if (m_State != State.PLAYING)
            {
                if (m_ChangeOfState != null)
                {
                    m_ChangeOfState.Invoke(this);
                }
            }
            m_State = State.PLAYING;
            if (m_CurrentInstruction >= m_Instructions.Count - 1)
            {
                m_CurrentInstruction = 0;
            }
            if (m_CurrentInstruction < m_Instructions.Count)
            {
                CameraManager.instance.cutsceneCamera.transform.position = m_Instructions[m_CurrentInstruction].currentGoal;
            }
        }
        //Plays from the specified index
        public void play(int aIndex)
        {
            if (m_State != State.PLAYING)
            {
                if (m_ChangeOfState != null)
                {
                    m_ChangeOfState.Invoke(this);
                }
            }
            m_CurrentInstruction = Mathf.Clamp(aIndex, 0, m_Instructions.Count);
            if (m_CurrentInstruction < m_Instructions.Count)
            {
                m_Instructions[m_CurrentInstruction].reset();
                CameraManager.instance.cutsceneCamera.transform.position = m_Instructions[m_CurrentInstruction].currentGoal;
            }
            m_State = State.PLAYING;
        }
        //Stops the cutscene from playing, resets it aswell
        public void stop()
        {
            if (m_State != State.STOPPED)
            {
                if (m_ChangeOfState != null)
                {
                    m_ChangeOfState.Invoke(this);
                }
            }
            m_State = State.STOPPED;
            reset();
        }
        //Pauses the cutscene keeping the current insturction index
        public void pause()
        {
            if (m_State != State.PAUSED)
            {
                if (m_ChangeOfState != null)
                {
                    m_ChangeOfState.Invoke(this);
                }
            }
            m_State = State.PAUSED;
        }
        //Resets all the instructions back to the default state
        public void reset()
        {
            m_CurrentInstruction = 0;
            for (int i = 0; i < m_Instructions.Count; i++)
            {
                m_Instructions[i].reset();
            }
        }


        public void registerStateChange(CutSceneStateChangeCallback aCallback)
        {
            if (aCallback != null)
            {
                m_ChangeOfState += aCallback;
            }
        }
        public void unregisterStateChange(CutSceneStateChangeCallback aCallback)
        {
            if (aCallback != null)
            {
                m_ChangeOfState -= aCallback;
            }
        }

        //public Transform parent
        //{
        //    get { return m_Parent; }
        //}
        public bool isPaused
        {
            get { return m_State == State.PAUSED; }
        }
        public bool isPlaying
        {
            get { return m_State == State.PLAYING; }
        }
        public bool isStopped
        {
            get { return m_State == State.STOPPED; }
        }

        public List<CutsceneInstruction> instructions
        {
            get { return m_Instructions; }
        }

        public string cutsceneName
        {
            get { return m_CutsceneName; }
            set { m_CutsceneName = value; }
        }
        public string sceneName
        {
            get { return m_SceneName; }
            set { m_SceneName = value; }
        }
        //To show in the editor or not
        public bool showCutscene
        {
            get { return m_ShowCutScene; }
            set { m_ShowCutScene = value; }
        }
        public void addInstruction(string aName, CutsceneActionMode aMode)
        {
            CutsceneInstruction instruction = new CutsceneInstruction(this);
            instruction.name = aName;
            instruction.setAsActionMode(aMode);
            m_Instructions.Add(instruction);
        }

        public void removeInstruction(int aIndex)
        {
            if (aIndex >= 0 && aIndex < m_Instructions.Count)
            {
                m_Instructions.RemoveAt(aIndex);
            }
        }
        public void removeInstruction(string aName)
        {
            for (int i = 0; i < m_Instructions.Count; i++)
            {
                if (m_Instructions[i].name == aName)
                {
                    m_Instructions.RemoveAt(i);
                    return;
                }
            }
        }
        public void removeInstruction(CutsceneInstruction aInstruction)
        {
            if (aInstruction != null)
            {
                m_Instructions.Remove(aInstruction);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (m_Instructions != null && m_Instructions.Count > 0)
            {
                for (int i = 0; i < m_Instructions.Count; i++)
                {
                    m_Instructions[i].gizmosDrawPath();
                    m_Instructions[i].gizmosDrawTargets();
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (m_DrawPath == true)
            {
                if (m_Instructions != null && m_Instructions.Count > 0)
                {
                    for (int i = 0; i < m_Instructions.Count; i++)
                    {
                        m_Instructions[i].gizmosDrawPath();
                        m_Instructions[i].gizmosDrawTargets();
                    }
                }
            }
        }
    }
}