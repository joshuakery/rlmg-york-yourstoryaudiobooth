using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.York.AudioRecordingBooth
{
    public class TimerDisplay : MonoBehaviour
    {
        private AudioRecorderProcess process;

        [SerializeField]
        private Image radialFill;

        private void Awake()
        {
            if (process == null)
                process = FindObjectOfType<AudioRecorderProcess>();
        }

        private void OnEnable()
        {
            if (process != null)
                process.onInit += OnInit;
        }

        private void OnDisable()
        {
            if (process != null)
                process.onInit -= OnInit;
        }

        private void OnInit()
        {
            if (radialFill != null)
                radialFill.fillAmount = 0;
        }

        private void Update()
        {
            if (process.isRecording && process.StartTime != null && !process.wasStopRequested)
            {
                TimeSpan timeSinceStart = (TimeSpan)(DateTime.Now - process.StartTime);
                float fill = (float)timeSinceStart.TotalMilliseconds / (float)process.RecordingDuration;

                //Round to 1 if we are very close
                fill = 1f - fill < 0.001f ? 1f : fill;

                if (radialFill != null)
                    radialFill.fillAmount = fill;
            }
        }
    }
}


