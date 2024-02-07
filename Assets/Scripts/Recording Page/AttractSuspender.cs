using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class AttractSuspender : MonoBehaviour
    {
        [SerializeField]
        private AttractScreen attractScreen;

        private AudioRecorderProcess recorderProcess;

        private void Awake()
        {
            recorderProcess = FindObjectOfType<AudioRecorderProcess>();
        }

        private void Update()
        {
            if (attractScreen != null && recorderProcess != null)
            {
                if (recorderProcess.isRecording)
                    attractScreen.timeOfLastInput = Time.time;
            }
        }
    }
}

