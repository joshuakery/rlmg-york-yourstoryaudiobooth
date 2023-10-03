using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JoshKery.GenericUI.DOTweenHelpers
{
    [CustomEditor(typeof(BaseWindow), true)]
    public class BaseWindowEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BaseWindow script = (BaseWindow)target;
            if (GUILayout.Button("Open"))
            {
                script.Open(SequenceType.UnSequenced);
            }
            if (GUILayout.Button("Close"))
            {
                script.Close(SequenceType.UnSequenced);
            }
        }
    }
}
