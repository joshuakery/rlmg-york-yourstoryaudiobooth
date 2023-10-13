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

            process.onTrimError += Open;
            process.onFail += OnFail;
            process.onTrimSuccess += Close;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            process.onTrimError -= Open;
            process.onFail -= OnFail;
            process.onTrimSuccess -= Close;
        }

        private void OnFail(System.Exception e)
        {
            Open();
        }
    }
}


