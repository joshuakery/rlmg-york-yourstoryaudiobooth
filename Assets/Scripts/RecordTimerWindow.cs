using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    /// <summary>
    /// Controls the background element BaseWindows
    /// </summary>
    public class RecordTimerWindow : BaseWindow
    {
        private AudioRecorderProcess process;

        protected override void Awake()
        {
            base.Awake();

            if (process == null)
                process = FindObjectOfType<AudioRecorderProcess>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (process != null)
            {
                process.onFirstProcessStarted += OnFirstProcessStarted;
                process.onStopRequested += CompleteCurrentSequenceAndClose;
                process.onFail += CloseOnFail;
                process.onInit += CompleteCurrentSequenceAndClose;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (process != null)
            {
                process.onFirstProcessStarted -= OnFirstProcessStarted;
                process.onStopRequested -= CompleteCurrentSequenceAndClose;
                process.onFail -= CloseOnFail;
                process.onInit -= CompleteCurrentSequenceAndClose;
            }
        }

        private void OnFirstProcessStarted()
        {
            Open();
        }

        private void CloseOnFail(System.Exception e)
        {
            Close(SequenceType.UnSequenced);
        }

        private void CompleteCurrentSequenceAndClose()
        {
            sequenceManager.CompleteCurrentSequence();
            Close();
        }
    }

}

