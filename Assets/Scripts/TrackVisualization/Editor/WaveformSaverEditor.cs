using UnityEngine;
using UnityEditor;

namespace JoshKery.York.AudioRecordingBooth
{
    [CustomEditor(typeof(WaveformSaver), true)]
    public class WaveformSaverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WaveformSaver script = (WaveformSaver)target;

            if (GUILayout.Button("Redraw"))
            {
                script.OnSaveWaveform();
            }
        }
    }
}

