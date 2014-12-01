using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gem
{

    public class BatchingUtil : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("CONTEXT/BatchingUtil/BatchTargets")]
        private static void BatchTargets(MenuCommand aCommand)
        {
            BatchingUtil batcher = aCommand.context as BatchingUtil;
            if(batcher == null)
            {
                return;
            }
            List<GameObject> targets = new List<GameObject>();
            IEnumerator iter = batcher.transform.GetEnumerator();
            while(iter.MoveNext())
            {
                Transform transform = iter.Current as Transform;
                if(transform == null)
                {
                    DebugUtils.LogWarning("Missing transform while trying to batch targets.");
                    continue;
                }
                if(transform.renderer != null)
                {
                    targets.Add(transform.gameObject);
                }
            }

            batcher.m_Targets = targets.ToArray();
            EditorUtility.SetDirty(batcher);
        }
#endif

        [SerializeField]
        GameObject m_Root = null;

        [SerializeField]
        GameObject[] m_Targets = null;
        // Use this for initialization
        void Start()
        {
            StaticBatchingUtility.Combine(m_Targets, m_Root);
        }
    }
}