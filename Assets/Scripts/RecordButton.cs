using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.York.AudioRecordingBooth
{
    public class RecordButton : MonoBehaviour
    {
        [SerializeField]
        private AudioRecorderProcess process;

        [SerializeField]
        private Button button;

        private void OnEnable()
        {
            if (process != null)
            {
                process.onRecordingStarted.AddListener(OnRecordingStarted);
                process.onRecordingFinished.AddListener(OnRecordingFinished);
            }

            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnDisable()
        {
            if (process != null)
            {
                process.onRecordingStarted.RemoveListener(OnRecordingStarted);
                process.onRecordingFinished.RemoveListener(OnRecordingFinished);
            }

            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
            }
        }

        private void Start()
        {
            
        }

        private void OnClick()
        {
            if (process != null)
            {
                if (!process.isRecording)
                {
                    Debug.Log("start");
                    process.OnStartRecording();
                }
                else
                {
                    Debug.Log("stop");
                    process.OnStopRecording();
                }
            }
        }

        private void OnRecordingStarted()
        {
            
        }

        private void OnRecordingFinished()
        {

        }
    }
}

