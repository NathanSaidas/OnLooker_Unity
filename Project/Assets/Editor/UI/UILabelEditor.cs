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
            UILabel inspected = target as UILabel;
            DrawDefaultInspector();
            GUI.enabled = false;
            EditorUtilities.ObjectField<Material>("Material",inspected.material);
            GUI.enabled = true;
            if(GUI.changed)
            {
                
                if(inspected != null)
                {
                    inspected.UpdateComponents();
                    EditorUtility.SetDirty(inspected);
                }
            }
        }
    }
}