using UnityEngine;
using System.Collections.Generic;

namespace EndevGame
{


    public class CutsceneManager : MonoBehaviour
    {

        [SerializeField]
        private List<Cutscene> m_CutScenes = new List<Cutscene>();

        public List<Cutscene> cutscenes
        {
            get { return m_CutScenes; }
        }

        public void addCutscene(Cutscene aCutScene)
        {
            if(aCutScene != null)
            {
                m_CutScenes.Add(aCutScene);
            }
        }
        public Cutscene addCutscene(string aName)
        {
            if(aName != string.Empty)
            {
                GameObject gameObject = new GameObject("Cutscene_" + aName);
                gameObject.transform.parent = transform;
                Cutscene cutscene = gameObject.AddComponent<Cutscene>();
                addCutscene(cutscene);
                cutscene.cutsceneName = aName;
                return cutscene;
            }
            return null;
        }
        public void removeCutscene(Cutscene aCutScene)
        {
            if(aCutScene != null)
            {
                m_CutScenes.Remove(aCutScene);
                
            }
        }
        public void removeCutscene(int aIndex)
        {
            if(m_CutScenes.Count > 0 && aIndex < m_CutScenes.Count)
            {
                m_CutScenes.RemoveAt(aIndex);
            }
        }
        public void removeCutscene(string aName)
        {
            for(int i = 0; i < m_CutScenes.Count; i++)
            {
                if(m_CutScenes[i].cutsceneName == aName)
                {
                    m_CutScenes.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
