using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class TrimErrorDisplay : BaseWindow
    {
        private AudioTrimProcess process;

        protected override void Awake()
        {
            base.Awake();

            process = FindObjectOfType<AudioTrimProcess>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            process.onTrimError += OnTrimError;
            process.onFail += OnFail;
            process.onTrimSuccess += OnTrimSuccess;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            process.onTrimError -= OnTrimError;
            process.onFail -= OnFail;
            process.onTrimSuccess -= OnTrimSuccess;
        }

        private void OnTrimError()
        {
            Open();
        }

        private void OnFail(System.Exception e)
        {
            Open();
        }

        private void OnTrimSuccess()
        {
            Close();
        }
    }
}


