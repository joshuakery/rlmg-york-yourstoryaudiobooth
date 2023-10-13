using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class RecordingSession : MonoBehaviour
    {
        public enum State
        {
            Error = -1,
            NotYetStarted = 0,
            RecordingNow = 1,
            CompletedRecording = 2,
            SuccessfulRecording = 3
        }

        private AudioRecorderProcess process;

        public State state;

        private void Awake()
        {
            if (process == null)
                process = FindObjectOfType<AudioRecorderProcess>();
        }

        private void OnEnable()
        {
            if (process != null)
            {
                process.onFirstProcessStarted += OnFirstProcessStarted;
                process.onStopRequested += OnStopRequested;
                process.onFail += OnFail;
                process.onRecordingSuccess += OnRecordingSuccess;
                process.onRecordingError += OnRecordingError;
                process.onInit += Init;
            }
        }

        private void OnDisable()
        {
            if (process != null)
            {
                process.onFirstProcessStarted -= OnFirstProcessStarted;
                process.onStopRequested -= OnStopRequested;
                process.onFail -= OnFail;
                process.onRecordingSuccess -= OnRecordingSuccess;
                process.onRecordingError -= OnRecordingError;
                process.onInit -= Init;
            }
        }

        private void OnFirstProcessStarted()
        {
            state = State.RecordingNow;
        }

        private void OnStopRequested()
        {
            state = State.CompletedRecording;
        }

        private void OnFail(System.Exception e)
        {
            state = State.Error;
        }

        private void OnRecordingSuccess()
        {
            state = State.SuccessfulRecording;
        }

        private void OnRecordingError()
        {
            state = State.Error;
        }

        private void Init()
        {
            state = State.NotYetStarted;
        }
    }
}


