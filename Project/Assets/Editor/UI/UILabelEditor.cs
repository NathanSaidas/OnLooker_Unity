using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Gem
{
    /// <summary>
    /// A custom editor which updates the external components of the UILabel everytime the GUI changes.
    /// </summary>
    [CustomEditor(typeof(UILabel))]
    public class UILabelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if(GUI.changed)
            {
                UILabel inspected = target as UILabel;
                if(inspected != null)
                {
                    inspected.UpdateComponents();
                    EditorUtility.SetDirty(inspected);
                }
            }
        }
    }
}